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
    [ObservableProperty] private string _statusText = "Vali torn ja puuduta ruutu, et see paigutada.";
    [ObservableProperty] private string _startButtonText = "▶";
    [ObservableProperty] private TowerType _selectedType = TowerType.Archer;
    [ObservableProperty] private bool _isSellMode;

    // Abiomadused nuppude esiletõstmiseks (XAML-is kasutame neid hiljem)
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
        Game.TowerFired += (tower, enemy) => ShotFired?.Invoke(tower, enemy);
        Game.GameEnded += OnGameEnded;

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
        Ticked?.Invoke(dt);

        UpdateLabels();
        RedrawRequested?.Invoke();
    }

    private async void OnGameEnded(bool won)
    {
        int score = Game.Player.Score;

        // Parim tulemus tuleb nüüd JSON-seadetest
        AppSettings settings = SettingsService.Current;
        int best = settings.BestScore;
        bool newBest = score > best;
        if (newBest)
        {
            settings.BestScore = score;
            SettingsService.Save();
        }

        string title = won ? "🎉 Võit!" : "💀 Kaotus";
        string message =
            $"Punktid: {score}\n" +
            $"Aeg: {Game.ElapsedSeconds:F0} s\n" +
            $"Laine: {Game.CurrentWave}/{Game.TotalWaves}\n" +
            (newBest ? "Uus rekord!" : $"Parim tulemus: {best}");

        await Shell.Current.DisplayAlert(title, message, "OK");

        Game.Reset();
        GameReset?.Invoke();
        IsSellMode = false;
        StartButtonText = "▶";
        StatusText = "Vali torn ja puuduta ruutu, et see paigutada.";
        UpdateLabels();
        RedrawRequested?.Invoke();
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
            ? "Müügirežiim: puuduta torni, mida müüa."
            : "Vali torn ja puuduta ruutu, et see paigutada.";
    }

    [RelayCommand]
    private void StartPause()
    {
        switch (Game.State)
        {
            case GameState.NotStarted:
                Game.Start();
                StartButtonText = "⏸";
                break;
            case GameState.Running:
                Game.Pause();
                StartButtonText = "▶";
                break;
            case GameState.Paused:
                Game.Resume();
                StartButtonText = "⏸";
                break;
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
                Game.SellTower(tower);
                StatusText = $"Müüdud: {tower.Name} (+ {value} 💰)";
            }
            else
            {
                StatusText = "Selles ruudus pole torni";
            }
        }
        else
        {
            Game.TryPlaceTower(SelectedType, column, row, out string message);
            StatusText = message;
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