using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState{
        Running,
        Sliding,
        Crouching,
    }

    [Header("Movement")]

    public MovementState moveState;

    [Header("Running Speed")]
    public float moveSpeed;
    public float maxSpeed;
    

    [Header("Jumping")]
    public float jumpForce;
    public int jumpCount;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float startYScale;
    
    [Header("Sliding")]
    public float slidingSpeed;
    public bool canSlide;
    public bool noCancel = false;
    public float standUpTime;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerCam cam;

    public PlayerInput playerInput;
    public InputAction move;
    private Vector3 moveDirection;
    private float xDir;
    private float yDir;
    private Vector3 prevRight;
    private Vector3 prevForward;

    public Rigidbody rb;


    void Awake()
    {
        playerInput = new PlayerInput();
        isGrounded = true;
        startYScale = transform.localScale.y;
    }

    void OnEnable()
    {
        move = playerInput.GroundMovement.Move;
        playerInput.Enable();
    }

    void OnDisable(){
        playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move(){
        xDir = move.ReadValue<Vector2>().x;
        yDir = move.ReadValue<Vector2>().y;

        moveDirection += xDir * orientation.right * moveSpeed;
        moveDirection += yDir * orientation.forward * moveSpeed;
    
        rb.AddForce(moveDirection * Time.fixedDeltaTime, ForceMode.Impulse);
        moveDirection = Vector3.zero;
    }
}
