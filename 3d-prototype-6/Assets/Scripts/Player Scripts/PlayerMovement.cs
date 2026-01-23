using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Camera")]
    public Transform playerCam;
    public float cameraSensitivity = 20f;
    public float yawRotationSpeed, pitchRotationSpeed;
    public float standCamLevel = .55f;
    public float positionChangeSpeed = .15f;
    [HideInInspector] public float targetCamLevel = 0f;

    [Header("Move Conditions")]
    public bool isGrounded;
    public bool isMoving;
    public bool isSprinting;

    public bool canMove;
    [Header("Player Movement")]
    public float moveSpeed = 6f;
    public float sprintMult = 1.75f;
    public float gravityScale = 9.81f;
    public float jumpHeight;
    public float airborneDamp = .15f;
    public float upwardGravMult = 3f;

    public PositionState position;
    private PositionState lastPosition = PositionState.Stand;
    [Header("Crouch")]
    public float crouchMult = .5f;
    public float crouchHeight = .35f;
    public float crouchCamLevel = .325f;

    [Header("Prone")]
    public float proneMult = .15f;
    public float proneHeight = .35f;
    public float proneCamLevel;

    [Header("Acceleration")]
    public float groundAcceleration = 35f;
    public float groundDeceleration = 45f;
    public float airAcceleration = 8f;
    public float airDeceleration = 6f;
    public float maxAirSpeed = 7f;

    // public float turnSpeed = 20f;
    private Player main;
    private CharacterController controller;
    private Vector3 moveDir;
    private Vector3 cachedDir;
    private Vector3 aimInputVector;
    public Vector3 lookInput {get { return aimInputVector;}}
    private Vector3 vel;
    private Vector3 horizVel;
    private Quaternion normalYawRot;
    private Quaternion normalPitchRot;
    private bool wasGrounded;
    private float standingHeight;
    
    [Header("Recoil")]
    public float newPitchAngle;
    public float newYawAngle;
    public float recoilKickSmoothTime = 0.03f;   // fast kick
    public float recoilReturnSpeed = 10f;        // slow return (higher = faster return)
    public float recoilYawOffset;
    public float recoilPitchOffset;
    private float recoilYawTarget;
    private float recoilPitchTarget;
    private float recoilYawVel;
    private float recoilPitchVel;
    private float currentPitch;
    private float currentYaw;
    private float yawVel;
    private float pitchVel;

    // Optional: cached "normal" rotations (requested)
    private Quaternion cachedNormalPlayerRot;
    private Quaternion cachedNormalCamRot;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        main = GetComponent<Player>();
        targetCamLevel = standCamLevel;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        newYawAngle = transform.eulerAngles.y;

        newPitchAngle = playerCam.localEulerAngles.x;
        if (newPitchAngle > 180f) newPitchAngle -= 360f;

        standingHeight = controller.height;
    }

    public void MovementInput(Vector2 dir)
    {
        moveDir = new Vector3(dir.x, 0f, dir.y);
    }

    public void SprintInput(bool s)
    {
        isSprinting = s;
    }

    public void LookInput(Vector2 dir)
    {
        aimInputVector = new Vector3(dir.x, 0f, dir.y);
    }

    public void Jump()
    {
        if (position != PositionState.Stand)
        {
            position = PositionState.Stand;
            StartCoroutine(PositionChangeLerp(position));
        }
        else
        {
            if (isGrounded) vel.y = Mathf.Sqrt(jumpHeight * 2f * gravityScale);
        }
    }

    public void Crouch()
    {
        if (isGrounded)
        {
            lastPosition = position;
            if (position == PositionState.Crouch)
                position = PositionState.Stand;
            else
                position = PositionState.Crouch;

            StartCoroutine(PositionChangeLerp(position));
        }
    }

    public void Prone()
    {
        if (isGrounded)
        {

            if (lastPosition == PositionState.Stand)
                position = PositionState.Prone;
            else if (lastPosition == PositionState.Prone)
                position = PositionState.Stand;
            StartCoroutine(PositionChangeLerp(position));
        }
    }

    IEnumerator PositionChangeLerp(PositionState positionState)
    {
        float targetHeight = 0f;

        switch (positionState)
        {
            case PositionState.Stand:
                targetHeight = standingHeight;
                targetCamLevel = standCamLevel;
                break;
            case PositionState.Crouch:
                targetHeight = crouchHeight;
                targetCamLevel = crouchCamLevel;
                break;
            case PositionState.Prone:
                targetHeight = proneHeight;
                targetCamLevel = proneCamLevel;
                break;
        }

        float startHeight = controller.height;

        Vector3 pos = main.body.head.localPosition;
        float startCamLevel = pos.y;

        float t = 0;

        bool ceiling = false;
        while (t < 1)
        {
            Vector3 origin = transform.position + new Vector3(0, controller.height / 2, 0);
            ceiling = Physics.Raycast(origin, Vector3.up, 0.2f, Layer.Wall | Layer.Ground) && position == PositionState.Stand;

            if (!ceiling)
            {
                pos.y = Mathf.Lerp(startCamLevel, targetCamLevel, t);
                main.body.head.localPosition = pos;
                controller.height = Mathf.Lerp(startHeight, targetHeight, t);

                t += Time.deltaTime / positionChangeSpeed;
            }
            else { ceiling = true; break; }


            yield return null;
        }

        if (!ceiling)
        {
            pos.y = targetCamLevel;
            main.body.head.localPosition = pos;

            controller.height = targetHeight;
        }
    }

    void Update()
    {
        Move();

        Look();
    }
    private void Move()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;

        Vector3 input = new Vector3(moveDir.x, 0f, moveDir.z);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        isMoving = input.sqrMagnitude > 0.0001f;

        // Desired horizontal velocity relative to player facing
        Vector3 desiredHoriz = (transform.right * input.x + transform.forward * input.z);
        float spd = moveSpeed;
        if (isMoving)
        {
            switch (position)
            {
                case PositionState.Stand:
                    if (isSprinting) spd *= sprintMult;
                    break;
                case PositionState.Crouch:
                    spd *= crouchMult;
                    break;
                case PositionState.Prone:
                    spd *= proneMult;
                    break;
            }
        }
        else
            spd = 0f;
        desiredHoriz *= spd;
        // When we leave the ground, cache the current horizontal velocity to preserve momentum
        if (!isGrounded && wasGrounded)
            cachedDir = horizVel;

        // Build target horizontal velocity (ground = direct, air = keep cached + damped steering)
        Vector3 targetHoriz;
        if (isGrounded)
        {
            targetHoriz = desiredHoriz;
        }
        else
        {
            targetHoriz = cachedDir + (desiredHoriz * airborneDamp);

            // Optional clamp so air control can't stack you into crazy speed
            float max = Mathf.Max(moveSpeed, maxAirSpeed);
            if (targetHoriz.magnitude > max)
                targetHoriz = targetHoriz.normalized * max;
        }

        // Accelerate toward target
        float accel = isGrounded
            ? (isMoving ? groundAcceleration : groundDeceleration)
            : (isMoving ? airAcceleration : airDeceleration);

        horizVel = Vector3.MoveTowards(horizVel, targetHoriz, accel * Time.deltaTime);

        // Gravity
        if (isGrounded && vel.y < 0f)
            vel.y = -2f;
        else
        {
            if (vel.y > 0f) vel.y -= gravityScale * upwardGravMult * Time.deltaTime;
            else vel.y -= gravityScale * Time.deltaTime;
        }
        Vector3 motion = horizVel + new Vector3(0f, vel.y, 0f);
        controller.Move(motion * Time.deltaTime);
    }

    public void AddRecoil(float yawDelta, float pitchDelta)
    {
        recoilYawTarget += yawDelta;
        recoilPitchTarget += pitchDelta;

        // prevents flipping if you spam recoil
        recoilPitchTarget = Mathf.Clamp(recoilPitchTarget, -30f, 30f);
    }
    private void Look()
    {
        newYawAngle += aimInputVector.x * cameraSensitivity * Time.deltaTime;
        newPitchAngle -= aimInputVector.z * cameraSensitivity * Time.deltaTime;

        newPitchAngle = Mathf.Clamp(newPitchAngle, -89f, 89f);

        currentYaw = Mathf.SmoothDamp(currentYaw, newYawAngle, ref yawVel, yawRotationSpeed);
        currentPitch = Mathf.SmoothDamp(currentPitch, newPitchAngle, ref pitchVel, pitchRotationSpeed);

        cachedNormalPlayerRot = Quaternion.Euler(0f, currentYaw, 0f);
        cachedNormalCamRot = Quaternion.Euler(currentPitch, 0f, 0f);

        recoilYawOffset = Mathf.SmoothDamp(recoilYawOffset, recoilYawTarget, ref recoilYawVel, recoilKickSmoothTime);
        recoilPitchOffset = Mathf.SmoothDamp(recoilPitchOffset, recoilPitchTarget, ref recoilPitchVel, recoilKickSmoothTime);


        recoilYawTarget = Mathf.Lerp(recoilYawTarget, 0f, recoilReturnSpeed * Time.deltaTime);
        recoilPitchTarget = Mathf.Lerp(recoilPitchTarget, 0f, recoilReturnSpeed * Time.deltaTime);
        HUDManager.CursorRecoil(recoilPitchTarget);

        transform.rotation = cachedNormalPlayerRot * Quaternion.Euler(0f, recoilYawOffset, 0f);
        playerCam.localRotation = cachedNormalCamRot * Quaternion.Euler(recoilPitchOffset, 0f, 0f);
    }
    public enum PositionState
    {
        Stand,
        Crouch,
        Prone
    }
}
