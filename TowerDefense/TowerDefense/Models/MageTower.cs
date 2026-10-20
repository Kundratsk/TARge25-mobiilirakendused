namespace TowerDefense.Models;

    public class MageTower : Tower
    {
        public override string Symbol => "🧙";

        public MageTower(int column, int row)
            : base("Võlur", cost: 170, range: 2, damage: 20, fireRate: 1.5, column, row)





    {
    }

}
