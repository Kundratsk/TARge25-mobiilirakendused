namespace TowerDefense.Models;

// Kõik, mis on vaja pooleli jäänud mängu taastamiseks (salvestatakse JSON-ina)
public class SaveData
{
    public int Wave { get; set; }
    public int Lives { get; set; }
    public int Money { get; set; }
    public int Score { get; set; }
    public double ElapsedSeconds { get; set; }
    public List<SavedTower> Towers { get; set; } = new();
}

// Üks salvestatud torn: tüüp ja asukoht ruudustikus
public class SavedTower
{
    public TowerType Type { get; set; }
    public int Column { get; set; }
    public int Row { get; set; }
}