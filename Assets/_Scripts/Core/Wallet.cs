using UnityEngine;

public static class Wallet
{
    private const string Key = "Coins";

    public static int GetCoins() => PlayerPrefs.GetInt(Key, 0);

    public static void Add(int amount)
    {
        if (amount <= 0) return;
        PlayerPrefs.SetInt(Key, GetCoins() + amount);
        PlayerPrefs.Save();
    }

    public static bool TrySpend(int amount)
    {
        int coins = GetCoins();
        if (amount <= 0 || coins < amount) return false;
        PlayerPrefs.SetInt(Key, coins - amount);
        PlayerPrefs.Save();
        return true;
    }
}