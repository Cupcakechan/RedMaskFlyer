using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float despawnX = -12f;   // off the left edge

    void Update()
    {
        float speed = (WorldManager.Instance != null ? WorldManager.Instance.Speed : 5f);
        transform.position += Vector3.left * (speed * Time.deltaTime);

        if (transform.position.x <= despawnX)
            gameObject.SetActive(false);              // return to pool
    }
}