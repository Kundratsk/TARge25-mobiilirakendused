namespace TowerDefense.Models;

// Kallis ja aeglane torn, suur kahjustus ja pikem laskeulatus
public class CannonTower : Tower
{
    public override string Symbol => "💣";

    public CannonTower(int column, int row)
        : base("Kahur", cost: 120, range: 3.5, damage: 40, fireRate: 0.7, column, row)
    {
    }
}