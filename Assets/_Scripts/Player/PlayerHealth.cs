using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 5;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Death")]
    [SerializeField] private float deathFallGravity = 3f;
    [SerializeField] private float deathDelay = 2f;

    [Header("HUD")]
    [SerializeField] private Image heartImage;
    [Tooltip("Order: Element 0 = least full ... last Element = full heart.")]
    [SerializeField] private Sprite[] heartSprites;

    private int currentLives;
    private bool isInvincible;
    private bool isDead;
    private float invincibilityTimer;
    private float blinkTimer;

    private SpriteRenderer sr;
    private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentLives = maxLives;
        UpdateHeart();
    }

    void Update()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0f)
        {
            sr.enabled = !sr.enabled;
            blinkTimer = blinkInterval;
        }

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            sr.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
            TakeDamage();
    }

    public void TakeDamage()
    {
        if (isInvincible || isDead) return;

        currentLives--;
        if (currentLives <= 0)
        {
            Die();
            return;
        }

        UpdateHeart();
        animator.SetTrigger("Hurt");

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;
    }

    void UpdateHeart()
    {
        if (heartImage == null || heartSprites == null || heartSprites.Length == 0) return;
        int index = Mathf.Clamp(currentLives - 1, 0, heartSprites.Length - 1);
        heartImage.sprite = heartSprites[index];
    }

    void Die()
    {
        isDead = true;
        sr.enabled = true;
        animator.SetTrigger("Dead");

        if (WorldManager.Instance != null) WorldManager.Instance.Speed = 0f;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = deathFallGravity;

        // Record the run + update the best.
        int meters = (ScoreManager.Instance != null) ? ScoreManager.Instance.GetMeters() : 0;
        RunData.LastScore = meters;
        int best = PlayerPrefs.GetInt("BestScore", 0);
        RunData.IsNewBest = meters > best;
        if (RunData.IsNewBest)
        {
            PlayerPrefs.SetInt("BestScore", meters);
            PlayerPrefs.Save();
        }

        StartCoroutine(GoToGameOver());
    }

    IEnumerator GoToGameOver()
    {
        yield return new WaitForSeconds(deathDelay);
        SceneManager.LoadScene("GameOver");
    }
}