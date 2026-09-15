using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace TARge25MAUI;

public partial class DateTimePage : ContentPage
{
    Label lblInfo;
    DatePicker datePicker;
    TimePicker timePicker;

    public DateTimePage()
    {
        InitializeComponent();

        Title = "Kuupäev ja Aeg";
        AbsoluteLayout absoluteLayout = new AbsoluteLayout();

        lblInfo = new Label
        {
            Text = "Vali kuupäev ja aeg",
            FontSize = 20,
            HorizontalTextAlignment = TextAlignment.Center
        };

        datePicker = new DatePicker();
        datePicker.DateSelected += DateTimeChanged;

        timePicker = new TimePicker();
        timePicker.PropertyChanged += DateTimeChanged;

        AbsoluteLayout.SetLayoutBounds(lblInfo, new Rect(0.5, 0.1, 0.9, 0.1));
        AbsoluteLayout.SetLayoutFlags(lblInfo, AbsoluteLayoutFlags.All);

        AbsoluteLayout.SetLayoutBounds(datePicker, new Rect(0.5, 0.3, 0.8, 0.1));
        AbsoluteLayout.SetLayoutFlags(datePicker, AbsoluteLayoutFlags.All);

        AbsoluteLayout.SetLayoutBounds(timePicker, new Rect(0.5, 0.5, 0.8, 0.1));
        AbsoluteLayout.SetLayoutFlags(timePicker, AbsoluteLayoutFlags.All);

        absoluteLayout.Children.Add(lblInfo);
        absoluteLayout.Children.Add(datePicker);
        absoluteLayout.Children.Add(timePicker);

        Content = absoluteLayout;
    }

    private void DateTimeChanged(object sender, EventArgs e)
    {
        lblInfo.Text = $"Kuupäev: {datePicker.Date:dd.MM.yyyy}\nAeg: {timePicker.Time}";
    }
}
