using UnityEngine;

[System.Serializable]
public class BiomeData
{
    public string biomeName;

    [Header("Hazards")]
    public HazardSpawner.SpawnEntry[] entries;

    [Header("Ground")]
    public Sprite groundSprite;

    [Header("Background / Parallax")]
    [Tooltip("Camera background color (the sky) for this biome.")]
    public Color skyColor = Color.black;
    [Tooltip("Far (slow) parallax cloud sprite.")]
    public Sprite parallaxFarSprite;
    [Tooltip("Near (faster) parallax cloud sprite.")]
    public Sprite parallaxNearSprite;
}