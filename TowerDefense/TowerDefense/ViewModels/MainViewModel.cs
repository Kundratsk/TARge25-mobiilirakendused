using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TowerDefense.Models;
using TowerDefense.Services;

namespace TowerDefense.ViewModels;

// "partial" on vajalik, sest CommunityToolkit genereerib selle klassi teise poole ise
// (omadused väljade põhjal ja käsud meetodite põhjal).
public partial class MainViewModel : ObservableObject
{
    private readonly IDispatcherTimer _timer;
    private DateTime _lastTick;
    private int _lastLives;   // et märgata, millal elu kaoti (heli jaoks)

    public Game Game { get; }

    // Sündmused, mille kaudu ViewModel annab lehele (View) teada, et midagi tuleb joonistada
    public event Action<double>? Ticked;              // iga tiksu järel (lehel uuenevad lasud)
    public event Action<Tower, Enemy>? ShotFired;     // torn tulistas
    public event Action? GameReset;                   // mäng nullitud (leht tühjendab lasud)
    public event Action? RedrawRequested;             // laud tuleb uuesti joonistada

    // [ObservableProperty] teeb väljast "_livesText" omaduse "LivesText",
    // mis annab XAML-ile muutusest automaatselt teada.
    [ObservableProperty] private string _livesText = "";
    [ObservableProperty] private string _moneyText = "";
    [ObservableProperty] private string _scoreText = "";
    [ObservableProperty] private string _waveText = "";
    [ObservableProperty] private string _bestScoreText = SettingsService.Current.BestScore.ToString();
    [ObservableProperty] private string _statusText = LocalizationService.Get("StartHint");
    [ObservableProperty] private string _startButtonText = "▶";
    [ObservableProperty] private TowerType _selectedType = TowerType.Archer;
    [ObservableProperty] private bool _isSellMode;

    // Abiomadused nuppude esiletõstmiseks
    public bool IsArcherSelected => SelectedType == TowerType.Archer;
    public bool IsCannonSelected => SelectedType == TowerType.Cannon;
    public bool IsMageSelected => SelectedType == TowerType.Mage;

    // Kutsutakse automaatselt, kui SelectedType muutub
    partial void OnSelectedTypeChanged(TowerType value)
    {
        OnPropertyChanged(nameof(IsArcherSelected));
        OnPropertyChanged(nameof(IsCannonSelected));
        OnPropertyChanged(nameof(IsMageSelected));
    }

    public MainViewModel()
    {
        Game = new Game(new Player("Mängija"));
        Game.TowerFired += OnTowerFired;
        Game.GameEnded += OnGameEnded;

        // Kui eelmisest korrast on pooleli jäänud mäng, taastame selle (jääb pausile)
        SaveData? save = SaveGameService.Load();
        if (save != null)
        {
            Game.LoadFrom(save);
            StartButtonText = "▶";
            StatusText = LocalizationService.Get("GameRestored");
        }

        _lastLives = Game.Player.Lives;

        _lastTick = DateTime.UtcNow;
        _timer = Dispatcher.GetForCurrentThread()!.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(33);
        _timer.Tick += OnTick;
        _timer.Start();

        UpdateLabels();
    }

    // ---------- Mängu tsükkel ----------

    private void OnTick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.UtcNow;
        double dt = Math.Min((now - _lastTick).TotalSeconds, 0.1);
        _lastTick = now;

        Game.Update(dt);

        // Elu kaotus: heli (mitte siis, kui mäng just lõppes, sest siis kõlab kaotuse heli)
        int lives = Game.Player.Lives;
        if (lives < _lastLives && Game.State == GameState.Running)
            _ = AudioService.PlayEffectAsync("life_lost.wav");
        _lastLives = lives;

        Ticked?.Invoke(dt);

