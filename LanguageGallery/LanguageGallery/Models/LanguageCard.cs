using System.ComponentModel;
using LanguageGallery.Services;

namespace LanguageGallery.Models;

public class LanguageCard : INotifyPropertyChanged
{
    private static LocalizationResourceManager Loc => LocalizationResourceManager.Instance;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Key { get; }
    public string Name { get; }
    public string ImageSource { get; }

    public string Description => Loc[$"{Key}_Desc"];
    public string Info => Loc[$"{Key}_Info"];

    public LanguageCard(string key, string name, string imageSource)
    {
        Key = key;
        Name = name;
        ImageSource = imageSource;
    }

    public void Refresh() =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
}