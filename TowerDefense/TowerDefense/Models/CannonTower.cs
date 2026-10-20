namespace TowerDefense.Models;

// Kallis ja aeglane torn, suur kahjustus ja pikem laskeulatus
public class CannonTower : Tower
{
    public override string Symbol => "💣";

    public CannonTower(int column, int row)
        : base("Kahur", cost: 120, range: 3.5, damage: 20, fireRate: 0.7, column, row)
    {
    }
    protected override void Attack(
    Enemy target,
    IEnumerable<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.IsAlive)
                continue;

            double dx = enemy.X - target.X;
            double dy = enemy.Y - target.Y;

            if (Math.Sqrt(dx * dx + dy * dy) <= 1.0)
                enemy.TakeDamage(Damage);
        }
    }

}