using System.ComponentModel;
using System.Globalization;
using LanguageGallery.Resources.Strings;

namespace LanguageGallery.Services;

public class LocalizationResourceManager : INotifyPropertyChanged
{
    public static LocalizationResourceManager Instance { get; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] =>
        AppResources.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;

    public void SetCulture(string code)
    {
        var culture = new CultureInfo(code);
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }
}