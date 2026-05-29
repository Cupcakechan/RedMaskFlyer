using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public static BiomeManager Instance { get; private set; }

    public Sprite CurrentGroundSprite { get; private set; }

    [Header("Biomes")]
    [Tooltip("Always plays first. Same every run.")]
    [SerializeField] private BiomeData starterBiome;
    [Tooltip("Shuffled into a random order on run start, then looped indefinitely.")]
    [SerializeField] private BiomeData[] shuffledBiomes;

    [Header("Timing")]
    [Tooltip("Meters each biome lasts before swapping.")]
    [SerializeField] private float biomeLength = 500f;

    [Header("References")]
    [SerializeField] private HazardSpawner staticSpawner;

    private List<BiomeData> sequence;   // [starter, shuffled...]
    private int currentBiomeIndex;
    private float nextBoundary;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        BuildSequence();
        currentBiomeIndex = 0;
        nextBoundary = biomeLength;

        // Apply starter biome in Awake so it's ready before any HazardSpawner.Update runs.
        if (sequence != null && sequence.Count > 0)
        {
            ApplyBiome(sequence[0]);
        }
    }

    void Update()
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

    void BuildSequence()
    {
        sequence = new List<BiomeData>();
        if (starterBiome != null) sequence.Add(starterBiome);

        // Fisher–Yates shuffle the non-starter biomes
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
        if (next != null) ApplyBiome(next);
    }

    /// <summary>
    /// Index 0 is the starter; everything beyond loops the shuffled portion forever.
    /// </summary>
    BiomeData GetBiomeAt(int index)
    {
        if (sequence == null || sequence.Count == 0) return null;
        if (index == 0) return sequence[0];

        int shuffledCount = sequence.Count - 1;
        if (shuffledCount <= 0) return sequence[0];

        int wrapped = ((index - 1) % shuffledCount) + 1;
        return sequence[wrapped];
    }

    void ApplyBiome(BiomeData biome)
{
    if (biome == null || staticSpawner == null) return;
    staticSpawner.SetEntries(biome.entries);
    CurrentGroundSprite = biome.groundSprite;   // NEW
    Debug.Log($"[BiomeManager] Now entering: {biome.biomeName}");
}
}