using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("Extra leftward speed ON TOP of world speed. 0 = planted (static hazard). >0 = walks toward the player (enemy).")]
    [SerializeField] private float extraSpeed = 0f;
    [SerializeField] private float despawnX = -12f;

    void Update()
    {
        float worldSpeed = (WorldManager.Instance != null ? WorldManager.Instance.Speed : 5f);

        // World frozen (e.g. on death) → everything stops, including walking enemies.
        if (worldSpeed <= 0f) return;

        float speed = worldSpeed + extraSpeed;
        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x <= despawnX)
            gameObject.SetActive(false);
    }
}