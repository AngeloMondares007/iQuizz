using UnityEngine;
using System.Collections.Generic;  // Add this line for List<>
using UnityEngine.UI; // For using the UI Text component
using TMPro; // Use this if you're using TextMeshPro

public class MainMenuLeaderboard : MonoBehaviour
{
    public Text leaderboardText;  // Use this for regular Text
    // public TMP_Text leaderboardText; // Use this for TextMeshPro

    void Start()
    {
        // Update the leaderboard when the game starts
        UpdateLeaderboard();
    }

    // Method to update and display the leaderboard
    void UpdateLeaderboard()
    {
        // Load leaderboard entries
        List<LeaderboardEntry> leaderboard = LoadLeaderboard();

        // If no leaderboard data exists, populate with default entries
        if (leaderboard.Count == 0)
        {
            leaderboard = GetDefaultLeaderboard();
            // Save default leaderboard data so that it won't be reset every time
            SaveLeaderboard(leaderboard);
        }

        // Initialize leaderboard text with headers and a separator
        leaderboardText.text = "Rank   Name         Score\n\n";

        // Loop through the leaderboard and format each entry
        for (int i = 0; i < leaderboard.Count; i++)
        {
            LeaderboardEntry entry = leaderboard[i];

            string rank = (i + 1).ToString().PadRight(5); // Rank
            string name = TruncateString(entry.Name, 10).PadRight(10); // Name truncated
            string score = entry.Score.ToString().PadLeft(6); // Right-aligned Score

            leaderboardText.text += $"{rank}{name}{score}\n";
        }
    }

    // Helper function to create default leaderboard entries
    List<LeaderboardEntry> GetDefaultLeaderboard()
    {
        List<LeaderboardEntry> defaultLeaderboard = new List<LeaderboardEntry>();

        // Create 10 default leaderboard entries
        for (int i = 0; i < 10; i++)
        {
            defaultLeaderboard.Add(new LeaderboardEntry { Name = "Name", Score = 0 });
        }

        return defaultLeaderboard;
    }

    // Helper function to save leaderboard data
    void SaveLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        string json = JsonUtility.ToJson(new LeaderboardWrapper { Entries = leaderboard });
        PlayerPrefs.SetString("Leaderboard", json);
        PlayerPrefs.Save();
    }

    // Helper function to load leaderboard data
    List<LeaderboardEntry> LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey("Leaderboard"))
        {
            string json = PlayerPrefs.GetString("Leaderboard");
            return JsonUtility.FromJson<LeaderboardWrapper>(json).Entries;
        }
        return new List<LeaderboardEntry>(); // Return empty list if no data found
    }

    // Helper function to truncate the string to a maximum length
    string TruncateString(string input, int maxLength)
    {
        if (input.Length > maxLength)
        {
            return input.Substring(0, maxLength) + "..."; // Truncate and append "..."
        }
        return input; // Return the string as is if it's short enough
    }
}

// Classes for leaderboard data
[System.Serializable]
public class LeaderboardEntry
{
    public string Name;
    public int Score;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public List<LeaderboardEntry> Entries;
}
