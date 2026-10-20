using System;
using System.Collections.Generic;
using System.Text;

namespace TowerDefense.Models
{
    public abstract class Tower
    {
        public string Name { get;  }
        public abstract string Symbol { get; }
        public int Cost { get;  }
        public double Range { get; } // Ruutudes
        public int Damage { get;  }
        public double FireRate { get; }
        public int Column { get;  }
        public int Row { get;  }

        private double _cooldown; 

        protected Tower (string name, int cost, double range, int damage, double fireRate, int column, int row)
        {
            Name = name;
            Cost = cost;
            Range = range;
            Damage = damage;
            FireRate = fireRate;
            Column = column;
            Row = row;
        }

        public bool IsInRange(Enemy enemy)
        {
            double dx = enemy.X - Column;
            double dy = enemy.Y - Row;
            return Math.Sqrt(dx * dx + dy * dy) <= Range;
        }

        public int GetSellValue()
        {
            return (int)(Cost * 0.7);
        }

        // Tagastab vaenlase, keda tulistati (või null, kui ei tulistatud)
        public Enemy? Update(double deltaSeconds, IEnumerable<Enemy>enemies)
        {
            _cooldown -= deltaSeconds;
            if (_cooldown > 0) return null;

            Enemy? target = FindTarget(enemies);
            if (target == null) return null;

            _cooldown = 1.0 / FireRate;
            Attack(target, enemies);
            return target;
        }
        // Vaikimisi sihib vaenlast, kes on rajal kõige kaugemal
        protected virtual Enemy? FindTarget(IEnumerable<Enemy> enemies) =>
            enemies.Where(e => e.IsAlive && IsInRange(e))
                   .OrderByDescending(e => e.WaypointIndex)
                   .FirstOrDefault();

        // Alamklassid saavad seda üle kirjutada (nt kahjustus mitmele vaenlasele)
        protected virtual void Attack(Enemy target,IEnumerable<Enemy> enemies) => target.TakeDamage(Damage);
    }
}
