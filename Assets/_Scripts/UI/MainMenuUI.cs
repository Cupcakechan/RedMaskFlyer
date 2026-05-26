using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartRun()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnHowToPlay()
    {
        // Placeholder — we'll build this panel later
        Debug.Log("How To Play clicked");
    }

    public void OnHighScore()
    {
        // Placeholder — we'll build this panel later
        Debug.Log("High Score clicked");
    }

    public void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}