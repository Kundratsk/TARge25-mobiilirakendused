namespace TowerDefense.Models;

// Üks rida edetabelis (salvestatakse JSON-ina)
public class ScoreEntry
{
    public string Name { get; set; } = "";
    public int Score { get; set; }
    public int Wave { get; set; }          // mitmenda laineni jõuti
    public bool Won { get; set; }          // true = läbis kõik lained
    public double Seconds { get; set; }    // mänguaeg
    public DateTime Date { get; set; }
}