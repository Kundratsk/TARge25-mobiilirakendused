using LanguageGallery.Models;
using LanguageGallery.Services;
using LanguageGallery.ViewModels;

namespace LanguageGallery;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm = new();
    private readonly IDispatcherTimer _timer;
    private static LocalizationResourceManager Loc => LocalizationResourceManager.Instance;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = _vm;

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(4);
        _timer.Tick += (_, _) =>
        {
            if (_vm.Cards.Count > 0)
                Carousel.Position = (Carousel.Position + 1) % _vm.Cards.Count;
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _timer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer.Stop();
    }

    private async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not VisualElement view || view.BindingContext is not LanguageCard card)
            return;

        _timer.Stop(); // kerimine seisab, kuni alert on lahti

        await view.ScaleTo(0.94, 80);
        await view.ScaleTo(1.0, 80);

        await DisplayAlert(card.Name, card.Info, Loc["Close"]);

        _timer.Start();
    }

    private void OnLanguageClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string code })
        {
            Loc.SetCulture(code);
            _vm.RefreshCards();
        }
    }
}