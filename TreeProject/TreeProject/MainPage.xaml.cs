using Microsoft.Maui.Controls;

namespace TreeProject;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        SeasonDatePicker.Date = DateTime.Today;
    }

    // ===================== PÕHITEGEVUSE KÄIVITAJA =====================
    private async void OnActionClicked(object sender, EventArgs e)
    {
        RunButton.IsEnabled = false;

        string action = ActionPicker.SelectedItem?.ToString();
        uint speed = (uint)SpeedStepper.Value;

        switch (action)
        {
            case "Kasva":
                InfoLabel.Text = "Puu kasvab...";
                await GrowTree(speed);
                InfoLabel.Text = "Puu on kasvanud!";
                break;

            case "Õitse":
                InfoLabel.Text = "Puu õitseb...";
                await BloomTree(speed);
                InfoLabel.Text = "Puu õitseb!";
                break;

            case "Värise":
                InfoLabel.Text = "Puu väriseb tuules...";
                await ShakeTree(speed);
                InfoLabel.Text = "Tuul rahunes.";
                break;

            case "Langeta":
                await TryChopTree(speed);
                break;

            default:
                InfoLabel.Text = "Palun vali tegevus!";
                break;
        }

        RunButton.IsEnabled = true;
    }

    // ===================== ANIMATSIOONID =====================

    private async Task GrowTree(uint speed)
    {
        await TreeGroup.ScaleTo(1.3, speed);
    }

    private async Task BloomTree(uint speed)
    {
        await LeavesFrame.FadeTo(0.3, speed / 2);
        LeavesFrame.BackgroundColor = Colors.HotPink;
        await LeavesFrame.FadeTo((float)OpacitySlider.Value, speed / 2);
    }

    private async Task ShakeTree(uint speed)
    {
        for (int i = 0; i < 3; i++)
        {
            await TreeGroup.TranslateTo(-15, 0, speed / 6);
            await TreeGroup.TranslateTo(15, 0, speed / 6);
        }
        await TreeGroup.TranslateTo(0, 0, speed / 6);
    }

    private async Task TryChopTree(uint speed)
    {
        int month = SeasonDatePicker.Date.GetValueOrDefault(DateTime.Today).Month;
        int hour = DayTimePicker.Time.GetValueOrDefault().Hours;

        bool isWinter = month == 12 || month == 1 || month == 2;
        bool isDaylight = hour >= 8 && hour <= 17;

        if (!isWinter || !isDaylight)
        {
            InfoLabel.Text = "Pimedas ja suvel puid ei langetata!";
            return;
        }

        InfoLabel.Text = "Puud langetatakse...";
        await TreeGroup.RotateTo(90, speed);
        InfoLabel.Text = "Puu on langetatud.";
    }

    // ===================== SEOTUD KONTROLLID =====================

    private void OnOpacitySliderChanged(object sender, ValueChangedEventArgs e)
    {
        LeavesFrame.Opacity = e.NewValue;
    }

    private void OnSpeedStepperChanged(object sender, ValueChangedEventArgs e)
    {
        SpeedLabel.Text = $"{(int)e.NewValue} ms";
    }

    // ===================== LISAÜLESANNE: ÕUNAD =====================

    private async void OnAppleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not View apple) return;

        apple.GestureRecognizers.Clear();

        await apple.TranslateTo(0, 400, 800, Easing.CubicIn);
        apple.IsVisible = false;
    }
    // ===================== RESET =====================

    private async void OnResetClicked(object sender, EventArgs e)
    {
        RunButton.IsEnabled = false;
        ResetButton.IsEnabled = false;

        // Puu läheb tagasi püsti ja algsesse suurusesse/asendisse
        await TreeGroup.RotateTo(0, 400);
        await TreeGroup.TranslateTo(0, 0, 300);
        await TreeGroup.ScaleTo(1.0, 300);

        // Lehestik tagasi roheline ja täisnähtav
        LeavesFrame.BackgroundColor = Colors.Green;
        LeavesFrame.Opacity = 1;
        OpacitySlider.Value = 1;

        // Õunad tagasi nähtavale (kui olid ära klõpsatud)
        RestoreApple(Apple1, 60, 50);
        RestoreApple(Apple2, 140, 90);
        RestoreApple(Apple3, 30, 120);

        InfoLabel.Text = "Puu on lähtestatud.";

        RunButton.IsEnabled = true;
        ResetButton.IsEnabled = true;
    }

    private void RestoreApple(View apple, double x, double y)
    {
        apple.TranslationX = 0;
        apple.TranslationY = 0;
        apple.IsVisible = true;

        if (apple.GestureRecognizers.Count == 0)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += OnAppleTapped;
            apple.GestureRecognizers.Add(tap);
        }
    }
}