        UpdateLabels();
        RedrawRequested?.Invoke();
    }

    private void OnTowerFired(Tower tower, Enemy enemy)
    {
        ShotFired?.Invoke(tower, enemy);
        _ = AudioService.PlayShotAsync();   // "_ =" tähendab: ei oota heli lõppu
    }

    private async void OnGameEnded(bool won)
    {
        // Mäng sai läbi: muusika peatub, kõlab võidu- või kaotusheli
        AudioService.StopMusic();
        _ = AudioService.PlayEffectAsync(won ? "win.wav" : "lose.wav");

        // Pooleli jäänud mängu salvestust pole enam vaja
        SaveGameService.Delete();

        // Jätame tulemuse meelde enne dialoogi (dialoogi ajal mäng enam ei uuene)
        int score = Game.Player.Score;
        int wave = Game.CurrentWave;
        double seconds = Game.ElapsedSeconds;

        // Parim tulemus tuleb JSON-seadetest
        AppSettings settings = SettingsService.Current;
        int best = settings.BestScore;
        bool newBest = score > best;
        if (newBest)
        {
            settings.BestScore = score;
            SettingsService.Save();
            BestScoreText = score.ToString();
        }

        string title = LocalizationService.Get(won ? "Victory" : "Defeat");
        string message =
            LocalizationService.Get("ResultScore", score) + "\n" +
            LocalizationService.Get("ResultTime", seconds.ToString("F0")) + "\n" +
            LocalizationService.Get("ResultWave", wave, Game.TotalWaves) + "\n" +
            (newBest
                ? LocalizationService.Get("NewRecord")
                : LocalizationService.Get("BestScore", best)) +
            "\n\n" + LocalizationService.Get("EnterName");

        // Küsime mängija nime; "Jäta vahele" annab tulemuseks null
        string? name = await Shell.Current.DisplayPromptAsync(
            title,
            message,
            accept: LocalizationService.Get("Ok"),
            cancel: LocalizationService.Get("Skip"),
            placeholder: LocalizationService.Get("NamePlaceholder"),
            maxLength: 20,
            initialValue: settings.PlayerName);

        int rank = 0;
        if (name != null)
        {
            string typed = name.Trim();

            // Tühja nime korral kasutame vaikenime, aga seda meelde ei jäta
            if (typed.Length > 0)
            {
                settings.PlayerName = typed;
                SettingsService.Save();
            }

            rank = LeaderboardService.Add(new ScoreEntry
            {
                Name = typed.Length > 0 ? typed : LocalizationService.Get("DefaultPlayerName"),
                Score = score,
                Wave = wave,
                Won = won,
                Seconds = seconds,
                Date = DateTime.Now
            });
        }

        RestartGame();

        // Kui tulemus mahtus edetabelisse, ütleme seda olekureal
        if (rank > 0)
            StatusText = LocalizationService.Get("LeaderboardRank", rank);
    }

    // Nullib mängu ja kogu sellega seotud UI oleku (kasutavad nii mängu lõpp kui 🔄 nupp)
    private void RestartGame()
    {
        AudioService.StopMusic();

        Game.Reset();
        _lastLives = Game.Player.Lives;

        GameReset?.Invoke();
        IsSellMode = false;
        StartButtonText = "▶";
        StatusText = LocalizationService.Get("StartHint");
        UpdateLabels();
        RedrawRequested?.Invoke();
    }

    // ---------- Salvestamine ----------

    // Salvestab ainult käimasoleva või pausil mängu; lõppenud mängu ei salvesta
    public void SaveProgress()
    {
        if (Game.State == GameState.Running || Game.State == GameState.Paused)
            SaveGameService.Save(Game.CreateSaveData());
    }

    // ---------- Käsud (nupud seovad end nende külge) ----------

    // [RelayCommand] teeb meetodist SelectArcher käsu nimega SelectArcherCommand
    [RelayCommand]
    private void SelectArcher() => SelectedType = TowerType.Archer;

    [RelayCommand]
    private void SelectCannon() => SelectedType = TowerType.Cannon;

    [RelayCommand]
    private void SelectMage() => SelectedType = TowerType.Mage;

    [RelayCommand]
    private void ToggleSell()
    {
        IsSellMode = !IsSellMode;

        StatusText = IsSellMode
            ? LocalizationService.Get("SellModeOn")
            : LocalizationService.Get("StartHint");
    }

    [RelayCommand]
    private void StartPause()
    {
        switch (Game.State)
        {
            case GameState.NotStarted:
                Game.Start();
                StartButtonText = "⏸";
                _ = AudioService.StartMusicAsync();
                break;
            case GameState.Running:
                Game.Pause();
                StartButtonText = "▶";
                AudioService.PauseMusic();
                SaveProgress();   // pausile panemisel salvestame
                break;
            case GameState.Paused:
                Game.Resume();
                StartButtonText = "⏸";
                _ = AudioService.StartMusicAsync();
                break;
        }
    }

    // Async käsk: ootab kasutaja vastust ("await"), ilma et rakendus hanguks
    [RelayCommand]
    private async Task ResetGame()
    {
        // Peatame mängu, kuni kasutaja otsustab
        bool wasRunning = Game.State == GameState.Running;
        if (wasRunning) Game.Pause();

        bool confirmed = await Shell.Current.DisplayAlert(
            LocalizationService.Get("ResetTitle"),
            LocalizationService.Get("ResetMessage"),
            LocalizationService.Get("Yes"),
            LocalizationService.Get("No"));

        if (confirmed)
        {
            SaveGameService.Delete();   // pooleli jäänud mängu salvestus kustub
            RestartGame();
        }
        else if (wasRunning)
        {
            Game.Resume();
        }
    }

    // ---------- Lauale puudutamine (leht arvutab ruudu ja kutsub selle meetodi) ----------

    public void TapCell(int column, int row)
    {
        if (IsSellMode)
        {
            Tower? tower = Game.GetTowerAt(column, row);

            if (tower != null)
            {
                int value = tower.GetSellValue();
                string name = Game.GetTowerName(tower);
                Game.SellTower(tower);
                StatusText = LocalizationService.Get("TowerSold", name, value);
                _ = AudioService.PlayEffectAsync("sell.wav");
            }
            else
            {
                StatusText = LocalizationService.Get("NoTowerHere");
            }
        }
        else
        {
            bool placed = Game.TryPlaceTower(SelectedType, column, row, out string message);
            StatusText = message;

            if (placed)
                _ = AudioService.PlayEffectAsync("place.wav");
        }

        UpdateLabels();
        RedrawRequested?.Invoke();
    }

    // ---------- Abimeetodid ----------

    private void UpdateLabels()
    {
        LivesText = $"❤️ {Game.Player.Lives}";
        MoneyText = $"💰 {Game.Player.Money}";
        ScoreText = $"⭐ {Game.Player.Score}";
        WaveText = $"🌊 {Game.CurrentWave}/{Game.TotalWaves}";
    }
}