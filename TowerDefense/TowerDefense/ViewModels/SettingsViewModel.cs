using CommunityToolkit.Mvvm.ComponentModel;
using TowerDefense.Services;

namespace TowerDefense.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    // Tekstid tulevad ressursifailidest ja uuenevad keele vahetamisel
    [ObservableProperty] private string _titleText = "";
    [ObservableProperty] private string _languageLabel = "";
    [ObservableProperty] private string _themeLabel = "";
    [ObservableProperty] private string _soundLabel = "";
    [ObservableProperty] private string _volumeLabel = "";
    [ObservableProperty] private string _backText = "";

    // Kasutaja seaded; muutumisel kirjutatakse need seadetesse
    [ObservableProperty] private bool _soundEnabled;
    [ObservableProperty] private double _volume;

    public SettingsViewModel()
    {
        // Väljadele kirjutamine otse ei käivita "Changed" meetodeid, mis siin ongi hea
        _soundEnabled = SettingsService.Current.SoundEnabled;
        _volume = SettingsService.Current.Volume;
        RefreshTexts();
    }

    // Kutsutakse automaatselt, kui SoundEnabled muutub
    partial void OnSoundEnabledChanged(bool value)
    {
        SettingsService.Current.SoundEnabled = value;
        SettingsService.Save();
    }

    // Kutsutakse automaatselt, kui Volume muutub.
    // Faili kirjutab leht alles siis, kui lohistamine lõpeb (muidu kirjutaks fail iga liigutuse peale)
    partial void OnVolumeChanged(double value)
    {
        SettingsService.Current.Volume = value;
    }

    public void RefreshTexts()
    {
        TitleText = LocalizationService.Get("SettingsTitle");
        LanguageLabel = LocalizationService.Get("LanguageLabel");
        ThemeLabel = LocalizationService.Get("ThemeLabel");
        SoundLabel = LocalizationService.Get("SoundLabel");
        VolumeLabel = LocalizationService.Get("VolumeLabel");
        BackText = LocalizationService.Get("Back");
    }
}