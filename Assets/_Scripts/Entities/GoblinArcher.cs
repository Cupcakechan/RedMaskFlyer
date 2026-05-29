using System.Collections;
using UnityEngine;

public class GoblinArcher : MonoBehaviour
{
    [Header("Volley Timing")]
    [SerializeField] private float pauseDuration = 0.9f;
    [SerializeField] private int arrowCount = 3;
    [SerializeField] private float arrowSpacing = 0.5f;
    [SerializeField] private float despawnDelay = 0.8f;

    [Header("Arrow Spawn")]
    [SerializeField] private Vector2 arrowSpawnOffset = new Vector2(0.4f, 0f);

    private ObjectPool arrowPool;
    private Vector2 facingDirection = Vector2.left;

    public void Initialize(Vector2 direction, ObjectPool pool)
    {
        facingDirection = direction.normalized;
        arrowPool = pool;

        Vector3 scale = transform.localScale;
        scale.x = (facingDirection.x < 0f) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;

        StopAllCoroutines();
        StartCoroutine(VolleySequence());
    }

    IEnumerator VolleySequence()
    {
        yield return new WaitForSeconds(pauseDuration);

        for (int i = 0; i < arrowCount; i++)
        {
            FireArrow();
            if (i < arrowCount - 1) yield return new WaitForSeconds(arrowSpacing);
        }

        yield return new WaitForSeconds(despawnDelay);
        gameObject.SetActive(false);
    }

    void FireArrow()
    {
        if (arrowPool == null) return;

        GameObject arrow = arrowPool.Get();

        // Spawn at the bow position (offset mirrored by facing)
        Vector3 spawnPos = transform.position;
        spawnPos.x += arrowSpawnOffset.x * facingDirection.x;
        spawnPos.y += arrowSpawnOffset.y;
        arrow.transform.position = spawnPos;

        // Snapshot-aim: point at the player's CURRENT position at this arrow's fire moment
        Vector2 aimDir = facingDirection;   // fallback if player missing
        if (PlayerController.Instance != null)
        {
            Vector2 toPlayer = (Vector2)PlayerController.Instance.transform.position - (Vector2)spawnPos;
            if (toPlayer.sqrMagnitude > 0.0001f) aimDir = toPlayer.normalized;
        }

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null) arrowScript.Initialize(aimDir);
    }
}