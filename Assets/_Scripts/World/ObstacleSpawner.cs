using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool pool;
    [SerializeField] private float spawnX = 11f;       // off the right edge
    [SerializeField] private float groundY = -3.3f;    // hazard rest height on the grass
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
        GameObject obstacle = pool.Get();
        obstacle.transform.position = new Vector3(spawnX, groundY, 0f);
    }
}