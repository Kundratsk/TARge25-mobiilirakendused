using TowerDefense.Models;
using TowerDefense.Themes;

namespace TowerDefense;

public partial class MainPage : ContentPage
{
    private readonly Game _game;
    private readonly GameDrawable _drawable;
    private readonly IDispatcherTimer _timer;
    private DateTime _lastTick;

    private TowerType _selectedType = TowerType.Archer;
    private Theme _theme = Theme.All[1];   // vaikimisi "Tume"
    private bool _sellMode = false;

    public MainPage()
    {
        InitializeComponent();

        _game = new Game(new Player("Mängija"));
        _game.TowerFired += OnTowerFired;
        _game.GameEnded += OnGameEnded;

        _drawable = new GameDrawable { Game = _game, Theme = _theme };
        BoardView.Drawable = _drawable;

        ThemePicker.ItemsSource = Theme.All.Select(t => t.Name).ToList();
        ThemePicker.SelectedIndex = 1;   // käivitab OnThemeChanged
        ApplyTheme();

        _lastTick = DateTime.UtcNow;
        _timer = Dispatcher.CreateTimer();
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

        _game.Update(dt);

        for (int i = _drawable.Shots.Count - 1; i >= 0; i--)
        {
            _drawable.Shots[i].Ttl -= dt;
            if (_drawable.Shots[i].Ttl <= 0)
                _drawable.Shots.RemoveAt(i);
        }

        UpdateLabels();
        BoardView.Invalidate();
    }

    private void OnTowerFired(Tower tower, Enemy enemy)
    {
        _drawable.Shots.Add(new Shot
        {
            X1 = tower.Column + 0.5f,
            Y1 = tower.Row + 0.5f,
            X2 = (float)enemy.X + 0.5f,
            Y2 = (float)enemy.Y + 0.5f
        });
    }

    private async void OnGameEnded(bool won)
    {
        int score = _game.Player.Score;
        int best = Preferences.Get("BestScore", 0);
        bool newBest = score > best;
        if (newBest) Preferences.Set("BestScore", score);

        string title = won ? "🎉 Võit!" : "💀 Kaotus";
        string message =
            $"Punktid: {score}\n" +
            $"Aeg: {_game.ElapsedSeconds:F0} s\n" +
            $"Laine: {_game.CurrentWave}/{Game.TotalWaves}\n" +
            (newBest ? "Uus rekord!" : $"Parim tulemus: {best}");

        await DisplayAlert(title, message, "OK");

        _game.Reset();
        _drawable.Shots.Clear();
        _sellMode = false;
        UpdateSellButton();
        StartButton.Text = "▶ Start";
        StatusLabel.Text = "Vali torn ja puuduta ruutu, et see paigutada.";
        UpdateLabels();
        BoardView.Invalidate();
    }

    // ---------- UI sündmused ----------

    private void OnBoardTapped(object? sender, TappedEventArgs e)
    {
        Point? p = e.GetPosition(BoardView);
        if (p == null || _drawable.CellSize <= 0) return;

        int column = (int)Math.Floor((p.Value.X - _drawable.OffsetX) / _drawable.CellSize);
        int row = (int)Math.Floor((p.Value.Y - _drawable.OffsetY) / _drawable.CellSize);

        if (_sellMode)
        {
            Tower? tower = _game.GetTowerAt(column, row);

            if (tower != null)
            {
                int value = tower.GetSellValue();
                _game.SellTower(tower);
                StatusLabel.Text = $"Müüdud: {tower.Name} (+ {value} 💰)";
            }
            else
            {
                StatusLabel.Text = "Selles ruudus pole torni";
            }
            UpdateLabels();
            BoardView.Invalidate();
            return;
        }

        _game.TryPlaceTower(_selectedType, column, row, out string message);
        StatusLabel.Text = message;

        UpdateLabels();
        BoardView.Invalidate();
    }

    private void OnArcherClicked(object? sender, EventArgs e)
    {
        _selectedType = TowerType.Archer;
        UpdateTowerButtons();
    }

    private void OnCannonClicked(object? sender, EventArgs e)
    {
        _selectedType = TowerType.Cannon;
        UpdateTowerButtons();
    }

    private void OnMageClicked(object? sender, EventArgs e)
    {
        _selectedType = TowerType.Mage;
        UpdateTowerButtons();
    }
    private void OnSellClicked(object? sender, EventArgs e)
    {
        _sellMode = !_sellMode;   // ! pöörab väärtuse ümber: false -> true, true -> false

        StatusLabel.Text = _sellMode
            ? "Müügirežiim: puuduta torni, mida müüa."
            : "Vali torn ja puuduta ruutu, et see müüa.";

        UpdateSellButton();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        switch (_game.State)
        {
            case GameState.NotStarted:
                _game.Start();
                StartButton.Text = "⏸ Paus";
                break;
            case GameState.Running:
                _game.Pause();
                StartButton.Text = "▶ Jätka";
                break;
            case GameState.Paused:
                _game.Resume();
                StartButton.Text = "⏸ Paus";
                break;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (ThemePicker.SelectedIndex < 0) return;

        _theme = Theme.All[ThemePicker.SelectedIndex];
        ApplyTheme();
        BoardView.Invalidate();
    }

    // ---------- Abimeetodid ----------

    private void ApplyTheme()
    {
        _theme.Apply(this);
        _drawable.Theme = _theme;

        StartButton.BackgroundColor = _theme.AccentColor;
        StartButton.TextColor = _theme.BackgroundColor;
        UpdateTowerButtons();
        UpdateSellButton();
    }

    private void UpdateTowerButtons()
    {
        bool archer = _selectedType == TowerType.Archer;
        bool cannon = _selectedType == TowerType.Cannon;
        bool mage = _selectedType == TowerType.Mage;

        ArcherButton.BackgroundColor = archer
            ? _theme.AccentColor
            : _theme.CardColor;
        ArcherButton.TextColor = archer
            ? _theme.BackgroundColor
            : _theme.TextColor;

        CannonButton.BackgroundColor = cannon
            ? _theme.AccentColor
            : _theme.CardColor;
        CannonButton.TextColor = cannon
            ? _theme.BackgroundColor
            : _theme.TextColor;

        MageButton.BackgroundColor = mage
            ? _theme.AccentColor
            : _theme.CardColor;
        MageButton.TextColor = mage
            ? _theme.BackgroundColor
            : _theme.TextColor;
    }
    private void UpdateSellButton()
    {
        SellButton.BackgroundColor = _sellMode ? _theme.AccentColor : _theme.CardColor;
        SellButton.TextColor = _sellMode ? _theme.BackgroundColor : _theme.TextColor;
    }

    private void UpdateLabels()
    {
        LivesLabel.Text = $"❤️ {_game.Player.Lives}";
        MoneyLabel.Text = $"💰 {_game.Player.Money}";
        ScoreLabel.Text = $"⭐ {_game.Player.Score}";
        WaveLabel.Text = $"🌊 {_game.CurrentWave}/{Game.TotalWaves}";
    }
}