using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private bool isPaused;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;   // safety: ensure scene starts unfrozen
        isPaused = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;   // critical: unfreeze before reloading
        SceneManager.LoadScene("Gameplay");
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;   // critical: unfreeze before scene swap
        SceneManager.LoadScene("MainMenu");
    }
}