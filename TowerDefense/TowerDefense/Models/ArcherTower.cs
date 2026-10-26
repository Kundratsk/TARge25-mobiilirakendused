namespace TowerDefense.Models;

// Odav ja kiire torn, väike kahjustus
public class ArcherTower : Tower
{
    public override string Symbol => "🏹";
    

    public ArcherTower(int column, int row)
        : base("Vibukütt", cost: 50, range: 2.5, damage: 10, fireRate: 2.0, column, row)
    {
    }
}