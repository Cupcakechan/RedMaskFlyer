using UnityEngine;

[System.Serializable]
public class BiomeData
{
    public string biomeName;

    [Tooltip("Tick this for space/cosmic biomes so loose clouds use the purple PS_Space_Clouds set instead of white.")]
    public bool useSpaceClouds = false;
    
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