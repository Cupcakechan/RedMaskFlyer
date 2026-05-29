using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class HighScoreEntry
{
    public string initials;
    public int score;

    public HighScoreEntry(string initials, int score)
    {
        this.initials = initials;
        this.score = score;
    }
}

[Serializable]
public class HighScoreTable
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

/// <summary>
/// Static high-score store: Top 5, 3-letter initials, saved as JSON in PlayerPrefs.
/// Keeps the legacy "BestScore" key in sync with the #1 entry so existing UI still works.
/// </summary>
public static class HighScores
{
    public const int MaxEntries = 5;
    public const int InitialsLength = 3;

    private const string PrefsKey = "HighScores";
    private const string BestScoreKey = "BestScore";

    private static readonly HighScoreEntry[] DefaultSeed =
    {
        new HighScoreEntry("RMF", 500),
        new HighScoreEntry("HRO", 400),
        new HighScoreEntry("FLY", 300),
        new HighScoreEntry("SKY", 200),
        new HighScoreEntry("AAA", 100),
    };

    public static HighScoreTable Load()
    {
        // First ever run: build the seeded table, folding in any pre-existing BestScore.
        if (!PlayerPrefs.HasKey(PrefsKey))
        {
            HighScoreTable seeded = new HighScoreTable();
            foreach (HighScoreEntry e in DefaultSeed)
                seeded.entries.Add(new HighScoreEntry(e.initials, e.score));

            int existingBest = PlayerPrefs.GetInt(BestScoreKey, 0);
            if (existingBest > 0)
                seeded.entries.Add(new HighScoreEntry("YOU", existingBest));

            Save(seeded);   // sorts, trims to 5, writes JSON + BestScore
            return seeded;
        }

        string json = PlayerPrefs.GetString(PrefsKey);
        HighScoreTable table = JsonUtility.FromJson<HighScoreTable>(json);
        if (table == null || table.entries == null)
            table = new HighScoreTable();

        SortDescending(table);
        Trim(table);
        return table;
    }

    public static void Save(HighScoreTable table)
    {
        if (table == null) return;

        SortDescending(table);
        Trim(table);

        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(table));
        PlayerPrefs.SetInt(BestScoreKey, table.entries.Count > 0 ? table.entries[0].score : 0);
        PlayerPrefs.Save();
    }

    public static List<HighScoreEntry> GetEntries() => Load().entries;

    /// <summary>True if this score would make the top 5.</summary>
    public static bool Qualifies(int score)
    {
        HighScoreTable table = Load();
        if (table.entries.Count < MaxEntries) return true;
        return score > table.entries[table.entries.Count - 1].score;
    }

    /// <summary>
    /// Insert a run. Returns the 0-based rank it landed at (0 = top),
    /// or -1 if it did not make the cut.
    /// </summary>
    public static int Insert(string initials, int score)
    {
        HighScoreTable table = Load();

        HighScoreEntry entry = new HighScoreEntry(SanitizeInitials(initials), score);
        table.entries.Add(entry);
        SortDescending(table);

        int rank = table.entries.IndexOf(entry);   // exact, by reference
        Trim(table);
        Save(table);

        return (rank >= 0 && rank < MaxEntries) ? rank : -1;
    }

    /// <summary>Uppercase, letters/digits only, padded/clamped to InitialsLength.</summary>
    public static string SanitizeInitials(string raw)
    {
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(raw))
        {
            foreach (char c in raw.ToUpperInvariant())
                if (char.IsLetterOrDigit(c)) sb.Append(c);
        }

        string clean = sb.ToString();
        if (clean.Length > InitialsLength) clean = clean.Substring(0, InitialsLength);
        while (clean.Length < InitialsLength) clean += "A";
        return clean;
    }

    private static void SortDescending(HighScoreTable table)
    {
        // OrderByDescending is stable: tied scores keep insertion order (a new tie sits below equals).
        table.entries = table.entries.OrderByDescending(e => e.score).ToList();
    }

    private static void Trim(HighScoreTable table)
    {
        if (table.entries.Count > MaxEntries)
            table.entries.RemoveRange(MaxEntries, table.entries.Count - MaxEntries);
    }

#if UNITY_EDITOR
    public static void ClearAndReseed()
    {
        PlayerPrefs.DeleteKey(PrefsKey);
        PlayerPrefs.DeleteKey(BestScoreKey);
        Load();   // re-seeds + saves
    }
#endif
}