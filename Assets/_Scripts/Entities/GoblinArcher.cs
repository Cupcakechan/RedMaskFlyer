using System.Collections;
using UnityEngine;

public class GoblinArcher : MonoBehaviour
{
    [Header("Volley Timing")]
    [SerializeField] private float pauseDuration = 0.9f;
    [SerializeField] private int arrowCount = 3;
    [SerializeField] private float arrowSpacing = 0.25f;
    [SerializeField] private float despawnDelay = 0.6f;

    [Header("Arrow Spawn")]
    [Tooltip("Local offset from archer center where arrows spawn (X is mirrored when facing left).")]
    [SerializeField] private Vector2 arrowSpawnOffset = new Vector2(0.4f, 0f);

    // Injected at runtime by ArcherEvent — NOT serialized (prefabs can't hold scene refs).
    private ObjectPool arrowPool;
    private Vector2 facingDirection = Vector2.left;

    /// <summary>Called by ArcherEvent when spawning. Provides direction AND the arrow pool to fire from.</summary>
    public void Initialize(Vector2 direction, ObjectPool pool)
    {
        facingDirection = direction.normalized;
        arrowPool = pool;

        // Flip sprite (assumes goblin art is drawn facing RIGHT by default)
        Vector3 scale = transform.localScale;
        scale.x = (facingDirection.x < 0f) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }


    void OnEnable()
    {
        StartCoroutine(VolleySequence());
    }

    IEnumerator VolleySequence()
    {
        // Telegraph pause — the archer appears, holds, player sees the threat
        yield return new WaitForSeconds(pauseDuration);

        // Fire the volley
        for (int i = 0; i < arrowCount; i++)
        {
            FireArrow();
            if (i < arrowCount - 1) yield return new WaitForSeconds(arrowSpacing);
        }

        // Linger briefly, then despawn
        yield return new WaitForSeconds(despawnDelay);
        gameObject.SetActive(false);
    }

    void FireArrow()
    {
        if (arrowPool == null) return;

        GameObject arrow = arrowPool.Get();

        // Position arrow at the archer's bow (offset mirrored by facing)
        Vector3 spawnPos = transform.position;
        spawnPos.x += arrowSpawnOffset.x * facingDirection.x;
        spawnPos.y += arrowSpawnOffset.y;
        arrow.transform.position = spawnPos;

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null) arrowScript.Initialize(facingDirection);
    }
}