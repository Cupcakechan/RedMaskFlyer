using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public static BiomeManager Instance { get; private set; }

    [Header("Biomes")]
    [SerializeField] private BiomeData starterBiome;
    [SerializeField] private BiomeData[] shuffledBiomes;

    [Header("Timing")]
    [SerializeField] private float biomeLength = 500f;

    [Header("References")]
    [SerializeField] private HazardSpawner staticSpawner;

    [Header("Sky Color")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float skyLerpDuration = 3f;

    public Sprite CurrentGroundSprite { get; private set; }
    public Sprite CurrentFarSprite { get; private set; }
    public Sprite CurrentNearSprite { get; private set; }

    private List<BiomeData> sequence;
    private int currentBiomeIndex;
    private float nextBoundary;

    private Color startSkyColor, targetSkyColor;
    private float skyLerpTimer;
    private bool lerpingSky;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (targetCamera == null) targetCamera = Camera.main;

        BuildSequence();
        currentBiomeIndex = 0;
        nextBoundary = biomeLength;

        if (sequence != null && sequence.Count > 0)
            ApplyBiome(sequence[0], instant: true);
    }

    void Update()
    {
        UpdateBiomeProgress();
        UpdateSkyLerp();
    }

    void UpdateBiomeProgress()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;
        if (ScoreManager.Instance == null) return;

        float meters = ScoreManager.Instance.GetMeters();
        if (meters >= nextBoundary)
        {
            AdvanceBiome();
            nextBoundary += biomeLength;
        }
    }

    void UpdateSkyLerp()
    {
        if (!lerpingSky || targetCamera == null) return;
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;

        skyLerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(skyLerpTimer / skyLerpDuration);
        targetCamera.backgroundColor = Color.Lerp(startSkyColor, targetSkyColor, t);
        if (t >= 1f) lerpingSky = false;
    }

    void BuildSequence()
    {
        sequence = new List<BiomeData>();
        if (starterBiome != null) sequence.Add(starterBiome);

        List<BiomeData> shuffled = new List<BiomeData>(shuffledBiomes);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int swap = Random.Range(0, i + 1);
            (shuffled[i], shuffled[swap]) = (shuffled[swap], shuffled[i]);
        }
        sequence.AddRange(shuffled);
    }

    void AdvanceBiome()
    {
        currentBiomeIndex++;
        BiomeData next = GetBiomeAt(currentBiomeIndex);
        if (next != null) ApplyBiome(next, instant: false);
    }

    BiomeData GetBiomeAt(int index)
    {
        if (sequence == null || sequence.Count == 0) return null;
        if (index == 0) return sequence[0];
        int shuffledCount = sequence.Count - 1;
        if (shuffledCount <= 0) return sequence[0];
        int wrapped = ((index - 1) % shuffledCount) + 1;
        return sequence[wrapped];
    }

    void ApplyBiome(BiomeData biome, bool instant)
    {
        if (biome == null) return;

        if (staticSpawner != null) staticSpawner.SetEntries(biome.entries);
        CurrentGroundSprite = biome.groundSprite;
        CurrentFarSprite = biome.parallaxFarSprite;
        CurrentNearSprite = biome.parallaxNearSprite;

        if (targetCamera != null)
        {
            if (instant)
            {
                targetCamera.backgroundColor = biome.skyColor;
                lerpingSky = false;
            }
            else
            {
                startSkyColor = targetCamera.backgroundColor;
                targetSkyColor = biome.skyColor;
                skyLerpTimer = 0f;
                lerpingSky = true;
            }
        }

        Debug.Log($"[BiomeManager] Now entering: {biome.biomeName}");
    }
}