using Microsoft.Maui.Controls;

namespace TreeProject;

public partial class MainPage : ContentPage
{
    // Määrame värvid: tavaline roheline ja ülimalt mahe, naturaalne heleroosa viljapuu õite toon
    private readonly Color _summerGreen = Color.FromArgb("#2E7D32");
    private readonly Color _naturalBlossomPink = Color.FromArgb("#FBCFE8");

    public MainPage()
    {
        InitializeComponent();

        // Algväärtustame kuupäeva ja värskendame tausta vastavalt sellele
        SeasonDatePicker.Date = DateTime.Today;
        UpdateSkyBackground();

        SeasonDatePicker.DateSelected += (s, e) => UpdateSkyBackground();
        DayTimePicker.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(TimePicker.Time)) UpdateSkyBackground();
        };
    }

    // ===================== PÕHITEGEVUSE KÄIVITAJA =====================
    private async void OnActionClicked(object sender, EventArgs e)
    {
        string action = ActionPicker.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(action))
        {
            InfoLabel.Text = "Palun vali tegevus!";
            return;
        }

        RunButton.IsEnabled = false;
        uint speed = (uint)SpeedStepper.Value;

        switch (action)
        {
            case "Kasva":
                InfoLabel.Text = "Puu kasvab...";
                await GrowTree(speed);
                InfoLabel.Text = "Puu on kasvanud!";
                break;

            case "Õitse":
                InfoLabel.Text = "Puu hakkab õitsema...";
                await BloomTree(speed);
                InfoLabel.Text = "Puu kattus mahedate kevadõitega ja vanad viljad kukkusid maha!";
                break;

            case "Värise":
                InfoLabel.Text = "Puu väriseb tuules...";
                await ShakeTree(speed);
                InfoLabel.Text = "Tuul rahunes.";
                break;

            case "Langeta":
                await TryChopTree(speed);
                break;
        }

        RunButton.IsEnabled = true;
    }

    // ===================== ANIMATSIOONID =====================

    private async Task GrowTree(uint speed)
    {
        await TreeGroup.ScaleTo(1.25, speed, Easing.CubicOut);
    }

    private async Task BloomTree(uint speed)
    {
        if (LeavesFrame != null)
        {
            // UUS: Paneme kõik õunad korraga ja paralleelselt maha kukkuma enne õite puhkemist
            var appleDropTasks = new List<Task>
            {
                DropApple(Apple1),
                DropApple(Apple2),
                DropApple(Apple3)
            };

            // Hajutame sujuvalt lehestiku korraks pehmemaks ja vahetame värvi naturaalse roosa vastu
            var bloomTasks = new List<Task>
            {
                LeavesFrame.FadeTo(0.5, speed / 2),
                Task.WhenAll(appleDropTasks) // Ootame ka õunte kukkumise ära
            };

            await Task.WhenAll(bloomTasks);

            LeavesFrame.BackgroundColor = _naturalBlossomPink;
            await LeavesFrame.FadeTo((float)OpacitySlider.Value, speed / 2);
        }
    }

    private async Task ShakeTree(uint speed)
    {
        for (int i = 0; i < 3; i++)
        {
            await TreeGroup.TranslateTo(-12, 0, speed / 6, Easing.SinIn);
            await TreeGroup.TranslateTo(12, 0, speed / 6, Easing.SinOut);
        }
        await TreeGroup.TranslateTo(0, 0, speed / 6, Easing.CubicOut);
    }

    private async Task TryChopTree(uint speed)
    {
        int month = SeasonDatePicker.Date?.Month ?? 6;
        int hour = DayTimePicker.Time?.Hours ?? 12;

        bool isWinter = month == 12 || month == 1 || month == 2;
        bool isDaylight = hour >= 8 && hour <= 17;

        if (!isWinter || !isDaylight)
        {
            InfoLabel.Text = "Pimedas ja suvel puid ei langetata! (Vali talv ja päevaaeg)";
            return;
        }

        InfoLabel.Text = "Puud langetatakse...";
        await TreeGroup.RotateTo(90, speed, Easing.CubicIn);
        InfoLabel.Text = "Puu on langetatud.";
    }

    // ===================== SEOTUD KONTROLLID =====================

    private void OnOpacitySliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (LeavesFrame != null)
        {
            LeavesFrame.Opacity = e.NewValue;
        }
    }

    private void OnSpeedStepperChanged(object sender, ValueChangedEventArgs e)
    {
        if (SpeedLabel != null)
        {
            SpeedLabel.Text = $"{(int)e.NewValue} ms";
        }
    }

    private void UpdateSkyBackground()
    {
        if (TreeCanvas == null) return;

        int hour = DayTimePicker.Time?.Hours ?? 12;
        int month = SeasonDatePicker.Date?.Month ?? 6;

        if (hour < 7 || hour > 19)
        {
            TreeCanvas.BackgroundColor = Color.FromArgb("#1E293B");
            InfoLabel.TextColor = Colors.LightGray;
        }
        else
        {
            if (month == 12 || month == 1 || month == 2)
            {
                TreeCanvas.BackgroundColor = Color.FromArgb("#E2E8F0");
            }
            else if (month >= 9 && month <= 11)
            {
                TreeCanvas.BackgroundColor = Color.FromArgb("#CBD5E1");
            }
            else
            {
                TreeCanvas.BackgroundColor = Color.FromArgb("#E0F2FE");
            }
            InfoLabel.TextColor = Color.FromArgb("#5A6A85");
        }
    }

    // ===================== ÕUNAD =====================

    private async void OnAppleTapped(object sender, TappedEventArgs e)
    {
        if (sender is not View apple) return;
        await DropApple(apple);
    }

    // UUS ABI-FUNKTSIOON: Tegeleb ühe õuna kukkumise ja peitmisega
    private async Task DropApple(View apple)
    {
        if (apple == null || !apple.IsVisible) return;

        // Eemaldame klikitavuse, et kukkumise ajal topelt vajutada ei saaks
        apple.GestureRecognizers.Clear();

        // Õun kukub maapinnale
        await apple.TranslateTo(0, 240, 700, Easing.CubicIn);

        // Väike põrge maas olles
        await apple.TranslateTo(0, 230, 100, Easing.CubicOut);
        await apple.TranslateTo(0, 240, 100, Easing.CubicIn);

        // Hajub sujuvalt ära ja peidetakse ekraanilt
        await apple.FadeTo(0, 300);
        apple.IsVisible = false;
    }

    // ===================== RESET =====================

    private async void OnResetClicked(object sender, EventArgs e)
    {
        RunButton.IsEnabled = false;
        ResetButton.IsEnabled = false;

        var resetTasks = new List<Task>
        {
            TreeGroup.RotateTo(0, 400, Easing.CubicOut),
            TreeGroup.TranslateTo(0, 0, 300, Easing.CubicOut),
            TreeGroup.ScaleTo(1.0, 300, Easing.CubicOut),
            LeavesFrame.FadeTo(1, 300)
        };

        await Task.WhenAll(resetTasks);

        // Muudame lehestiku tagasi suviseks roheliseks
        LeavesFrame.BackgroundColor = _summerGreen;
        OpacitySlider.Value = 1;

        RestoreApple(Apple1);
        RestoreApple(Apple2);
        RestoreApple(Apple3);

        InfoLabel.Text = "Puu on lähtestatud.";

        RunButton.IsEnabled = true;
        ResetButton.IsEnabled = true;
    }

    private void RestoreApple(View apple)
    {
        if (apple == null) return;

        apple.TranslationX = 0;
        apple.TranslationY = 0;
        apple.Opacity = 1;
        apple.IsVisible = true;

        if (apple.GestureRecognizers.Count == 0)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += OnAppleTapped;
            apple.GestureRecognizers.Add(tap);
        }
    }
}
