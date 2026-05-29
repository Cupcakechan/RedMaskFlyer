#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class HighScoreDebug
{
    [MenuItem("Red Mask Flyer/High Scores/Log Table")]
    public static void LogTable()
    {
        var entries = HighScores.GetEntries();
        string s = "[HighScores] Current table:\n";
        for (int i = 0; i < entries.Count; i++)
            s += $"  {i + 1}. {entries[i].initials}  {entries[i].score} m\n";
        Debug.Log(s);
    }

    [MenuItem("Red Mask Flyer/High Scores/Test Insert (TST 350)")]
    public static void TestInsert()
    {
        int rank = HighScores.Insert("TST", 350);
        Debug.Log(rank >= 0
            ? $"[HighScores] Inserted TST 350 at rank {rank + 1}."
            : "[HighScores] TST 350 did not make the top 5.");
        LogTable();
    }

    [MenuItem("Red Mask Flyer/High Scores/Clear and Reseed")]
    public static void ClearAndReseed()
    {
        HighScores.ClearAndReseed();
        Debug.Log("[HighScores] Cleared and reseeded to defaults.");
        LogTable();
    }
}
#endif