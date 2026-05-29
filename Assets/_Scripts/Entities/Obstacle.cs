using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Tooltip("Extra leftward speed ON TOP of world speed. 0 = planted (static hazard). >0 = walks toward the player (enemy).")]
    [SerializeField] private float extraSpeed = 0f;
    [SerializeField] private float despawnX = -12f;

    private bool primed;   // skip the pool's construction-time OnEnable

    void OnEnable()
    {
        // The pool instantiates each object active, then immediately disables it — that first
        // OnEnable is construction, not a real spawn, so we skip it. Every later activation
        // (i.e. an actual spawn from the pool) plays the noise.
        if (!primed) { primed = true; return; }

        // Walking enemies (extraSpeed > 0) play a random noise as they appear.
        if (extraSpeed > 0f && AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyNoise();
    }

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