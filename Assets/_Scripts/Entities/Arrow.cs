using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float lifetime = 4f;
    [Tooltip("Set to 180 if your arrow art points LEFT by default instead of right.")]
    [SerializeField] private float spriteAngleOffset = 0f;

    private Vector2 direction = Vector2.right;
    private float age;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        age = 0f;

        // Rotate the sprite to point along travel direction (assumes art points RIGHT at 0°)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        if (WorldManager.Instance != null && WorldManager.Instance.Speed <= 0f) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        age += Time.deltaTime;
        if (age >= lifetime) gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Despawn ONLY when hitting the player. Ignore the archer's own collider and other enemies
        // so upward shots can pass through the archer's body without self-despawning.
        if (other.GetComponentInParent<PlayerHealth>() != null)
        {
            gameObject.SetActive(false);
        }
    }
}