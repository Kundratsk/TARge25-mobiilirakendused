using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;

namespace TARge25MAUI;

public partial class RgbColorPage : ContentPage
{
    BoxView colorBox;
    Slider sliderR, sliderG, sliderB;
    Stepper stepperRadius;
    Label lblR, lblG, lblB, lblRadius;
    Button btnRandom;

    public RgbColorPage()
    {
        InitializeComponent();

        Title = "RGB Värvimuutja";
        AbsoluteLayout layout = new AbsoluteLayout();

        colorBox = new BoxView { Color = Color.FromRgb(128, 128, 128), CornerRadius = 0 };
        AbsoluteLayout.SetLayoutBounds(colorBox, new Rect(0.5, 0.05, 0.8, 0.25));
        AbsoluteLayout.SetLayoutFlags(colorBox, AbsoluteLayoutFlags.All);

        lblR = new Label { Text = "R: 128", FontAttributes = FontAttributes.Bold, TextColor = Colors.Red };
        AbsoluteLayout.SetLayoutBounds(lblR, new Rect(0.1, 0.35, 0.2, 0.05));
        AbsoluteLayout.SetLayoutFlags(lblR, AbsoluteLayoutFlags.All);

        sliderR = new Slider { Minimum = 0, Maximum = 255, Value = 128 };
        sliderR.ValueChanged += ColorSlider_ValueChanged;
        AbsoluteLayout.SetLayoutBounds(sliderR, new Rect(0.9, 0.35, 0.7, 0.05));
        AbsoluteLayout.SetLayoutFlags(sliderR, AbsoluteLayoutFlags.All);

        lblG = new Label { Text = "G: 128", FontAttributes = FontAttributes.Bold, TextColor = Colors.Green };
        AbsoluteLayout.SetLayoutBounds(lblG, new Rect(0.1, 0.45, 0.2, 0.05));
        AbsoluteLayout.SetLayoutFlags(lblG, AbsoluteLayoutFlags.All);

        sliderG = new Slider { Minimum = 0, Maximum = 255, Value = 128 };
        sliderG.ValueChanged += ColorSlider_ValueChanged;
        AbsoluteLayout.SetLayoutBounds(sliderG, new Rect(0.9, 0.45, 0.7, 0.05));
        AbsoluteLayout.SetLayoutFlags(sliderG, AbsoluteLayoutFlags.All);

        lblB = new Label { Text = "B: 128", FontAttributes = FontAttributes.Bold, TextColor = Colors.Blue };
        AbsoluteLayout.SetLayoutBounds(lblB, new Rect(0.1, 0.55, 0.2, 0.05));
        AbsoluteLayout.SetLayoutFlags(lblB, AbsoluteLayoutFlags.All);

        sliderB = new Slider { Minimum = 0, Maximum = 255, Value = 128 };
        sliderB.ValueChanged += ColorSlider_ValueChanged;
        AbsoluteLayout.SetLayoutBounds(sliderB, new Rect(0.9, 0.55, 0.7, 0.05));
        AbsoluteLayout.SetLayoutFlags(sliderB, AbsoluteLayoutFlags.All);

        lblRadius = new Label { Text = "Ümarus: 0", FontAttributes = FontAttributes.Bold };
        AbsoluteLayout.SetLayoutBounds(lblRadius, new Rect(0.1, 0.68, 0.4, 0.05));
        AbsoluteLayout.SetLayoutFlags(lblRadius, AbsoluteLayoutFlags.All);

        stepperRadius = new Stepper { Minimum = 0, Maximum = 100, Increment = 5, Value = 0 };
        stepperRadius.ValueChanged += StepperRadius_ValueChanged;
        AbsoluteLayout.SetLayoutBounds(stepperRadius, new Rect(0.85, 0.68, 0.4, 0.05));
        AbsoluteLayout.SetLayoutFlags(stepperRadius, AbsoluteLayoutFlags.All);

        btnRandom = new Button { Text = "Juhuslik värv (Animatsioon)", BackgroundColor = Colors.DarkOrange, TextColor = Colors.White };
        btnRandom.Clicked += BtnRandom_Clicked;
        AbsoluteLayout.SetLayoutBounds(btnRandom, new Rect(0.5, 0.85, 0.8, 0.08));
        AbsoluteLayout.SetLayoutFlags(btnRandom, AbsoluteLayoutFlags.All);

        layout.Children.Add(colorBox);
        layout.Children.Add(lblR); layout.Children.Add(sliderR);
        layout.Children.Add(lblG); layout.Children.Add(sliderG);
        layout.Children.Add(lblB); layout.Children.Add(sliderB);
        layout.Children.Add(lblRadius); layout.Children.Add(stepperRadius);
        layout.Children.Add(btnRandom);

        Content = layout;
    }

    private void ColorSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int r = Convert.ToInt32(sliderR.Value);
        int g = Convert.ToInt32(sliderG.Value);
        int b = Convert.ToInt32(sliderB.Value);

        lblR.Text = $"R: {r}";
        lblG.Text = $"G: {g}";
        lblB.Text = $"B: {b}";

        colorBox.Color = Color.FromRgb(r, g, b);
    }

    private void StepperRadius_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int radius = Convert.ToInt32(e.NewValue);
        lblRadius.Text = $"Ümarus: {radius}";
        colorBox.CornerRadius = radius;
    }

    private async void BtnRandom_Clicked(object sender, EventArgs e)
    {
        Random rand = new Random();
        int targetR = rand.Next(0, 256);
        int targetG = rand.Next(0, 256);
        int targetB = rand.Next(0, 256);

        int steps = 10;
        double stepR = (targetR - sliderR.Value) / steps;
        double stepG = (targetG - sliderG.Value) / steps;
        double stepB = (targetB - sliderB.Value) / steps;

        for (int i = 0; i < steps; i++)
        {
            sliderR.Value += stepR;
            sliderG.Value += stepG;
            sliderB.Value += stepB;
            await Task.Delay(25);
        }

        sliderR.Value = targetR;
        sliderG.Value = targetG;
        sliderB.Value = targetB;
    }
}
