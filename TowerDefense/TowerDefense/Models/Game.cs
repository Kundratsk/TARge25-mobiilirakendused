using Microsoft.Maui.Graphics;
using TowerDefense.Services;

namespace TowerDefense.Models;

public class Game
{
    // Mänguväli on ruudustik; torn asub ruudus (Column, Row)
    public const int Columns = 9;
    public const int Rows = 7;
    public const int TotalWaves = 100;

    private const double SpawnInterval = 0.9;   // sekundit vaenlaste vahel
    private const double WaveDelay = 3.0;       // paus lainete vahel

    // Rada: vaenlased liiguvad nende punktide kaudu (veerg, rida)
    private readonly List<Point> _path = new()
{
    new Point(0, 1),
    new Point(6, 1),
    new Point(6, 0),
    new Point(8, 0),
    new Point(8, 3),
    new Point(0, 3),
    new Point(0, 6),
    new Point(2, 6),
    new Point(2, 4),
    new Point(4, 4),
    new Point(4, 6),
    new Point(6, 6),
    new Point(6, 4),
    new Point(8, 4),
    new Point(8, 6)
};

    private readonly List<Enemy> _enemies = new();
    private readonly List<Tower> _towers = new();
    private readonly Queue<Func<Enemy>> _spawnQueue = new();

    private double _spawnTimer;
    private double _waveDelayTimer;
    private bool _waveInProgress;

    public Player Player { get; private set; }
    public GameState State { get; private set; } = GameState.NotStarted;
    public int CurrentWave { get; private set; }
    public double ElapsedSeconds { get; private set; }

    public IReadOnlyList<Point> Path => _path;
    public IReadOnlyList<Enemy> Enemies => _enemies;
    public IReadOnlyList<Tower> Towers => _towers;

    // Sündmused, millele UI saab reageerida
    public event Action<int>? WaveStarted;
    public event Action<Tower, Enemy>? TowerFired;
    public event Action<bool>? GameEnded;   // true = võit, false = kaotus

    public Game(Player player)
    {
        Player = player;
        _waveDelayTimer = 2.0;
    }

    // ---------- Mängu juhtimine ----------

    public void Start()
    {
        if (State == GameState.NotStarted)
            State = GameState.Running;
    }

    public void Pause()
    {
        if (State == GameState.Running)
            State = GameState.Paused;
    }

    public void Resume()
    {
        if (State == GameState.Paused)
            State = GameState.Running;
    }

    public void Reset()
    {
        Player = new Player(Player.Name);
        _enemies.Clear();
        _towers.Clear();
        _spawnQueue.Clear();
        CurrentWave = 0;
        ElapsedSeconds = 0;
        _spawnTimer = 0;
        _waveDelayTimer = 2.0;
        _waveInProgress = false;
        State = GameState.NotStarted;
    }

    public void getStrongerEnemies()
    {
        if (CurrentWave >= 5)
        {
            foreach (Enemy enemy in _enemies)
            {

            }
        }
    }

    // ---------- Salvestamine ja laadimine ----------

    // Kogub kokku kõik, mis on vaja mängu hiljem taastamiseks
    public SaveData CreateSaveData()
    {
        var data = new SaveData
        {
            // Kui laine on pooleli, laeme selle uuesti algusest: seega salvestame eelmise laine numbri
            Wave = _waveInProgress ? Math.Max(0, CurrentWave - 1) : CurrentWave,
            Lives = Player.Lives,
            Money = Player.Money,
            Score = Player.Score,
            ElapsedSeconds = ElapsedSeconds
        };

        foreach (Tower tower in _towers)
        {
            data.Towers.Add(new SavedTower
            {
                Type = GetTowerType(tower),
                Column = tower.Column,
                Row = tower.Row
            });
        }

        return data;
    }

    // Taastab mängu salvestusest; mäng jääb pausile, kuni mängija vajutab ▶
    public void LoadFrom(SaveData data)
    {
        Reset();

        Player.Restore(data.Lives, data.Money, data.Score);
        CurrentWave = data.Wave;
        ElapsedSeconds = data.ElapsedSeconds;

        foreach (SavedTower saved in data.Towers)
            _towers.Add(CreateTower(saved.Type, saved.Column, saved.Row));

        State = GameState.Paused;
    }

    // Torni objektist tagasi tüüp (vajalik salvestamisel)
    private static TowerType GetTowerType(Tower tower) => tower switch
    {
        ArcherTower => TowerType.Archer,
        CannonTower => TowerType.Cannon,
        MageTower => TowerType.Mage,
        _ => throw new ArgumentException("Tundmatu torni klass.", nameof(tower))
    };

    // Torni nimi kasutaja keeles (teadetes näitamiseks)
    public static string GetTowerName(Tower tower) => GetTowerType(tower) switch
    {
        TowerType.Archer => LocalizationService.Get("TowerArcher"),
        TowerType.Cannon => LocalizationService.Get("TowerCannon"),
        TowerType.Mage => LocalizationService.Get("TowerMage"),
        _ => tower.Name
    };

    private static Tower CreateTower(TowerType type, int column, int row) => type switch
    {
        TowerType.Archer => new ArcherTower(column, row),
        TowerType.Cannon => new CannonTower(column, row),
        TowerType.Mage => new MageTower(column, row),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tundmatu torni tüüp.")
    };

