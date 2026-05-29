using UnityEngine;

public class ArcherEvent : MonoBehaviour
{
    [Header("Pools")]
    [SerializeField] private ObjectPool archerPool;
    [SerializeField] private ObjectPool arrowPool;    // NEW

    [Header("Timing")]
    [SerializeField] private float minInterval = 10f;
    [SerializeField] private float maxInterval = 18f;

    [Header("Spawn Position")]
    [SerializeField] private float spawnXLeft = -8f;
    [SerializeField] private float spawnXRight = 8f;
    [SerializeField] private float yMin = -2f;
    [SerializeField] private float yMax = 2f;

    private float timer;
    private float nextEvent;

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;

        timer += Time.deltaTime;
        if (timer >= nextEvent)
        {
            FireEvent();
            timer = 0f;
            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        nextEvent = Random.Range(minInterval, maxInterval);
    }

    void FireEvent()
    {
        if (archerPool == null) return;

        bool fromRight = Random.value > 0.5f;
        float spawnX = fromRight ? spawnXRight : spawnXLeft;
        float y = Random.Range(yMin, yMax);
        Vector2 facing = fromRight ? Vector2.left : Vector2.right;

        GameObject archer = archerPool.Get();
        archer.transform.position = new Vector3(spawnX, y, 0f);

        GoblinArcher archerScript = archer.GetComponent<GoblinArcher>();
        if (archerScript != null) archerScript.Initialize(facing, arrowPool);   // pass pool through
    }
}