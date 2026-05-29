using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Best Score")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("How To Play")]
    [SerializeField] private GameObject howToPlayPanel;

    void Start()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText != null)
            bestScoreText.text = "BEST: " + best + " m";

        // Always start with the overlay hidden, no matter how it was left in the editor.
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void OnStartRun() => SceneManager.LoadScene("Gameplay");

    public void OnHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void OnCloseHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void OnHighScore() => Debug.Log("High Score clicked");

    public void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}