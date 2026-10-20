using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

// Aeglane, aga väga vastupidav
public class TankEnemy : Enemy
{
    public override string Symbol => "🐘";

    public TankEnemy(Point start)
        : base("Tank", maxHealth: 150, speed: 1.0, reward: 25, start)
    {
    }
}