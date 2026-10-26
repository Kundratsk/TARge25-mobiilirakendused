using TowerDefense.Services;
using TowerDefense.Themes;
using TowerDefense.ViewModels;

namespace TowerDefense;

public partial class LeaderboardPage : ContentPage
{
    private readonly LeaderboardViewModel _vm = new();

    public LeaderboardPage()
    {
        InitializeComponent();
        BindingContext = _vm;

        // Kasutame salvestatud teemat
        int index = Math.Clamp(
            SettingsService.Current.ThemeIndex, 0, Theme.All.Count - 1);
        Theme.All[index].Apply(this);
    }

    // Ühe tulemuse kustutamine (nupu BindingContext on selle rea andmed)
    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: LeaderboardRow row })
            return;

        bool confirmed = await DisplayAlert(
            LocalizationService.Get("DeleteTitle"),
            LocalizationService.Get("DeleteEntryMessage", row.Name, row.Entry.Score),
            LocalizationService.Get("Yes"),
            LocalizationService.Get("No"));

        if (!confirmed) return;

        LeaderboardService.Remove(row.Entry);
        _vm.Reload();
    }

    // Kõigi tulemuste kustutamine; nullib ka parima tulemuse
    private async void OnClearAllClicked(object? sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            LocalizationService.Get("ClearAll"),
            LocalizationService.Get("ClearAllMessage"),
            LocalizationService.Get("Yes"),
            LocalizationService.Get("No"));

        if (!confirmed) return;

        LeaderboardService.Clear();

        SettingsService.Current.BestScore = 0;
        SettingsService.Save();

        _vm.Reload();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}