using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

// Kiire, aga napilt elu
public class FastEnemy : Enemy
{
    public override string Symbol => "🐇";

    public FastEnemy(Point start)
        : base("Kiire", maxHealth: 25, speed: 2.5, reward: 15, start)
    {
    }
}