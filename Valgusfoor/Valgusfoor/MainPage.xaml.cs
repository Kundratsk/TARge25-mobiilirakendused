namespace Valgusfoor;

public partial class MainPage : ContentPage
{
    bool trafficLightOn = false;
    private bool _isAutomaticActive = false;
    public MainPage()
    {
        InitializeComponent();
    }

    // SISSE
    private async void OnOnClicked(object? sender, EventArgs e)
    {
        trafficLightOn = true;

        StatusLabel.Text = "Vali valgus";

        RedLight.BackgroundColor = Colors.Red;
        YellowLight.BackgroundColor = Colors.Yellow;
        GreenLight.BackgroundColor = Colors.Green;

        // Animatsioon
        RedLight.Opacity = 0;
        YellowLight.Opacity = 0;
        GreenLight.Opacity = 0;

        await RedLight.FadeTo(1, 300);
        await YellowLight.FadeTo(1, 300);
        await GreenLight.FadeTo(1, 300);
    }

    // VÄLJA
    private async void OnOffClicked(object? sender, EventArgs e)
    {
        trafficLightOn = false;

        StatusLabel.Text = "Lülita  foor sisse";

        await Task.WhenAll(
            RedLight.FadeTo(0, 200),
            YellowLight.FadeTo(0, 200),
            GreenLight.FadeTo(0, 200)
        );

        RedLight.BackgroundColor = Colors.Gray;
        YellowLight.BackgroundColor = Colors.Gray;
        GreenLight.BackgroundColor = Colors.Gray;

        RedLight.Opacity = 1;
        YellowLight.Opacity = 1;
        GreenLight.Opacity = 1;
    }

    // PUNANE
    private void OnRedClicked(object? sender, TappedEventArgs e)
    {
        if (!trafficLightOn)
        {
            StatusLabel.Text = "Lülita foor sisse";
            return;
        }

        StatusLabel.Text = "Seisa";
    }

    // KOLLANE
    private void OnYellowClicked(object? sender, TappedEventArgs e)
    {
        if (!trafficLightOn)
        {
            StatusLabel.Text = "Lülita foor sisse";
            return;
        }

        StatusLabel.Text = "Valmistu sõitma";
    }

    // ROHELINE
    private void OnGreenClicked(object? sender, TappedEventArgs e)
    {
        if (!trafficLightOn)
        {
            StatusLabel.Text = "Lülita foor sisse";
            return;
        }

        StatusLabel.Text = "Sõida";
    }

    
    private async void AutomaatRegime(object sender, EventArgs e)
    {
        // 1. Kui automaatrežiim JUBA TÖÖTAB, siis lülitame selle VÄLJA
        if (_isAutomaticActive)
        {
            _isAutomaticActive = false;

            if (sender is Button nupp)
            {
                nupp.Text = "Automaatreziim";
            }

            StatusLabel.Text = "Automaatrežiim peatatud";

            // Teeme tuled uuesti halliks, kui režiim peatatakse
            RedLight.BackgroundColor = Colors.Gray;
            YellowLight.BackgroundColor = Colors.Gray;
            GreenLight.BackgroundColor = Colors.Gray;
            return;
        }

        // 2. Kui režiim EI TÖÖTANUD, lülitame SISSE
        _isAutomaticActive = true;

        if (sender is Button sisseNupp)
        {
            sisseNupp.Text = "Lülita välja";
        }

        // Lõputu tsükkel foori tuledega
        while (_isAutomaticActive)
        {
            // PUNANE TULI
            StatusLabel.Text = "Punane tuli";
            RedLight.BackgroundColor = Colors.Red;
            YellowLight.BackgroundColor = Colors.Gray;
            GreenLight.BackgroundColor = Colors.Gray;
            await Task.Delay(3000);

            if (!_isAutomaticActive) break;

            // KOLLANE TULI
            StatusLabel.Text = "Valmistu sõitma";
            RedLight.BackgroundColor = Colors.Gray;
            YellowLight.BackgroundColor = Colors.Yellow;
            GreenLight.BackgroundColor = Colors.Gray;
            await Task.Delay(1500);

            if (!_isAutomaticActive) break;

            // ROHELINE TULI
            StatusLabel.Text = "Sõida";
            RedLight.BackgroundColor = Colors.Gray;
            YellowLight.BackgroundColor = Colors.Gray;
            GreenLight.BackgroundColor = Colors.Green;
            await Task.Delay(4000);
        }
    }

}



