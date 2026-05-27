using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WarningBlink : MonoBehaviour
{
    [Tooltip("Time between on/off toggles. 0.12 = roughly 4Hz blink.")]
    [SerializeField] private float blinkInterval = 0.12f;

    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (sr != null) sr.enabled = true;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            sr.enabled = !sr.enabled;
        }
    }

    void OnDisable()
    {
        // Reset to visible state so next activation always starts visible.
        if (sr != null) sr.enabled = true;
    }
}