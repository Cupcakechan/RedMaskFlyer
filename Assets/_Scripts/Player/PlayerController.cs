using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float flySpeed = 4f;       // upward speed while holding

    [Header("Ground Movement")]
    [SerializeField] private float walkSpeed = 3f;      // left/right walk speed on ground

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;     // empty child at the feet
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private bool isGrounded;
    private float horizontalInput;
    private bool flyHeld;
    private string currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadInput();
        HandleFlip();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        HandleMovement();
    }

    void ReadInput()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;

        flyHeld = false;
        if (kb != null && (kb.spaceKey.isPressed || kb.wKey.isPressed || kb.upArrowKey.isPressed))
            flyHeld = true;
        if (mouse != null && mouse.leftButton.isPressed)
            flyHeld = true;

        horizontalInput = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontalInput = -1f;
            else if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontalInput = 1f;
        }
    }

    void HandleMovement()
    {
        if (flyHeld)
            rb.linearVelocity = new Vector2(0f, flySpeed);          // rise
        else if (isGrounded)
            rb.linearVelocity = new Vector2(horizontalInput * walkSpeed, rb.linearVelocity.y); // walk
        else
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // fall (gravity)
    }

    void HandleFlip()
    {
        if (horizontalInput < 0f) sr.flipX = true;
        else if (horizontalInput > 0f) sr.flipX = false;
    }

    void UpdateAnimation()
    {
        string newState;
        if (!isGrounded || flyHeld) newState = "Player_Fly";
        else if (Mathf.Abs(horizontalInput) > 0.01f) newState = "Player_Walk";
        else newState = "Player_Idle";

        if (newState != currentState)
        {
            animator.Play(newState);
            currentState = newState;
        }
    }
}