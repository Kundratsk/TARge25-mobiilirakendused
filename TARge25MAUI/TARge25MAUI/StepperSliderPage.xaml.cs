using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace TARge25MAUI;

public partial class StepperSliderPage : ContentPage
{
    Grid spinningContainer;
    BoxView spinningBox;
    Label lblDegrees;
    Slider rotationSlider;

    public StepperSliderPage()
    {
        InitializeComponent();

        Title = "Kasti pööramine Slideriga";
        AbsoluteLayout layout = new AbsoluteLayout();

        // 1. Loome konteineri (Grid), mis hakkab tervikuna pöörlema
        spinningContainer = new Grid
        {
            WidthRequest = 150,
            HeightRequest = 150
        };

        // Loome kasti, mis on konteineri taustaks
        spinningBox = new BoxView
        {
            Color = Colors.DarkSlateBlue,
            CornerRadius = 15
        };

        // Loome sildi kasti sisse, mis näitab kraade
        lblDegrees = new Label
        {
            Text = "0°",
            TextColor = Colors.White,
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Lisame kasti ja teksti koos konteinerisse (nad asuvad üksteise peal)
        spinningContainer.Children.Add(spinningBox);
        spinningContainer.Children.Add(lblDegrees);

        // Paigutame pöörleva kasti ekraani ülaossa (keskele)
        AbsoluteLayout.SetLayoutBounds(spinningContainer, new Rect(0.5, 0.2, 150, 150));
        AbsoluteLayout.SetLayoutFlags(spinningContainer, AbsoluteLayoutFlags.PositionProportional);

        // 2. Loome Slideri pööramiseks (0 kuni 360 kraadi)
        rotationSlider = new Slider
        {
            Minimum = 0,
            Maximum = 360,
            Value = 0
        };

        // Sündmus: Kui sliderit liigutatakse
        rotationSlider.ValueChanged += (s, e) =>
        {
            // Teisendame murdarvu täisarvuks kraadide jaoks
            int degrees = Convert.ToInt32(e.NewValue);

            // Pööramise funktsioon: Määrame konteinerile uue pöördenurga
            spinningContainer.Rotation = degrees;

            // Uuendame teksti kasti sees
            lblDegrees.Text = $"{degrees}°";
        };

        // Paigutame Slideri ekraani keskossa
        AbsoluteLayout.SetLayoutBounds(rotationSlider, new Rect(0.5, 0.6, 0.8, 0.1));
        AbsoluteLayout.SetLayoutFlags(rotationSlider, AbsoluteLayoutFlags.All);

        // 3. Lisame elemendid lehele
        layout.Children.Add(spinningContainer);
        layout.Children.Add(rotationSlider);

        Content = layout;
    }
}
