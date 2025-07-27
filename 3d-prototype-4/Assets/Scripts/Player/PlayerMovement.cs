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

    public Rigidbody rb;
    private Vector3 inputDirection;
    private Vector3 currentVelocity;
    private Player player;

    private bool isDashing = false;
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
            SnapToGround();
        }
        UpdateAnimation();
    }

    public void MyInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(x, 0, y).normalized;

        if (Input.GetKeyDown(KeyCode.R) && !isDashing && PlayerManager.Instance.CanDash())
        {
            if (dashRoutine != null)
                StopCoroutine(dashRoutine);
            dashRoutine = StartCoroutine(Dash(transform.forward));
            PlayerManager.Instance.Dash();
        }
    }

    IEnumerator Dash(Vector3 dir)
    {
        isDashing = true;
        rb.velocity = dir.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

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

        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
    }

    public void SnapToGround()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Vector3 pos = new Vector3(
                    transform.position.x,
                    hit.point.y,
                    transform.position.z
                );
                transform.position = pos;
            }
        }
    }
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

}
