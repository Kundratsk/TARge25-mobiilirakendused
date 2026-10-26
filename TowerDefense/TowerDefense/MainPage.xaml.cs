using TowerDefense.Models;
using TowerDefense.Services;
using TowerDefense.Themes;
using TowerDefense.ViewModels;

namespace TowerDefense;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private readonly GameDrawable _drawable;
    private Theme _theme;
    private string _shownLanguage;
    private bool _windowHooked;

    public MainPage()
    {
        InitializeComponent();

        _theme = LoadSavedTheme();
        _shownLanguage = LocalizationService.CurrentLanguage;

        // ViewModel on kogu mängu aju
        _vm = new MainViewModel();
        BindingContext = _vm;

        _drawable = new GameDrawable
        {
            Game = _vm.Game,
            Theme = _theme
        };

        BoardView.Drawable = _drawable;

        // Laadime mängupildid pärast lehe laadimist
        Loaded += async (_, _) =>
        {
            await _drawable.LoadEnemyImagesAsync();
            BoardView.Invalidate();
        };

        // ViewModeli sündmused
        _vm.Ticked += OnTicked;
        _vm.ShotFired += OnShotFired;
        _vm.GameReset += () => _drawable.Shots.Clear();
        _vm.RedrawRequested += () => BoardView.Invalidate();

        // Värskendame nuppe, kui valik muutub
        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedType) ||
                e.PropertyName == nameof(MainViewModel.IsSellMode))
            {
                UpdateButtons();
            }
        };

        ApplyTheme();

        // Akna sündmused
        Loaded += OnPageLoaded;
    }

    // ---------- Elutsükkel ----------

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _theme = LoadSavedTheme();
        ApplyTheme();

        // Edetabelis võidi parim tulemus nullida
        _vm.BestScoreText = SettingsService.Current.BestScore.ToString();

        if (_shownLanguage != LocalizationService.CurrentLanguage)
        {
            _shownLanguage = LocalizationService.CurrentLanguage;

            _vm.StatusText = _vm.IsSellMode
                ? LocalizationService.Get("SellModeOn")
                : LocalizationService.Get("StartHint");
        }
    }

    // ---------- Akna sündmused ----------

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (_windowHooked || Window == null)
            return;

        _windowHooked = true;

        // Aken kaotab fookuse
        Window.Deactivated += (_, _) => _vm.SaveProgress();

        // Rakendus läheb taustale
        Window.Stopped += (_, _) => PauseAndSave();

        // Aken suletakse
        Window.Destroying += (_, _) =>
        {
            _vm.SaveProgress();
            AudioService.StopMusic();
        };
    }

    private void PauseAndSave()
    {
        if (_vm.Game.State == Models.GameState.Running)
            _vm.StartPauseCommand.Execute(null);
        else
            _vm.SaveProgress();
    }

    // ---------- ViewModeli sündmused ----------

    private void OnTicked(double dt)
    {
        // Uuendame mürskude liikumist
        _drawable.UpdateShots(dt);

        // Joonistame mängulaua uuesti
        BoardView.Invalidate();
    }

    private void OnShotFired(Tower tower, Enemy enemy)
    {
        // Lisame mürsu, mis liigub tornist vaenlaseni
        _drawable.AddShot(tower, enemy);

        BoardView.Invalidate();
    }

    // ---------- UI sündmused ----------

    private void OnBoardTapped(object? sender, TappedEventArgs e)
    {
        Point? p = e.GetPosition(BoardView);

        if (p == null || _drawable.CellSize <= 0)
            return;

        int column = (int)Math.Floor(
            (p.Value.X - _drawable.OffsetX) / _drawable.CellSize);

        int row = (int)Math.Floor(
            (p.Value.Y - _drawable.OffsetY) / _drawable.CellSize);

        _vm.TapCell(column, row);
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        PauseIfRunning();
        await Navigation.PushAsync(new SettingsPage());
    }

    private async void OnLeaderboardTapped(object? sender, TappedEventArgs e)
    {
        PauseIfRunning();
        await Navigation.PushAsync(new LeaderboardPage());
    }

    // ---------- Abimeetodid ----------

    // Teiste lehtede ajaks peatame mängu, et vaenlased ei jookseks edasi
    private void PauseIfRunning()
    {
        if (_vm.Game.State == Models.GameState.Running)
            _vm.StartPauseCommand.Execute(null);
    }

    private static Theme LoadSavedTheme()
    {
        int index = Math.Clamp(
            SettingsService.Current.ThemeIndex,
            0,
            Theme.All.Count - 1);

        return Theme.All[index];
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);
        _drawable.Theme = _theme;

        UpdateButtons();
        BoardView.Invalidate();
    }

    private void UpdateButtons()
    {
        SetSelected(ArcherButton, _vm.IsArcherSelected);
        SetSelected(CannonButton, _vm.IsCannonSelected);
        SetSelected(MageButton, _vm.IsMageSelected);
        SetSelected(SellButton, _vm.IsSellMode);
    }

    // Valitud nupp saab rõhuvärvi
    private void SetSelected(Button button, bool selected)
    {
        button.BackgroundColor = selected
            ? _theme.AccentColor
            : _theme.CardColor;

        button.TextColor = selected
            ? _theme.BackgroundColor
            : _theme.TextColor;
    }
}