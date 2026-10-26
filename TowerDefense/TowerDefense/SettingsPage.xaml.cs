using TowerDefense.Services;
using TowerDefense.Themes;
using TowerDefense.ViewModels;

namespace TowerDefense;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm = new();

    // true, kui me ise täidame valijaid; siis ei tohi valiku muutumine seadeid üle kirjutada
    private bool _loading;

    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = _vm;

        FillLanguagePicker();
        FillThemePicker();
        ApplyTheme();
    }

    // ---------- Elutsükkel ----------

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LocalizationService.LanguageChanged += OnLanguageChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        LocalizationService.LanguageChanged -= OnLanguageChanged;
    }

    // Keel vahetus: uuendame tekstid ja teemade nimed uues keeles
    private void OnLanguageChanged()
    {
        _vm.RefreshTexts();
        FillThemePicker();
    }

    // ---------- Valijate täitmine ----------

    private void FillLanguagePicker()
    {
        _loading = true;

        LanguagePicker.ItemsSource = LocalizationService.Languages
            .Select(l => l.Name)
            .ToList();

        LanguagePicker.SelectedIndex = LocalizationService.Languages
            .ToList()
            .FindIndex(l => l.Code == LocalizationService.CurrentLanguage);

        _loading = false;
    }

    private void FillThemePicker()
    {
        _loading = true;

        ThemePicker.ItemsSource = Theme.All.Select(t => t.Name).ToList();
        ThemePicker.SelectedIndex = SavedThemeIndex();

        _loading = false;
    }

    private static int SavedThemeIndex() =>
        Math.Clamp(SettingsService.Current.ThemeIndex, 0, Theme.All.Count - 1);

    // ---------- UI sündmused ----------

    private void OnLanguagePickerChanged(object? sender, EventArgs e)
    {
        if (_loading || LanguagePicker.SelectedIndex < 0) return;

        string code = LocalizationService.Languages[LanguagePicker.SelectedIndex].Code;
        LocalizationService.SetLanguage(code);   // salvestab ja käivitab OnLanguageChanged
    }

    private void OnThemePickerChanged(object? sender, EventArgs e)
    {
        if (_loading || ThemePicker.SelectedIndex < 0) return;

        SettingsService.Current.ThemeIndex = ThemePicker.SelectedIndex;
        SettingsService.Save();
        ApplyTheme();
    }

    private void OnVolumeDragCompleted(object? sender, EventArgs e)
    {
        SettingsService.Save();
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // ---------- Abimeetodid ----------

    private void ApplyTheme()
    {
        Theme.All[SavedThemeIndex()].Apply(this);
    }
}