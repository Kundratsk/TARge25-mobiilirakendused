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

        // ViewModel on kogu mängu "aju"; leht ainult näitab seda
        _vm = new MainViewModel();
        BindingContext = _vm;

        _drawable = new GameDrawable { Game = _vm.Game, Theme = _theme };
        BoardView.Drawable = _drawable;

        // Reageerime ViewModeli sündmustele
        _vm.Ticked += OnTicked;
        _vm.ShotFired += OnShotFired;
        _vm.GameReset += () => _drawable.Shots.Clear();
        _vm.RedrawRequested += () => BoardView.Invalidate();

        // Kui valitud torn või müügirežiim muutub, värvime nupud uuesti
        _vm.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedType) ||
                e.PropertyName == nameof(MainViewModel.IsSellMode))
            {
                UpdateButtons();
            }
        };

        ApplyTheme();

        // Aken on kättesaadav alles pärast lehe laadimist
        Loaded += OnPageLoaded;
    }

    // ---------- Elutsükkel ----------

    // Kutsutakse iga kord, kui leht nähtavale tuleb (ka seadete lehelt tagasi tulles)
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Seadetes võis teema muutuda
        _theme = LoadSavedTheme();
        ApplyTheme();

        // Seadetes võis keel muutuda: värskendame olekurea teksti
        if (_shownLanguage != LocalizationService.CurrentLanguage)
        {
            _shownLanguage = LocalizationService.CurrentLanguage;
            _vm.StatusText = _vm.IsSellMode
                ? LocalizationService.Get("SellModeOn")
                : LocalizationService.Get("StartHint");
        }
    }

    // ---------- Akna sündmused (salvestamine) ----------

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        if (_windowHooked || Window == null) return;
        _windowHooked = true;

        // Salvestame, kui aken kaotab fookuse, rakendus läheb taustale või aken suletakse
        Window.Deactivated += (_, _) => _vm.SaveProgress();
        Window.Stopped += (_, _) => _vm.SaveProgress();
        Window.Destroying += (_, _) => _vm.SaveProgress();
    }

    // ---------- ViewModeli sündmused ----------

    private void OnTicked(double dt)
    {
        // Laskujoonte eluiga väheneb ja lõppenud eemaldatakse
        for (int i = _drawable.Shots.Count - 1; i >= 0; i--)
        {
            _drawable.Shots[i].Ttl -= dt;
            if (_drawable.Shots[i].Ttl <= 0)
                _drawable.Shots.RemoveAt(i);
        }
    }

    private void OnShotFired(Tower tower, Enemy enemy)
    {
        _drawable.Shots.Add(new Shot
        {
            X1 = tower.Column + 0.5f,
            Y1 = tower.Row + 0.5f,
            X2 = (float)enemy.X + 0.5f,
            Y2 = (float)enemy.Y + 0.5f
        });
    }

    // ---------- UI sündmused ----------

    private void OnBoardTapped(object? sender, TappedEventArgs e)
    {
        Point? p = e.GetPosition(BoardView);
        if (p == null || _drawable.CellSize <= 0) return;

        int column = (int)Math.Floor((p.Value.X - _drawable.OffsetX) / _drawable.CellSize);
        int row = (int)Math.Floor((p.Value.Y - _drawable.OffsetY) / _drawable.CellSize);

        // Ruudu arvutab leht (ta teab joonistuse mõõtmeid), ülejäänu teeb ViewModel
        _vm.TapCell(column, row);
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        // Seadete ajaks panen mängu pausile, et vaenlased ei jookseks edasi
        if (_vm.Game.State == Models.GameState.Running)
            _vm.StartPauseCommand.Execute(null);

        await Navigation.PushAsync(new SettingsPage());
    }

    // ---------- Abimeetodid ----------

    private static Theme LoadSavedTheme()
    {
        int index = Math.Clamp(
            SettingsService.Current.ThemeIndex, 0, Theme.All.Count - 1);
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

    // Valitud nupp saab rõhuvärvi, ülejäänud kaardivärvi
    private void SetSelected(Button button, bool selected)
    {
        button.BackgroundColor = selected ? _theme.AccentColor : _theme.CardColor;
        button.TextColor = selected ? _theme.BackgroundColor : _theme.TextColor;
    }
}