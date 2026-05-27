using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private int maxLives = 4;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("HUD")]
    [SerializeField] private Image heartImage;
    [Tooltip("Order: Element 0 = 1-chunk (almost empty) ... last Element = full heart.")]
    [SerializeField] private Sprite[] heartSprites;

    private int currentLives;
    private bool isInvincible;
    private float invincibilityTimer;
    private float blinkTimer;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
            sr.enabled = !sr.enabled;       // blink while invincible
            blinkTimer = blinkInterval;
        }

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            sr.enabled = true;              // always visible when it ends
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
            TakeDamage();
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        currentLives--;
        Debug.Log("Hit! Lives remaining: " + currentLives);

        if (currentLives <= 0)
        {
            Die();                          // empty heart frame never shows
            return;
        }

        UpdateHeart();

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
        Debug.Log("Player died — restarting run.");
        SceneManager.LoadScene("Gameplay");   // swapped to GameOver scene later
    }
}