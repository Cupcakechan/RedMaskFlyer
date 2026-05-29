using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private ObjectPool cloudPool;

    [Header("Cloud Sprite Sets")]
    [SerializeField] private Sprite[] whiteClouds;   // PS_Clouds
    [SerializeField] private Sprite[] spaceClouds;   // PS_Space_Clouds

    [Header("Spawn Cadence (seconds)")]
    [SerializeField] private float minInterval = 3.0f;
    [SerializeField] private float maxInterval = 5.5f;

    [Header("Vertical Band (world Y)")]
    [SerializeField] private float minY = 0.5f;
    [SerializeField] private float maxY = 4.5f;

    [Header("Scale")]
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 1.5f;

    [Header("Drift Speed (x WorldManager.Speed)")]
    [SerializeField] private float minSpeedMult = 0.18f;
    [SerializeField] private float maxSpeedMult = 0.30f;

    [Header("Placement")]
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float despawnX = -12f;
    [SerializeField] private int sortingOrder = -15;

    private float timer;

    void Start()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        // Only drip clouds while the world is actually moving (not dead, not paused).
        if (WorldManager.Instance == null || WorldManager.Instance.Speed <= 0f) return;

        // Skip spawning if this biome has clouds disabled. Existing clouds still drift out on their own.
        if (BiomeManager.Instance != null && !BiomeManager.Instance.CurrentSpawnsClouds) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnCloud();
            timer = Random.Range(minInterval, maxInterval);
        }
    }

    void SpawnCloud()
    {
        if (cloudPool == null) return;

        Sprite sprite = PickSprite();
        if (sprite == null) return;

        GameObject obj = cloudPool.Get();
        if (obj == null) return;

        CloudDrifter drifter = obj.GetComponent<CloudDrifter>();
        if (drifter == null) return;

        float y = Random.Range(minY, maxY);
        float scale = Random.Range(minScale, maxScale);
        float speedMult = Random.Range(minSpeedMult, maxSpeedMult);
        Vector3 pos = new Vector3(spawnX, y, 0f);

        drifter.Launch(sprite, pos, scale, speedMult, sortingOrder, despawnX);
    }

    Sprite PickSprite()
    {
        bool space = BiomeManager.Instance != null && BiomeManager.Instance.CurrentCloudsAreSpace;
        Sprite[] set = space ? spaceClouds : whiteClouds;

        // Fallback so we never try to spawn a null sprite.
        if (set == null || set.Length == 0) set = whiteClouds;
        if (set == null || set.Length == 0) return null;

        return set[Random.Range(0, set.Length)];
    }
}