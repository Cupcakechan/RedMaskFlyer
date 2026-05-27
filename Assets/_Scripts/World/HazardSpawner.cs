using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string name;                   // Inspector label, e.g. "Static Hazard"
        public ObjectPool pool;               // pool for this hazard type
        [Min(0f)] public float weight = 1f;   // relative spawn frequency
        public float groundY = -4f;           // spawn height for this type
    }

    [Header("Spawn Entries")]
    [SerializeField] private SpawnEntry[] entries;

    [Header("Timing")]
    [SerializeField] private float spawnX = 11f;
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 3f;

    private float timer;
    private float nextSpawn;

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        // Don't spawn while the world is frozen (e.g. on death).
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawn)
        {
            Spawn();
            timer = 0f;
            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        nextSpawn = Random.Range(minInterval, maxInterval);
    }

    void Spawn()
    {
        SpawnEntry entry = PickWeighted();
        if (entry == null || entry.pool == null) return;

        GameObject obj = entry.pool.Get();
        obj.transform.position = new Vector3(spawnX, entry.groundY, 0f);
    }

    SpawnEntry PickWeighted()
    {
        float total = 0f;
        foreach (SpawnEntry e in entries) total += e.weight;
        if (total <= 0f) return null;

        float r = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (SpawnEntry e in entries)
        {
            cumulative += e.weight;
            if (r <= cumulative) return e;
        }
        return entries[entries.Length - 1];
    }
}