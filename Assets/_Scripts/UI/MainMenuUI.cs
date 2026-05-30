using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Best Score")]
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Panels")]
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private GameObject shopPanel;

    void Start()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText != null) bestScoreText.text = "BEST: " + best + " m";

        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (highScorePanel != null) highScorePanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
    }

    public void OnStartRun() => SceneManager.LoadScene("Gameplay");

    public void OnHowToPlay()      { if (howToPlayPanel != null) howToPlayPanel.SetActive(true); }
    public void OnCloseHowToPlay() { if (howToPlayPanel != null) howToPlayPanel.SetActive(false); }

    public void OnHighScore()      { if (highScorePanel != null) highScorePanel.SetActive(true); }
    public void OnCloseHighScore() { if (highScorePanel != null) highScorePanel.SetActive(false); }

    public void OnShop()           { if (shopPanel != null) shopPanel.SetActive(true); }
    public void OnCloseShop()      { if (shopPanel != null) shopPanel.SetActive(false); }

    public void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}