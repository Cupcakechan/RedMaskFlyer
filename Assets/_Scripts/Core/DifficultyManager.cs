using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn-rate ramp (stepped)")]
    [Tooltip("Stays at base (1x) until this distance, then the first step kicks in.")]
    [SerializeField] private float startMeters = 1000f;
    [Tooltip("Each further step happens after this many additional meters.")]
    [SerializeField] private float metersPerStep = 500f;
    [Tooltip("How much the spawn-rate multiplier grows per step.")]
    [SerializeField] private float rateStep = 0.25f;
    [Tooltip("Hard cap on the spawn-rate multiplier.")]
    [SerializeField] private float maxMultiplier = 2.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Current spawn-rate multiplier (>= 1). Spawners divide their interval by this,
    /// so higher = shorter gaps = more hazards.
    /// </summary>
    public float SpawnRateMultiplier()
    {
        int meters = (ScoreManager.Instance != null) ? ScoreManager.Instance.GetMeters() : 0;
        if (meters < startMeters) return 1f;

        int steps = Mathf.FloorToInt((meters - startMeters) / Mathf.Max(1f, metersPerStep)) + 1;
        float mult = 1f + steps * rateStep;
        return Mathf.Clamp(mult, 1f, maxMultiplier);
    }

#if UNITY_EDITOR
    [Header("Debug (read-only while playing)")]
    [SerializeField] private float currentMultiplier = 1f;
    void Update() { currentMultiplier = SpawnRateMultiplier(); }
#endif
}