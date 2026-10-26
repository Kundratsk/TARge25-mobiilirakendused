using TowerDefense.Models;
using TowerDefense.Themes;
using TowerDefense.ViewModels;

namespace TowerDefense;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;
    private readonly GameDrawable _drawable;
    private Theme _theme = Theme.All[1];   // vaikimisi "Tume"

    public MainPage()
    {
        InitializeComponent();

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

        ThemePicker.ItemsSource = Theme.All.Select(t => t.Name).ToList();
        ThemePicker.SelectedIndex = 1;   // käivitab OnThemeChanged
        ApplyTheme();
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

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        if (ThemePicker.SelectedIndex < 0) return;

        _theme = Theme.All[ThemePicker.SelectedIndex];
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);
        _drawable.Theme = _theme;
        BoardView.Invalidate();
    }
}