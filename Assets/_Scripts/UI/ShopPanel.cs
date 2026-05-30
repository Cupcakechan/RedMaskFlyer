using TMPro;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private CharacterCardView[] cards;   // order matches CharacterManager

    void OnEnable() => Refresh();

    public void Refresh()
    {
        if (coinsText != null) coinsText.text = Wallet.GetCoins().ToString();

        var cm = CharacterManager.Instance;
        if (cm == null) return;

        for (int i = 0; i < cards.Length; i++)
        {
            CharacterCardView card = cards[i];
            if (card == null) continue;

            Character c = cm.Get(i);
            if (c == null) { card.gameObject.SetActive(false); continue; }
            card.gameObject.SetActive(true);

            if (card.previewImage != null) card.previewImage.sprite = c.previewSprite;
            if (card.nameText != null) card.nameText.text = c.displayName;

            bool owned = cm.IsOwned(i);
            bool selected = (cm.SelectedIndex == i);
            bool canAfford = Wallet.GetCoins() >= c.price;

            if (card.statusText != null)
                card.statusText.text = !owned ? ("COST: " + c.price) : (selected ? "SELECTED" : "OWNED");

            if (card.actionLabel != null)
                card.actionLabel.text = !owned ? "BUY" : (selected ? "SELECTED" : "SELECT");

            if (card.actionButton != null)
            {
                card.actionButton.interactable = !owned ? canAfford : !selected;
                int index = i;   // capture for the listener
                card.actionButton.onClick.RemoveAllListeners();
                card.actionButton.onClick.AddListener(() => OnCardPressed(index));
            }
        }
    }

    void OnCardPressed(int index)
    {
        var cm = CharacterManager.Instance;
        if (cm == null) return;

        if (!cm.IsOwned(index))
        {
            if (cm.TryBuy(index)) cm.Select(index);   // auto-select on purchase
        }
        else
        {
            cm.Select(index);
        }
        Refresh();
    }
}