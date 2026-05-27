using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string name;
        public ObjectPool pool;
        [Min(0f)] public float weight = 1f;
        public float groundY = -4f;

        [Tooltip("Distance (in meters) at which this entry joins the spawn mix. 0 = available from the start.")]
        [Min(0f)] public float unlockAtMeters = 0f;
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
        int meters = (ScoreManager.Instance != null) ? ScoreManager.Instance.GetMeters() : 0;

        // Sum the weights of currently-unlocked entries only.
        float total = 0f;
        foreach (SpawnEntry e in entries)
        {
            if (meters >= e.unlockAtMeters)
                total += e.weight;
        }
        if (total <= 0f) return null;

        float r = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (SpawnEntry e in entries)
        {
            if (meters < e.unlockAtMeters) continue;   // skip locked entries
            cumulative += e.weight;
            if (r <= cumulative) return e;
        }
        return null;
    }
}