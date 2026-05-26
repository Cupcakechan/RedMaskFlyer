using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Player hit an obstacle!");
            // The Lives system will hook in here next feature.
        }
    }
}