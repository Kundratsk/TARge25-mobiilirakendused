using System.Text.Json;
using TowerDefense.Models;

namespace TowerDefense.Services;

// Edetabel: 10 parimat tulemust, salvestatud eraldi JSON-faili
public static class LeaderboardService
{
    private const int MaxEntries = 10;

    private static readonly string FilePath =
        Path.Combine(FileSystem.AppDataDirectory, "leaderboard.json");

    private static List<ScoreEntry>? _entries;

    // ??= tähendab: kui _entries on tühi, lae see failist
    public static IReadOnlyList<ScoreEntry> Entries => _entries ??= Load();

    private static List<ScoreEntry> Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
            }
        }
        catch
        {
            // Kui fail on katki, alustame tühja edetabeliga
        }

        return new List<ScoreEntry>();
    }

    // Lisab tulemuse. Tagastab koha edetabelis (1 = parim) või 0, kui tulemus top 10-sse ei mahtunud.
    public static int Add(ScoreEntry entry)
    {
        List<ScoreEntry> list = _entries ??= Load();
        list.Add(entry);

        // Järjestus: rohkem punkte, siis rohkem laineid, siis kiirem aeg
        List<ScoreEntry> sorted = list
            .OrderByDescending(e => e.Score)
            .ThenByDescending(e => e.Wave)
            .ThenBy(e => e.Seconds)
            .ToList();

        int rank = sorted.IndexOf(entry) + 1;

        list.Clear();
        list.AddRange(sorted.Take(MaxEntries));

        Save();

        return rank <= MaxEntries ? rank : 0;
    }

    // Kustutab ühe tulemuse
    public static void Remove(ScoreEntry entry)
    {
        List<ScoreEntry> list = _entries ??= Load();

        if (list.Remove(entry))
            Save();
    }

    // Kustutab kõik tulemused
    public static void Clear()
    {
        List<ScoreEntry> list = _entries ??= Load();
        list.Clear();
        Save();
    }

    private static void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(
                _entries,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Salvestamise viga ei tohi mängu kokku jooksutada
        }
    }
}