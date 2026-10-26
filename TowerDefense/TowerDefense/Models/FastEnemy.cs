using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

// Kiire, aga napilt elu
public class FastEnemy : Enemy
{
    public override string Symbol => "👺";
    public override string ImageName => "fast_enemy.png";

    public FastEnemy(Point start)
        : base("Kiire", maxHealth: 35, speed: 2.5, reward: 15, start)
    {
    }
}