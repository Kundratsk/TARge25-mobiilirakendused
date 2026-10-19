using Microsoft.Maui.Graphics;

namespace TowerDefense.Models;

public abstract class Enemy
{
    public string Name { get; }
    public abstract string Symbol { get; }   // emoji, mida kuvame ekraanil
    public int MaxHealth { get; }
    public int Health { get; private set; }
    public double Speed { get; }             // ruutu sekundis
    public int Reward { get; }               // raha tapmise eest

    public double X { get; private set; }
    public double Y { get; private set; }
    public int WaypointIndex { get; private set; }
    public bool ReachedEnd { get; private set; }

    public bool IsAlive => Health > 0;

    protected Enemy(string name, int maxHealth, double speed, int reward, Point start)
    {
        Name = name;
        MaxHealth = maxHealth;
        Health = maxHealth;
        Speed = speed;
        Reward = reward;
        X = start.X;
        Y = start.Y;
    }

    public void TakeDamage(int damage)
        => Health = Math.Max(0, Health - damage);

    // Liigutab vaenlast mööda rada. path[0] on algus.
    public void Update(double deltaSeconds, IReadOnlyList<Point> path) // (IReadOnlyList) See kujutab endast andmekogu, millest saab elemente lugeda ja indeksi alusel üles otsida, kuid mida ei saa pärast loomist muuta 
    { 
        double move = Speed * deltaSeconds;

        while (move > 0 && !ReachedEnd)
        {
            if (WaypointIndex + 1 >= path.Count)
            {
                ReachedEnd = true;
                break;
            }

            Point target = path[WaypointIndex + 1];
            double dx = target.X - X;
            double dy = target.Y - Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist <= move)
            {
                X = target.X;
                Y = target.Y;
                WaypointIndex++;
                move -= dist;
            }
            else
            {
                X += dx / dist * move;
                Y += dy / dist * move;
                move = 0;
            }
        }

        if (WaypointIndex + 1 >= path.Count)
            ReachedEnd = true;
    }
}