

namespace TARge25MAUI;

public partial class StartPage : ContentPage
{
    // Lehtede ja nuputekstide listid
    public List<ContentPage> Lehed = new List<ContentPage>() { new TextPage(), new FigurePage(), new TimerPage() };
    public List<string> LeheNimed = new List<string>() { "Tekst", "Kujund", "Taimer" };

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

        // Paneme kogu sisu keritavasse vaatesse (ScrollView)
        ScrollView sv = new ScrollView { Content = vst };

        // Määrame lehe põhisistuks ScrollView
        Content = sv;
    }
}
