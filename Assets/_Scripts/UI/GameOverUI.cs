using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Run Result")]
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI bestText;

    [Header("High Score Entry")]
    [SerializeField] private GameObject entryGroup;          // prompt + input + submit button
    [SerializeField] private TMP_InputField initialsInput;
    [SerializeField] private TextMeshProUGUI resultText;     // shown after a qualifying submit

    [SerializeField] private TextMeshProUGUI coinsText;
    private int lastScore;
    private bool submitted;

    void Start()
    {
        lastScore = RunData.LastScore;

        if (coinsText != null) coinsText.text = "COINS +" + RunData.CoinsThisRun;

        if (distanceText != null) distanceText.text = "DISTANCE: " + lastScore + " m";
        RefreshBest();

        if (resultText != null) resultText.gameObject.SetActive(false);

        if (initialsInput != null)
        {
            initialsInput.characterLimit = HighScores.InitialsLength;
            initialsInput.onValueChanged.AddListener(ForceUpper);
        }

        if (HighScores.Qualifies(lastScore))
            ShowEntry();
        else if (entryGroup != null)
            entryGroup.SetActive(false);
    }

    void ShowEntry()
    {
        if (entryGroup != null) entryGroup.SetActive(true);

        if (initialsInput != null)
        {
            initialsInput.text = "";
            initialsInput.Select();
            initialsInput.ActivateInputField();   // auto-focus so the player can type right away
        }
    }

    // Wire this to the SUBMIT button's OnClick.
    public void OnSubmitInitials()
    {
        if (submitted) return;          // guard against a double submit
        submitted = true;

        string initials = (initialsInput != null) ? initialsInput.text : "";
        int rank = HighScores.Insert(initials, lastScore);

        if (entryGroup != null) entryGroup.SetActive(false);
        RefreshBest();

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = (rank >= 0)
                ? $"NEW HIGH SCORE — RANK #{rank + 1}!"
                : "NEW HIGH SCORE!";
        }
    }

    void RefreshBest()
    {
        if (bestText != null)
            bestText.text = "BEST: " + PlayerPrefs.GetInt("BestScore", 0) + " m";
    }

    void ForceUpper(string s)
    {
        if (initialsInput == null) return;
        string up = s.ToUpperInvariant();
        if (up != s) initialsInput.SetTextWithoutNotify(up);   // live-uppercase, no recursion
    }

    public void OnRetry()    => SceneManager.LoadScene("Gameplay");
    public void OnMainMenu() => SceneManager.LoadScene("MainMenu");
}