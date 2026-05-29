using TMPro;
using UnityEngine;

public class HighScorePanel : MonoBehaviour
{
    [Tooltip("Left text of each row (rank + initials). Order: top to bottom, length 5.")]
    [SerializeField] private TextMeshProUGUI[] labelTexts;

    [Tooltip("Right text of each row (score). Order: top to bottom, length 5.")]
    [SerializeField] private TextMeshProUGUI[] scoreTexts;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        var entries = HighScores.GetEntries();
        int rows = Mathf.Min(labelTexts.Length, scoreTexts.Length);

        for (int i = 0; i < rows; i++)
        {
            if (i < entries.Count)
            {
                if (labelTexts[i] != null) labelTexts[i].text = $"{i + 1}. {entries[i].initials}";
                if (scoreTexts[i] != null) scoreTexts[i].text = $"{entries[i].score} m";
            }
            else
            {
                if (labelTexts[i] != null) labelTexts[i].text = $"{i + 1}. ---";
                if (scoreTexts[i] != null) scoreTexts[i].text = "";
            }
        }
    }
}