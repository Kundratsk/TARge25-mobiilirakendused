using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TowerDefense.Services;

namespace TowerDefense.Themes;

public class Theme
{
    private readonly string _nameKey;

    // Nimi tuleb nüüd ressursifailist valitud keeles
    public string Name => LocalizationService.Get(_nameKey);

    public Color BackgroundColor { get; }
    public Color CardColor { get; }
    public Color TextColor { get; }
    public Color AccentColor { get; }
    public string FontFamily { get; }

    public Theme(string nameKey, Color background, Color card, Color text, Color accent, string fontFamily)
    {
        _nameKey = nameKey;
        BackgroundColor = background;
        CardColor = card;
        TextColor = text;
        AccentColor = accent;
        FontFamily = fontFamily;
    }

    // Rakendab teema lehele. XAML-is kasutame neid võtmeid DynamicResource'ina.
    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;
        page.Resources["PageBackground"] = BackgroundColor;
        page.Resources["CardColor"] = CardColor;
        page.Resources["PrimaryText"] = TextColor;
        page.Resources["AccentColor"] = AccentColor;
        page.Resources["AppFont"] = FontFamily;
    }

    // Kõik saadaval olevad teemad (esimene argument on ressursifaili võti)
    public static readonly List<Theme> All = new()
    {
        new Theme("ThemeLight",
            Color.FromArgb("#F5F5F5"), Color.FromArgb("#FFFFFF"),
            Color.FromArgb("#222222"), Color.FromArgb("#2E7D32"),
            "OpenSansRegular"),

        new Theme("ThemeDark",
            Color.FromArgb("#1E1E2E"), Color.FromArgb("#2A2A3C"),
            Color.FromArgb("#EEEEEE"), Color.FromArgb("#FF9800"),
            "OpenSansSemibold"),

        new Theme("ThemeColorful",
            Color.FromArgb("#4A148C"), Color.FromArgb("#7B1FA2"),
            Color.FromArgb("#FFEB3B"), Color.FromArgb("#00E5FF"),
            "OpenSansSemibold")
    };
}