using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;
    public float maxSpeed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.5f;
    bool onSlope = false;
    public Rigidbody rb;
    private Vector3 inputDirection;
    private Vector3 currentVelocity;
    private Player player;
    public bool isDashing = false;
    private float defaultSpeed;
    private Coroutine dashRoutine;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnDisable()
    {
        inputDirection = Vector3.zero;
        rb.velocity = Vector3.zero;

        player.body.Play("Forward", 0f);
        player.body.Play("Strafe", 0f);
    }

    void Update()
    {
        MyInput();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Movement();
        }
        SnapToGround();
        UpdateAnimation();
    }

    /// <summary>
    /// Input from the player
    /// </summary>
    public void MyInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(x, 0, y).normalized;

        // Dash logic
        if ((Input.GetKeyDown(KeyCode.R) || 0.5f <= Input.GetAxis("Dash")) && !isDashing && PlayerManager.Instance.CanDash())
        {
            if (dashRoutine != null)
                StopCoroutine(dashRoutine);
            dashRoutine = StartCoroutine(Dash(transform.forward));
            PlayerManager.Instance.Dash();
        }
    }

    /// <summary>
    /// Accelerate the player in the aimed direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    IEnumerator Dash(Vector3 dir)
    {
        isDashing = true;
        rb.velocity = dir.normalized * dashSpeed;
        PlayerManager.Instance.NukeImmunity();
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    /// <summary>
    /// Movement logic
    /// </summary>
    public void Movement()
    {
        if (inputDirection.magnitude > 0)
        {
            // Accelerate toward desired velocity
            Vector3 targetVelocity = inputDirection * moveSpeed;
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Decelerate to stop
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        // Clamp max speed before applying to Rigidbody
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
        }

        rb.useGravity = !onSlope;
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
    }

    /// <summary>
    /// Snap the player to the ground. It also prevents the player from sliding down slopes
    /// </summary>
    public void SnapToGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.25f, 1 << 6))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            onSlope = angle > 10f;

            Vector3 vel = rb.velocity;
            vel.y = 0f;
            rb.velocity = vel;
        }
        else onSlope = false;
    }

    /// <summary>
    /// Updates the player's animations based on movement
    /// </summary>
    void UpdateAnimation()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0f;

        if (velocity.magnitude < 0.01f)
        {
            player.body.Play("Forward", 0f);
            player.body.Play("Strafe", 0f);
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardAmount = Vector3.Dot(velocity.normalized, forward);
        float strafeAmount = Vector3.Dot(velocity.normalized, right);

        player.body.Play("Forward", forwardAmount);
        player.body.Play("Strafe", strafeAmount);
    }

    /// <summary>
    /// Change the player's speed given the percentage
    /// </summary>
    /// <param name="percentage"></param>
    public void AlterSpeed(float percentage)
    {
        maxSpeed += maxSpeed * percentage;
    }
}
