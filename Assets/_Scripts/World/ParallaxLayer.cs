using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Fraction of world speed this layer scrolls at. Smaller = farther away / slower.")]
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
        chunkWidth = sr.bounds.size.x;          // actual rendered width (accounts for scale)
        float halfWidth = chunkWidth * 0.5f;

        Camera cam = Camera.main;
        float cameraLeftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;
        wrapTriggerX = cameraLeftEdge - halfWidth;   // wrap when our right edge clears the camera's left edge
    }

    void Update()
    {
        if (WorldManager.Instance == null) return;

        float scrollSpeed = WorldManager.Instance.Speed * speedMultiplier;
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x < wrapTriggerX)
        {
            transform.position += new Vector3(chunkWidth * 2f, 0f, 0f);
        }
    }
}