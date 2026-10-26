using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TowerDefense.Models;
using TowerDefense.Services;

namespace TowerDefense.ViewModels;

// Üks rida edetabelis, valmis kuvamiseks
public class LeaderboardRow
{
    public ScoreEntry Entry { get; init; } = null!;   // algne tulemus (kustutamiseks)
    public string RankText { get; init; } = "";
    public string Name { get; init; } = "";
    public string ScoreText { get; init; } = "";
    public string WaveText { get; init; } = "";
    public string DetailText { get; init; } = "";
}

public partial class LeaderboardViewModel : ObservableObject
{
    [ObservableProperty] private string _titleText = "";
    [ObservableProperty] private string _backText = "";
    [ObservableProperty] private string _emptyText = "";
    [ObservableProperty] private string _clearAllText = "";

    // ObservableCollection annab ekraanile muutusest ise teada
    public ObservableCollection<LeaderboardRow> Rows { get; } = new();

    public bool HasRows => Rows.Count > 0;
    public bool IsEmpty => Rows.Count == 0;

    public LeaderboardViewModel()
    {
        TitleText = "🏆 " + LocalizationService.Get("LeaderboardTitle");
        BackText = LocalizationService.Get("Back");
        EmptyText = LocalizationService.Get("LeaderboardEmpty");
        ClearAllText = "🗑 " + LocalizationService.Get("ClearAll");

        Reload();
    }

    // Ehitab read uuesti edetabeli failist (kutsutakse ka pärast kustutamist)
    public void Reload()
    {
        Rows.Clear();

        // LeaderboardService hoiab tulemused juba õiges järjekorras
        int place = 1;
        foreach (ScoreEntry entry in LeaderboardService.Entries)
        {
            string wave = LocalizationService.Get("LeaderboardWave", entry.Wave, Game.TotalWaves);
            if (entry.Won) wave += " 🎉";

            Rows.Add(new LeaderboardRow
            {
                Entry = entry,
                RankText = place switch
                {
                    1 => "🥇",
                    2 => "🥈",
                    3 => "🥉",
                    _ => $"#{place}"
                },
                Name = entry.Name,
                ScoreText = $"⭐ {entry.Score}",
                WaveText = wave,
                DetailText = $"{entry.Seconds:F0} s · {entry.Date:dd.MM.yyyy}"
            });

            place++;
        }

        OnPropertyChanged(nameof(HasRows));
        OnPropertyChanged(nameof(IsEmpty));
    }
}