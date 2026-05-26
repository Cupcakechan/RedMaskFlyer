using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement (air + ground)")]
    [SerializeField] private float moveSpeed = 4f;        // max horizontal speed
    [SerializeField] private float acceleration = 25f;    // how quickly we ease to target speed

    [Header("Flight")]
    [SerializeField] private float flySpeed = 4f;         // upward speed while holding

    [Header("Screen Bounds (X clamp)")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private bool isGrounded;
    private float horizontalInput;
    private bool flyHeld;

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
        // Smooth horizontal ease toward target speed (works in air AND on ground)
        float targetX = horizontalInput * moveSpeed;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetX, acceleration * Time.fixedDeltaTime);

        // Vertical: thrust up while held, otherwise let gravity pull down
        float newY = flyHeld ? flySpeed : rb.linearVelocity.y;

        rb.linearVelocity = new Vector2(newX, newY);

        // Keep the player within horizontal screen bounds
        if (rb.position.x < minX || rb.position.x > maxX)
        {
            float clampedX = Mathf.Clamp(rb.position.x, minX, maxX);
            rb.position = new Vector2(clampedX, rb.position.y);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void HandleFlip()
    {
        if (horizontalInput < 0f) sr.flipX = true;
        else if (horizontalInput > 0f) sr.flipX = false;
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsFlying", flyHeld || !isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }
}