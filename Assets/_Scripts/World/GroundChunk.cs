using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GroundChunk : MonoBehaviour
{
    private SpriteRenderer sr;
    private float chunkWidth;
    private float halfWidth;
    private float wrapTriggerX;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        chunkWidth = sr.size.x;        // pulls width from the Tiled SpriteRenderer
        halfWidth = chunkWidth * 0.5f;
    }

    void Start()
    {
        // Compute the wrap trigger from the actual camera bounds at runtime.
        Camera cam = Camera.main;
        float cameraLeftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        wrapTriggerX = cameraLeftEdge - halfWidth;   // wrap when our right edge crosses camera left

        // Set initial sprite from the starter biome (in case Inspector sprite differs).
        RefreshSprite();
    }

    void Update()
    {
        if (WorldManager.Instance == null) return;

        transform.position += Vector3.left * WorldManager.Instance.Speed * Time.deltaTime;

        if (transform.position.x < wrapTriggerX)
        {
            // Teleport to the right of the partner chunk: jump exactly 2 chunk-widths.
            transform.position += new Vector3(chunkWidth * 2f, 0f, 0f);
            RefreshSprite();
        }
    }

    void RefreshSprite()
    {
        if (BiomeManager.Instance != null && BiomeManager.Instance.CurrentGroundSprite != null)
        {
            sr.sprite = BiomeManager.Instance.CurrentGroundSprite;
        }
    }
}