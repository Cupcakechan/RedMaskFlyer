using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScrollingObject : MonoBehaviour
{
    [Tooltip("Multiplier on world speed. 1 = ground speed; smaller = slower (distant parallax).")]
    [SerializeField] private float speedMultiplier = 1f;

    [Tooltip("Width of one tile repeat in WORLD UNITS. The object snaps back by this to stay seamless. A 16px tile at 16 PPU = 1.")]
    [SerializeField] private float tileUnit = 1f;

    private float startX;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        float speed = (WorldManager.Instance != null ? WorldManager.Instance.Speed : 5f) * speedMultiplier;
        transform.position += Vector3.left * (speed * Time.deltaTime);

        // Seamless loop: once we've scrolled one tile-unit left, snap back by one tile-unit.
        if (transform.position.x <= startX - tileUnit)
        {
            float overshoot = (startX - tileUnit) - transform.position.x;
            transform.position = new Vector3(startX - overshoot, transform.position.y, transform.position.z);
        }
    }
}