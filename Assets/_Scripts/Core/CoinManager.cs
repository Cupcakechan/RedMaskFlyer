using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI coinText;

    public int RunCoins { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        RunCoins = 0;
        UpdateHUD();
    }

    public void AddRunCoin(int amount = 1)
    {
        RunCoins += amount;
        UpdateHUD();
    }

    // Called once on death: move this run's coins into the persistent wallet.
    public void Bank()
    {
        Wallet.Add(RunCoins);
        RunData.CoinsThisRun = RunCoins;
    }

    void UpdateHUD()
    {
        if (coinText != null) coinText.text = RunCoins.ToString();
    }
}