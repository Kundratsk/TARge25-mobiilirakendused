using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

public class BasicEnemy : Enemy
{
    public override string Symbol => "👾";

    public BasicEnemy(Point start)
        : base("Tavaline", maxHealth: 40, speed: 1.0, reward: 10, start)
    {
    }
}