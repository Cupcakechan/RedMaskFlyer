using UnityEngine;

[System.Serializable]
public class Character
{
    public string id;             // unique key, e.g. "hero", "shadow"
    public string displayName;    // shown in the shop
    public int price;             // 0 for the default hero
    public Sprite previewSprite;  // shop preview image
    // (Sub-step 3 will add an in-game visual reference here.)
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Tooltip("Element 0 = default hero (price 0, owned from the start).")]
    [SerializeField] private Character[] characters;

    private const string OwnedPrefix = "CharOwned_";
    private const string SelectedKey = "SelectedChar";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // persist so Gameplay can read the selection
    }

    public int Count => (characters != null) ? characters.Length : 0;
    public Character Get(int index) => (index >= 0 && index < Count) ? characters[index] : null;

    public bool IsOwned(int index)
    {
        if (index == 0) return true;                      // default hero always owned
        Character c = Get(index);
        return c != null && PlayerPrefs.GetInt(OwnedPrefix + c.id, 0) == 1;
    }

    public bool TryBuy(int index)
    {
        Character c = Get(index);
        if (c == null || IsOwned(index)) return false;
        if (!Wallet.TrySpend(c.price)) return false;      // not enough coins

        PlayerPrefs.SetInt(OwnedPrefix + c.id, 1);
        PlayerPrefs.Save();
        return true;
    }

    public int SelectedIndex
    {
        get { int i = PlayerPrefs.GetInt(SelectedKey, 0); return (i >= 0 && i < Count) ? i : 0; }
    }

    public void Select(int index)
    {
        if (!IsOwned(index)) return;
        PlayerPrefs.SetInt(SelectedKey, index);
        PlayerPrefs.Save();
    }

    public Character Selected => Get(SelectedIndex);
}