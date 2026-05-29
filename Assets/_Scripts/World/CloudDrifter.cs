using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CloudDrifter : MonoBehaviour
{
    private SpriteRenderer sr;
    private float speedMultiplier;   // fraction of WorldManager.Speed
    private float despawnX;
    private bool active;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>Configure and launch this cloud. Called by the spawner each time it's pulled from the pool.</summary>
    public void Launch(Sprite sprite, Vector3 position, float scale,
                       float speedMultiplier, int sortingOrder, float despawnX)
    {
        this.speedMultiplier = speedMultiplier;
        this.despawnX = despawnX;

        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
        transform.position = position;
        transform.localScale = new Vector3(scale, scale, 1f);

        active = true;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    void Update()
    {
        if (!active) return;

        // Hold position when the world stops (death) or is paused (timeScale 0 -> deltaTime 0).
        float worldSpeed = WorldManager.Instance != null ? WorldManager.Instance.Speed : 0f;
        if (worldSpeed <= 0f) return;

        transform.position += Vector3.left * (worldSpeed * speedMultiplier) * Time.deltaTime;

        if (transform.position.x <= despawnX)
        {
            active = false;
            gameObject.SetActive(false);   // recycle: the pool's Get() will reuse this inactive instance
        }
    }
}