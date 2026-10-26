namespace TowerDefense.Models;

public class Player
{
    public string Name { get; set; }
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int Money { get; private set; }

    public bool IsAlive => Lives > 0;

    public Player(string name, int lives = 10, int money = 100)
    {
        Name = name;
        Lives = lives;
        Money = money;
    }

    public void AddScore(int points) => Score += points;

    public void AddMoney(int amount) => Money += amount;

    // Tagastab true, kui raha jätkus
    public bool SpendMoney(int amount)
    {
        if (amount > Money) return false;
        Money -= amount;
        return true;
    }

    public void LoseLife(int amount = 1)
        => Lives = Math.Max(0, Lives - amount);

    // Salvestatud mängu taastamiseks (setterid on privaatsed, seega läbi selle meetodi)
    public void Restore(int lives, int money, int score)
    {
        Lives = lives;
        Money = money;
        Score = score;
    }
}