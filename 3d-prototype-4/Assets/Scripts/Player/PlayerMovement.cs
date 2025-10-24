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
    public float dashCooldown = 1f;
    public bool canDash;
    bool onSlope = false;
    public Rigidbody rb;
    public GameObject dashSphere;
    private Vector3 inputDirection;
    private Vector3 currentVelocity;
    public bool isDashing = false;
    public bool isEmoting = false;
    public float attackDashDuration = 0.25f;

    private Coroutine dashRoutine;

    private Player player;
    private PlayerBody body;
    private PlayerLobbyInfo stats;
    void Awake()
    {
        player = GetComponent<Player>();
        body = GetComponent<PlayerBody>();
        stats = GetComponent<PlayerLobbyInfo>();

    }

    void OnDisable()
    {
        inputDirection = Vector3.zero;
        rb.velocity = Vector3.zero;

        body.Play("Forward", 0f);
        body.Play("Strafe", 0f);
    }

    void Update()
    {

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
    public void MovementInput(Vector2 input)
    {
        float x = input.x;
        float y = input.y;

        inputDirection = new Vector3(x, 0, y).normalized;
    }

    public void EmoteInput(int input)
    {
        isEmoting = true;
        body.Play("Emote Float", input);
    }

    public void Dash()
    {
        if (stats.isRampage && !isDashing && canDash)
        {
            dashRoutine = StartCoroutine(AttackDash(transform.forward));
        }
        else if (!isDashing && stats.CanDash() && canDash)
        {
            if (dashRoutine != null)
                StopCoroutine(dashRoutine);
            dashRoutine = StartCoroutine(Dash(transform.forward));
            stats.Dash();
        }
    }

    /// <summary>
    /// Accelerate the player in the aimed direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    IEnumerator Dash(Vector3 dir)
    {
        canDash = false;
        isDashing = true;
        rb.velocity = dir.normalized * dashSpeed;
        player.NukeImmunity();
        player.cc.excludeLayers += (1 << 8);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        player.cc.excludeLayers -= (1 << 8);
    }

    /// <summary>
    /// Accelerate the player in the aimed direction
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    IEnumerator AttackDash(Vector3 dir)
    {
        canDash = false;
        isDashing = true;
        rb.velocity = dir.normalized * dashSpeed;
        body.animator.SetTrigger("Dash");
        player.immuneSphere.SetActive(true);
        player.cc.excludeLayers += (1 << 8);
        yield return new WaitForSeconds(attackDashDuration);
        isDashing = false;
        yield return new WaitForSeconds(.5f);
        canDash = true;
        player.immuneSphere.SetActive(false);
        player.cc.excludeLayers -= (1 << 8);
    }

    /// <summary>
    /// Movement logic
    /// </summary>
    public void Movement()
    {
        Transform cameraTransform = PlayerManager.Instance.vcam.transform;
        Vector3 camF = cameraTransform.forward; camF.y = 0f; camF.Normalize();
        Vector3 camR = cameraTransform.right; camR.y = 0f; camR.Normalize();

        Vector2 input2D = new Vector2(inputDirection.x, inputDirection.z);
        float inputMag = Mathf.Clamp01(input2D.magnitude);

        Vector3 moveDir = (camF * input2D.y + camR * input2D.x);
        if (moveDir.sqrMagnitude > 1e-6f) moveDir.Normalize();

        if (inputMag > 0f)
        {
            isEmoting = false;
            Vector3 targetVelocity = moveDir * (moveSpeed * inputMag);
            currentVelocity = Vector3.MoveTowards(
                currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;

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
            body.Play("IsMoving", false);
            body.Play("Forward", 0f);
            body.Play("Strafe", 0f);
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardAmount = Vector3.Dot(velocity.normalized, forward);
        float strafeAmount = Vector3.Dot(velocity.normalized, right);

        body.Play("Forward", forwardAmount);
        body.Play("Strafe", strafeAmount);
        body.Play("IsMoving", true);
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
