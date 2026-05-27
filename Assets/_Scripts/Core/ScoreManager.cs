using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("Meters counted per world unit traveled. Raise it for bigger numbers.")]
    [SerializeField] private float metersPerUnit = 1f;

    private float distance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        float worldSpeed = (WorldManager.Instance != null ? WorldManager.Instance.Speed : 0f);
        distance += worldSpeed * Time.deltaTime;

        if (scoreText != null)
            scoreText.text = Mathf.FloorToInt(distance * metersPerUnit) + " m";
    }

    public int GetMeters() => Mathf.FloorToInt(distance * metersPerUnit);
}