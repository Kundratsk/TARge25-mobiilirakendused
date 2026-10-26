using System.Globalization;
using System.Resources;

namespace TowerDefense.Services;

// Loeb tekste .resx failidest valitud keeles
public static class LocalizationService
{
    // Nimi = projekti nimeruum + kaust + failinimi (ilma .resx lõputa)
    private static readonly ResourceManager Manager =
        new("TowerDefense.Resources.Strings.AppResources",
            typeof(LocalizationService).Assembly);

    // Saadaolevad keeled: kood ja nimi, mida kasutaja näeb
    public static IReadOnlyList<(string Code, string Name)> Languages { get; } =
        new List<(string Code, string Name)>
        {
            ("et", "Eesti"),
            ("en", "English")
        };

    private static CultureInfo? _culture;

    // Esimesel kasutamisel loeme keele seadetest; vigase koodi korral jääb eesti keel
    private static CultureInfo Culture => _culture ??= LoadCulture();

    private static CultureInfo LoadCulture()
    {
        try
        {
            return new CultureInfo(SettingsService.Current.Language);
        }
        catch
        {
            return new CultureInfo("et");
        }
    }

    public static string CurrentLanguage => Culture.TwoLetterISOLanguageName;

    // Teavitab, kui keel vahetub (leht uuendab siis oma tekste)
    public static event Action? LanguageChanged;

    public static void SetLanguage(string code)
    {
        _culture = new CultureInfo(code);
        CultureInfo.CurrentUICulture = _culture;
        CultureInfo.DefaultThreadCurrentUICulture = _culture;

        SettingsService.Current.Language = code;
        SettingsService.Save();

        LanguageChanged?.Invoke();
    }

    // Tagastab teksti võtme järgi; kui võtit pole, näitab võtit ennast
    public static string Get(string key) =>
        Manager.GetString(key, Culture) ?? key;

    // Versioon kohatäitjatega: Get("TowerSold", nimi, hind) asendab {0} ja {1}
    public static string Get(string key, params object[] args) =>
        string.Format(Culture, Get(key), args);
}