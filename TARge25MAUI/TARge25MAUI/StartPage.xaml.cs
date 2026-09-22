using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace TARge25MAUI;

public partial class StartPage : ContentPage
{
    // Lehtede ja nuputekstide listid - LISATUD: DateTimePage, StepperSliderPage, RgbColorPage
    public List<ContentPage> Lehed = new List<ContentPage>()
    {
        new TextPage(),
        new FigurePage(),
        new TimerPage(),
        new DateTimePage(),
        new StepperSliderPage(),
        new RgbColorPage(), // RGB värvimuutja ülesanne
        new Pop_Up_Page(),
        new GridPage()
    };

    public List<string> LeheNimed = new List<string>()
    {
        "Tekst",
        "Kujund",
        "Taimer",
        "Kuupaev ja Aeg",
        "Stepper ja Slider",
        "RGB Varvimuutja",
        "Pop_Up_Page näidis",
        "Grid"
    };

    public StartPage()
    {
        // Loome vertikaalse paigutuse nupude jaoks
        VerticalStackLayout vst = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 10
        };

        // Tsükkel nupude dünaamiliseks loomiseks
        for (int i = 0; i < Lehed.Count; i++)
        {
            // Kriitiline samm: teeme tsükli muutujast kohaliku koopia, 
            // et nupu klikkimisel avataks õige leht
            int index = i;

            Button nupp = new Button
            {
                Text = LeheNimed[index],
                FontSize = 36,
                FontFamily = "Socafe",
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 60
            };

            // Lisame nupule klikkimise sündmuse
            nupp.Clicked += async (sender, e) =>
            {
                // Võtame õige lehe salvestatud indeksi järgi
                var valik = Lehed[index];

                // Navigeerime valitud lehele
                await Navigation.PushAsync(valik);
            };

            // Lisame loodud nupu paigutusse
            vst.Add(nupp);
        }

        // ==========================================
        // LISATUD: Punane testnupp pärast for-tsüklit
        // ==========================================
        Button nulliNupp = new Button
        {
            Text = "Nulli seaded (Testimiseks)",
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 50,
            Margin = new Thickness(0, 30, 0, 0) // Jätame veidi tühja ruumi üles
        };

        // Mis juhtub nupule vajutades?
        nulliNupp.Clicked += async (sender, e) =>
        {
            // Kustutame seadme mälust meie spetsiifilise võtme
            Preferences.Default.Remove("EsimeneKäivitamine");

            // Anname tagasisidet, et nullimine õnnestus
            await DisplayAlertAsync("Edukalt nullitud", "Mälu on tühjendatud. Kui sa lehe uuesti avad, käitub äpp nagu täiesti uus!", "OK");
        };

        // Lisame testnupu samasse vst paigutusse
        vst.Add(nulliNupp);
        // ==========================================

        // Paneme kogu sisu keritavasse vaatesse (ScrollView)
        ScrollView sv = new ScrollView { Content = vst };

        // Määrame lehe põhisistuks ScrollView
        Content = sv;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Loeme seadme mälust muutuja "EsimeneKäivitamine".
        bool onEsimeneStart = Preferences.Default.Get("EsimeneKäivitamine", true);

        // 2. Kui on esimene start, kuvame dialoogiakna
        if (onEsimeneStart)
        {
            bool vastus = await DisplayAlertAsync("Tere tulemast!",
                "Tundub, et avasid selle rakenduse esimest korda. Kas soovid näha lühikest juhendit?",
                "Jah, palun",
                "Ei, saan ise hakkama");

            if (vastus)
            {
                await DisplayAlertAsync("Juhend",
                    "Siin on sinu lühike juhend: vali menüüst sobiv teema ja uuri, kuidas elemendid töötavad!",
                    "Selge");
            }

            // 3. Salvestame info, et esimene käivitamine on tehtud.
            Preferences.Default.Set("EsimeneKäivitamine", false);
        }
    }
}
