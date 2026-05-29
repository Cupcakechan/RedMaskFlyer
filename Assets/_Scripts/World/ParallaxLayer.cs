using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    public enum ParallaxRole { Far, Near }

    [Tooltip("Which background-set sprite this layer pulls.")]
    [SerializeField] private ParallaxRole role = ParallaxRole.Far;

    [Tooltip("Fraction of world speed this layer scrolls at. Smaller = farther / slower.")]
    [SerializeField] private float speedMultiplier = 0.2f;

    private SpriteRenderer sr;
    private float chunkWidth;
    private float wrapTriggerX;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ApplyBiomeSprite();   // set the starter sprite first

        chunkWidth = sr.bounds.size.x;
        float halfWidth = chunkWidth * 0.5f;
        Camera cam = Camera.main;
        float cameraLeftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        wrapTriggerX = cameraLeftEdge - halfWidth;
    }

    void Update()
    {
        if (WorldManager.Instance != null)
        {
            float scrollSpeed = WorldManager.Instance.Speed * speedMultiplier;
            transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

            if (transform.position.x < wrapTriggerX)
                transform.position += new Vector3(chunkWidth * 2f, 0f, 0f);
        }

        // Swap to the current biome's sprite the instant it changes (synced with the sky),
        // instead of waiting for a wrap — that lag was causing the cross-set mixing.
        ApplyBiomeSprite();
    }

    void ApplyBiomeSprite()
    {
        if (BiomeManager.Instance == null) return;
        Sprite target = (role == ParallaxRole.Far)
            ? BiomeManager.Instance.CurrentFarSprite
            : BiomeManager.Instance.CurrentNearSprite;
        if (target != null && sr.sprite != target)
            sr.sprite = target;
    }
}