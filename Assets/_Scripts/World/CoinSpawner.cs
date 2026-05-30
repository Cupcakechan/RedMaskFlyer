using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool coinPool;

    [Header("Cadence (seconds)")]
    [SerializeField] private float minInterval = 1.2f;
    [SerializeField] private float maxInterval = 2.8f;

    [Header("Spawn band")]
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 3.5f;
    [SerializeField] private float spawnX = 11f;

    private float timer, nextSpawn;

    void Start() => ScheduleNext();

    void Update()
    {
        if (WorldManager.Instance == null || WorldManager.Instance.Speed <= 0f) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawn)
        {
            Spawn();
            timer = 0f;
            ScheduleNext();
        }
    }

    void ScheduleNext() => nextSpawn = Random.Range(minInterval, maxInterval);

    void Spawn()
    {
        if (coinPool == null) return;
        GameObject coin = coinPool.Get();
        if (coin == null) return;
        coin.transform.position = new Vector3(spawnX, Random.Range(minY, maxY), 0f);
    }
}