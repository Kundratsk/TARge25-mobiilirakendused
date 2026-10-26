namespace TowerDefense.Models;

// Kõik kasutaja seaded ühes klassis; salvestatakse JSON-failina
public class AppSettings
{
    public int ThemeIndex { get; set; } = 1;       // 1 = "Tume"
    public string Language { get; set; } = "et";
    public bool SoundEnabled { get; set; } = true;
    public double Volume { get; set; } = 0.7;      // 0.0 - 1.0
    public int BestScore { get; set; }
    public string PlayerName { get; set; } = "";   // viimati sisestatud nimi
}