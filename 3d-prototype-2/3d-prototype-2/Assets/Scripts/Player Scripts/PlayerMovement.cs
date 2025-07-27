// Some stupid rigidbody based movement by Dani
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Components")]
    //Assingables
    public Transform playerCam;
    public Transform orientation;

    private Quaternion startingRot;

    //Other
    private Rigidbody rb;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    [Header("Movement")]
    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    [Header("Sliding")]
    //Crouch & Slide
    public Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    public Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float minSlideSpeed = 2f;
    private bool isSliding;
    private bool justSlid;

    [Header("Jumping")]
    //Jumping
    public bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    [Header("Dashing")]
    // Dashing
    public bool readyToDash = true;
    public bool isDashing = false;
    public bool isKnocked = false;
    public float dashCooldown = .75f;
    public float dashTime = .25f;
    public float knockedTime = .05f;
    public float dashForce = 250f;
    [Header("Movement Abilities")]
    public bool abilityMovement;
    public bool abilityWindow;

    [Header("Attack Dash")]
    // Dashing
    public bool canFlyKick = true;
    public bool isFlyingKick = false;
    public bool flyKickWindow = false;
    public float flyingKickCD = 1f;
    public float flyingKickTime = .5f;
    public float flyingKickForce = 150f;
    public Vector3 startingPos;
    //Input
    float x, y;
    bool jumping, sprinting, crouching, dashing;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    private Player player;
    public void SetPlayer(Player p) => player = p;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        startingRot = transform.rotation;
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        dashing = Input.GetKeyDown(KeyCode.LeftShift);

        if (x == 0.0f && y == 0.0f)
            player.BoolAnim("IsWalking", false);
        else
            player.BoolAnim("IsWalking", true);
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && !dashing)
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();

        //If ready to dash && presses dash, then dash
        if (readyToDash && dashing && !isDashing && !crouching)
            Dash();
    }

    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z);

        if (grounded && rb.velocity.magnitude > minSlideSpeed && !isSliding && !isDashing)
        {
            isSliding = true;
            justSlid = true;
            rb.AddForce(orientation.forward * slideForce);
            Invoke(nameof(StopSliding), slideDuration);
        }
    }

    private void StopSliding()
    {
        isSliding = false;
    }



    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        isSliding = false;
        justSlid = false;
    }

    public void StopMovement()
    {
        rb.velocity = Vector3.zero;
        isSliding = false;
    }

    private void Movement()
    {
        //Extra gravity
        if (!isDashing && !isKnocked)
            rb.AddForce(Vector3.down * Time.deltaTime * 10);
        else
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();



        //Set max speed
        float maxSpeed = this.maxSpeed;

        if (isDashing || isFlyingKick || isKnocked || isSliding || abilityMovement) return;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (isSliding && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        // Movement while sliding
        if (grounded && crouching) multiplierV = .5f;
        else if (grounded && justSlid) multiplierV = 0f;

        Vector3 moveDirection = orientation.transform.forward * y;
        moveDirection += orientation.transform.right * x;
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        //Apply forces to move player
        rb.AddForce(moveDirection * moveSpeed * Time.deltaTime * multiplier * multiplierV);

        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    /// <summary>
    /// A state in which the player is no longer kinematic so that abilities
    /// can influence movement freely.
    /// </summary>
    public void DashState()
    {
        rb.useGravity = false;
        
        rb.velocity *= .25f;
    }
    public void AbilityWindow() {
        abilityMovement = true;
        abilityWindow = true;
        startingPos = transform.position;
        Invoke(nameof(StartAbilityWindow), Time.fixedDeltaTime * 1f);
    }
    public void DashTotarget(GameObject target)
    {
        if (canFlyKick && !isDashing)
        {
            isFlyingKick = true;
            canFlyKick = false;
            flyKickWindow = true;
            player.PlayAnim("Dash");

            rb.velocity = Vector3.zero;

            Rigidbody targetRB = target.GetComponent<Rigidbody>();
            startingPos = transform.position;
            startingPos.y -= .5f;
            Vector3 futurePos = target.transform.position + (targetRB.velocity * .15f);

            Vector3 dashDirection = futurePos - startingPos;
            float magnitude = dashDirection.magnitude;
            rb.AddForce(normalVector * jumpForce * 1.75f);
            rb.AddForce(dashDirection.normalized * flyingKickForce * magnitude);

            Invoke(nameof(StopAttackDash), flyingKickTime);
            Invoke(nameof(StartFlyKickWindow), Time.fixedDeltaTime * 3f);
        }
    }

    public void DashToPoint(Vector3 target, float force)
    {
        DashState();
        rb.velocity = Vector3.zero;

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        rb.AddForce(Vector3.up * jumpForce * 0.75f);
        rb.AddForce(direction.normalized * force);

        Invoke(nameof(StopAbilityMovement), dashTime); // Use dashTime or a custom value
    }
    public void StopAbilityMovement()
    {
        abilityMovement = false;
        rb.useGravity = true;
    }
    public void KnockBack(Vector3 direction, float knockBackForce)
    {
        isKnocked = true;
        rb.useGravity = false;
        direction.y = 0f;
        rb.AddForce(normalVector * jumpForce);
        rb.AddForce(direction * knockBackForce);
        Invoke(nameof(StopKnocked), knockedTime);
    }

    private void Dash()
    {
        readyToDash = false;
        isDashing = true;
        rb.useGravity = false;
        player.PlayAnim("Dash");
        // Dash fast horizontally
        Vector3 dashDirection = orientation.transform.forward * y;
        dashDirection += orientation.transform.right * x;
        dashDirection.y = 0f;
        rb.AddForce(dashDirection * dashForce);

        // Reset Jump
        Invoke(nameof(StopDash), dashTime);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;
            player.PlayAnim("Jump");
            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void StopAttackDash()
    {
        isFlyingKick = false;
        Invoke(nameof(ResetAttackDash), flyingKickCD);
    }
    private void StopDash()
    {
        isDashing = false;
        rb.useGravity = true;
        Invoke(nameof(ResetDash), dashCooldown);
    }
    private void StopKnocked()
    {
        isKnocked = false;
        rb.useGravity = true;
    }
    private void ResetDash()
    {
        readyToDash = true;
    }
    private void ResetAttackDash()
    {
        canFlyKick = true;
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    public void ResetCamera()
    {
        playerCam.transform.localRotation = startingRot;
    }

    public void SetCamera(Quaternion cameraRot)
    {
        // Split quaternion into Euler angles
        Vector3 angles = cameraRot.eulerAngles;

        // Apply pitch to the camera (X), yaw to orientation and camera (Y)
        xRotation = angles.x;
        desiredX = angles.y;

        playerCam.transform.localRotation = Quaternion.Euler(angles.x, angles.y, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, angles.y, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping || isDashing || isFlyingKick || isKnocked || abilityMovement) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                flyKickWindow = false;
                abilityWindow = false;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
    private void StartFlyKickWindow()
    {
        flyKickWindow = true;
    }
    
    private void StartAbilityWindow()
    {
        abilityWindow = true;
    }
    
}