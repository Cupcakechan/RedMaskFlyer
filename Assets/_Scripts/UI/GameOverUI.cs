using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI bestText;
    [SerializeField] private GameObject newBestLabel;

    void Start()
    {
        int last = RunData.LastScore;
        int best = PlayerPrefs.GetInt("BestScore", 0);

        if (distanceText != null) distanceText.text = "DISTANCE: " + last + " m";
        if (bestText != null) bestText.text = "BEST: " + best + " m";
        if (newBestLabel != null) newBestLabel.SetActive(RunData.IsNewBest);
    }

    public void OnRetry()    => SceneManager.LoadScene("Gameplay");
    public void OnMainMenu() => SceneManager.LoadScene("MainMenu");
}