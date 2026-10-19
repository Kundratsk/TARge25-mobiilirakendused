using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

// Aeglane, aga väga vastupidav
public class TankEnemy : Enemy
{
    public override string Symbol => "🐘";

    public TankEnemy(Point start)
        : base("Tank", maxHealth: 150, speed: 0.6, reward: 40, start)
    {
    }
}