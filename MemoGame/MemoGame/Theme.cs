namespace MemoGame;

public class Theme
{
    // Väljad (inkapsulatsioon: väljastpoolt ligipääs ainult omaduste kaudu)
    private string _name;
    private Color _backgroundColor;
    private Color _textColor;
    private Color _cardColor;
    private string _fontFamily;

    // Omadused
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => _backgroundColor = value;
    }

    public Color TextColor
    {
        get => _textColor;
        set => _textColor = value;
    }

    public Color CardColor
    {
        get => _cardColor;
        set => _cardColor = value;
    }

    public string FontFamily
    {
        get => _fontFamily;
        set => _fontFamily = value;
    }

    // Konstruktor
    public Theme(string name, Color background, Color text, Color card, string fontFamily)
    {
        _name = name;
        _backgroundColor = background;
        _textColor = text;
        _cardColor = card;
        _fontFamily = fontFamily;
    }

    // Rakendab teema lehele. DynamicResource uuendab välimuse kohe.
    public void Apply(ContentPage page)
    {
        page.BackgroundColor = _backgroundColor;
        page.Resources["TextColor"] = _textColor;
        page.Resources["CardColor"] = _cardColor;
        page.Resources["AppFont"] = _fontFamily;
    }

    // Valmis teemad
    public static Theme Light()
    {
        return new Theme("Hele", Colors.White, Colors.Black,
            Color.FromArgb("#4A90D9"), "OpenSansRegular");
    }

    public static Theme Dark()
    {
        return new Theme("Tume", Color.FromArgb("#1E1E1E"), Colors.White,
            Color.FromArgb("#555555"), "OpenSansRegular");
    }

    public static Theme Colorful()
    {
        return new Theme("Värviline", Color.FromArgb("#FFF3B0"), Color.FromArgb("#9E0059"),
            Color.FromArgb("#FF6B6B"), "OpenSansSemibold");
    }

    public static List<Theme> GetAll()
    {
        return new List<Theme> { Light(), Dark(), Colorful() };
    }
}