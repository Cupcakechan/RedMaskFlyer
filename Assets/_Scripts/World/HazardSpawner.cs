using System.Collections;
using UnityEngine;

public class HazardSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string name;
        public ObjectPool pool;
        [Min(0f)] public float weight = 1f;

        [Header("Spawn Y")]
        [Tooltip("Y position when this is a GROUND entry (isFlying = false).")]
        public float groundY = -4f;

        [Tooltip("If true: uses flyYMin/flyYMax for spawn Y and shows the '!' warning first.")]
        public bool isFlying = false;
        public float flyYMin = -1f;
        public float flyYMax = 3.5f;
        [Tooltip("Seconds the '!' shows before the flyer enters.")]
        [Min(0f)] public float warningDuration = 0.8f;

        [Header("Active Window (meters)")]
        [Tooltip("Distance at which this entry JOINS the mix. 0 = available from the start.")]
        [Min(0f)] public float unlockAtMeters = 0f;
        [Tooltip("Distance at which this entry LEAVES the mix. 0 = never locks.")]
        [Min(0f)] public float lockAtMeters = 0f;
    }

    [Header("Spawn Entries")]
    [SerializeField] private SpawnEntry[] entries;

    [Header("Timing")]
    [SerializeField] private float spawnX = 11f;
    [SerializeField] private float minInterval = 1.5f;
    [SerializeField] private float maxInterval = 3f;

    [Header("Flying Warning (only used if entries have isFlying = true)")]
    [SerializeField] private GameObject warningInstance;
    [SerializeField] private float warningX = 7.5f;

    private float timer;
    private float nextSpawn;

    void Start()
    {
        if (warningInstance != null) warningInstance.SetActive(false);
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

        if (entry.isFlying)
        {
            StartCoroutine(SpawnFlyingWithWarning(entry));
        }
        else
        {
            GameObject obj = entry.pool.Get();
            obj.transform.position = new Vector3(spawnX, entry.groundY, 0f);
        }
    }

    /// <summary>
/// Called by BiomeManager on biome transitions to swap the active hazard pool/entries.
/// </summary>
public void SetEntries(SpawnEntry[] newEntries)
{
    entries = newEntries;
}

    IEnumerator SpawnFlyingWithWarning(SpawnEntry entry)
    {
        float y = Random.Range(entry.flyYMin, entry.flyYMax);

        if (warningInstance != null)
        {
            warningInstance.transform.position = new Vector3(warningX, y, 0f);
            warningInstance.SetActive(true);
        }

        yield return new WaitForSeconds(entry.warningDuration);

        if (warningInstance != null) warningInstance.SetActive(false);

        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) yield break;

        GameObject obj = entry.pool.Get();
        obj.transform.position = new Vector3(spawnX, y, 0f);
    }

    SpawnEntry PickWeighted()
    {
        int meters = (ScoreManager.Instance != null) ? ScoreManager.Instance.GetMeters() : 0;

        float total = 0f;
        foreach (SpawnEntry e in entries)
        {
            if (meters < e.unlockAtMeters) continue;
            if (e.lockAtMeters > 0f && meters >= e.lockAtMeters) continue;
            total += e.weight;
        }
        if (total <= 0f) return null;

        float r = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (SpawnEntry e in entries)
        {
            if (meters < e.unlockAtMeters) continue;
            if (e.lockAtMeters > 0f && meters >= e.lockAtMeters) continue;
            cumulative += e.weight;
            if (r <= cumulative) return e;
        }
        return null;
    }
}