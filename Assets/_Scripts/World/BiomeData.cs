using UnityEngine;

[System.Serializable]
public class BiomeData
{
    [Tooltip("Display name (used for debug logs and future transition UI).")]
    public string biomeName;

    [Tooltip("Static hazards active while this biome plays. Walker/flyer enemies are biome-agnostic.")]
    public HazardSpawner.SpawnEntry[] entries;

    [Tooltip("The tile sprite the scrolling ground uses while this biome is active.")]
    public Sprite groundSprite;
}