    // ---------- Tornide paigutamine ----------

    public bool IsPathCell(int column, int row)
    {
        for (int i = 0; i < _path.Count - 1; i++)
        {
            Point a = _path[i];
            Point b = _path[i + 1];

            bool inColumns = column >= Math.Min(a.X, b.X) && column <= Math.Max(a.X, b.X);
            bool inRows = row >= Math.Min(a.Y, b.Y) && row <= Math.Max(a.Y, b.Y);

            if (inColumns && inRows) return true;
        }
        return false;
    }

    public Tower? GetTowerAt(int column, int row) =>
        _towers.FirstOrDefault(t => t.Column == column && t.Row == row);

    public bool TryPlaceTower(TowerType type, int column, int row, out string message)
    {
        if (State != GameState.Running && State != GameState.Paused && State != GameState.NotStarted)
        {
            message = LocalizationService.Get("GameIsOver");
            return false;
        }

        if (column < 0 || column >= Columns || row < 0 || row >= Rows)
        {
            message = LocalizationService.Get("OutOfBounds");
            return false;
        }

        if (IsPathCell(column, row))
        {
            message = LocalizationService.Get("OnPath");
            return false;
        }

        if (GetTowerAt(column, row) != null)
        {
            message = LocalizationService.Get("TowerExists");
            return false;
        }

        Tower tower = CreateTower(type, column, row);

        if (!Player.SpendMoney(tower.Cost))
        {
            message = LocalizationService.Get("NotEnoughMoney", tower.Cost);
            return false;
        }

        _towers.Add(tower);
        message = LocalizationService.Get("TowerPlaced", GetTowerName(tower));
        return true;
    }

    //Tornide müümine

    public void SellTower(Tower tower)
    {
        if (_towers.Remove(tower))
        {
            Player.AddMoney(tower.GetSellValue());
        }
    }

    // ---------- Mängu uuendus (kutsutakse UI taimerist) ----------

    public void Update(double deltaSeconds)
    {
        if (State != GameState.Running) return;

        ElapsedSeconds += deltaSeconds;

        UpdateWaves(deltaSeconds);
        if (State != GameState.Running) return;

        // Vaenlased liiguvad
        foreach (Enemy enemy in _enemies)
            enemy.Update(deltaSeconds, _path);

        // Tornid tulistavad
        foreach (Tower tower in _towers)
        {
            Enemy? target = tower.Update(deltaSeconds, _enemies);
            if (target != null)
                TowerFired?.Invoke(tower, target);
        }

        // Surnud ja lõppu jõudnud vaenlased
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _enemies[i];

            if (!enemy.IsAlive)
            {
                Player.AddMoney(enemy.Reward);
                Player.AddScore(enemy.Reward);
                _enemies.RemoveAt(i);
            }
            else if (enemy.ReachedEnd)
            {
                Player.LoseLife();
                _enemies.RemoveAt(i);
            }
        }

        if (!Player.IsAlive)
            EndGame(false);
    }

    private void UpdateWaves(double deltaSeconds)
    {
        // Praegune laine veel käib: vaenlasi tuleb juurde
        if (_spawnQueue.Count > 0)
        {
            _spawnTimer -= deltaSeconds;
            if (_spawnTimer <= 0)
            {
                _enemies.Add(_spawnQueue.Dequeue()());
                _spawnTimer = SpawnInterval;
            }
            return;
        }

        // Laine on läbi, kui kõik vaenlased on kadunud
        if (_waveInProgress && _enemies.Count == 0)
        {
            _waveInProgress = false;

            if (CurrentWave >= TotalWaves)
            {
                Player.AddScore(Player.Lives * 50);   // boonus allesjäänud elude eest
                EndGame(true);
                return;
            }

            _waveDelayTimer = WaveDelay;
        }

        // Ootame järgmist lainet
        if (!_waveInProgress)
        {
            _waveDelayTimer -= deltaSeconds;
            if (_waveDelayTimer <= 0)
                StartNextWave();
        }
    }

    private void StartNextWave()
    {
        CurrentWave++;
        _waveInProgress = true;
        _spawnTimer = 0;

        Point start = _path[0];
        int n = CurrentWave;

        for (int i = 0; i < 3 + n * 2.5; i++)
            _spawnQueue.Enqueue(() => StrengthenEnemy(new BasicEnemy(start)));

        if (n >= 2)
            for (int i = 0; i < n; i++)
                _spawnQueue.Enqueue(() => StrengthenEnemy(new FastEnemy(start)));

        if (n >= 3)
            for (int i = 0; i < n - 2; i++)
                _spawnQueue.Enqueue(() => StrengthenEnemy(new TankEnemy(start)));

        

        WaveStarted?.Invoke(CurrentWave);
    }

    private void EndGame(bool won)
    {
        State = won ? GameState.Won : GameState.Lost;
        GameEnded?.Invoke(won);
    }
    private Enemy StrengthenEnemy(Enemy enemy)
    {
        int upgrades = CurrentWave / 5;

        for (int i = 0; i < upgrades; i++)
        {
            enemy.IncreaseHealth();
        }

        return enemy;
    }

}