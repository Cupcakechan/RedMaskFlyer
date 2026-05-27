using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;

    void Start()
    {
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText != null)
            bestScoreText.text = "BEST: " + best + " m";
    }

    public void OnStartRun() => SceneManager.LoadScene("Gameplay");

    public void OnHowToPlay() => Debug.Log("How To Play clicked");

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