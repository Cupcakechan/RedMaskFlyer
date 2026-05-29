using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float despawnX = 12f;

    private Vector2 direction = Vector2.right;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;

        // Flip sprite based on direction (assumes art is drawn facing RIGHT by default)
        Vector3 scale = transform.localScale;
        scale.x = (direction.x < 0f) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Update()
    {
        // Freeze guard: stop moving on death/pause-via-speed
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Mathf.Abs(transform.position.x) > despawnX)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Despawn on any collision (typically the player). PlayerHealth handles damage via its own Obstacle-tag check.
        gameObject.SetActive(false);
    }
}