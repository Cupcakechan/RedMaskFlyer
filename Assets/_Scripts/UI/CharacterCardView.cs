using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardView : MonoBehaviour
{
    public Image previewImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;   // "COST: 50" / "OWNED" / "SELECTED"
    public Button actionButton;
    public TextMeshProUGUI actionLabel;  // "BUY" / "SELECT" / "SELECTED"
}