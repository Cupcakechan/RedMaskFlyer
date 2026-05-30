using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;
    [SerializeField] private float despawnX = -12f;

    private bool collected;

    void OnEnable() => collected = false;   // reset when reused from the pool

    void Update()
    {
        float worldSpeed = (WorldManager.Instance != null ? WorldManager.Instance.Speed : 0f);
        if (worldSpeed <= 0f) return;   // world stopped (death) -> coins hold

        transform.position += Vector3.left * (worldSpeed * Time.deltaTime);
        if (transform.position.x <= despawnX) gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (other.GetComponentInParent<PlayerHealth>() == null) return;   // only the player collects

        collected = true;
        if (CoinManager.Instance != null) CoinManager.Instance.AddRunCoin(value);
        gameObject.SetActive(false);
    }
}