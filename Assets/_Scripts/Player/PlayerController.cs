using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Flight (manual acceleration, gravity off)")]
    [SerializeField] private float maxRiseSpeed = 5f;    // top upward speed while holding
    [SerializeField] private float maxFallSpeed = 7f;    // top downward speed when released
    [SerializeField] private float riseAccel = 30f;      // how fast we accelerate upward
    [SerializeField] private float fallAccel = 20f; 
   
    [Header("Ceiling")]
    [SerializeField] private float maxY = 3.7f;   // highest the player's feet can reach     // how fast we accelerate downward

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.12f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;

    public static PlayerController Instance { get; private set; }
   private bool isGrounded;
    private bool flyHeld;
    private bool wasFlyHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Instance = this;
    }

    void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        HandleFlight();
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

        // Play the fly SFX once, on the moment flight begins (false -> true edge).
        if (flyHeld && !wasFlyHeld && AudioManager.Instance != null)
            AudioManager.Instance.PlayFly();
        wasFlyHeld = flyHeld;
    }
    void HandleFlight()
    {
        float targetVy;
        float accel;

        if (flyHeld)
        {
            targetVy = maxRiseSpeed;
            accel = riseAccel;
        }
        else if (isGrounded)
        {
            targetVy = 0f;
            accel = fallAccel;
        }
        else
        {
            targetVy = -maxFallSpeed;
            accel = fallAccel;
        }

        float vy = Mathf.MoveTowards(rb.linearVelocity.y, targetVy, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(0f, vy);

        // Ceiling clamp — don't let the player fly off the top of the screen.
        if (rb.position.y > maxY)
        {
            rb.position = new Vector2(rb.position.x, maxY);
            rb.linearVelocity = new Vector2(0f, 0f);
        }
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsFlying", flyHeld || !isGrounded);
        animator.SetFloat("Speed", isGrounded ? 1f : 0f);  // grounded = auto-run (Walk), airborne = Fly
    }
}