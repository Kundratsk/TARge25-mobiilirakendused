using Microsoft.Maui.Controls;
using System;
using TripsTrapsTrull;

namespace TripsTrapsTrull
{
    public class RulesPage : ContentPage
    {
        private GameLogic _game;

        public RulesPage(GameLogic game)
        {
            _game = game;
            Title = "Trips-Traps-Trull: Reeglid";
            BackgroundColor = Colors.FloralWhite;

            var layout = new VerticalStackLayout
            {
                Padding = 30,
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var rulesHeader = new Label
            {
                Text = "Tere tulemast mängima!",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            var rulesText = new Label
            {
                Text = "Mängureeglid:\n\n" +
                       "1. Mängitakse 3x3 ruudustikul.\n" +
                       "2. Mängijad panevad kordamööda oma sümboleid (X ja O).\n" +
                       "3. Võidab see, kes saab esimesena kolm oma sümbolit ritta, veergu või diagonaali.\n" +
                       "4. Kui laud saab täis ja keegi ei võida, on tulemuseks viik.",
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start
            };

            // Nupp edasiliikumiseks
            var btnNext = new Button
            {
                Text = "Edasi",
                BackgroundColor = Colors.DodgerBlue,
                TextColor = Colors.White,
                WidthRequest = 200,
                Margin = new Thickness(0, 20, 0, 0)
            };
            btnNext.Clicked += BtnNext_Clicked;

            layout.Add(rulesHeader);
            layout.Add(rulesText);
            layout.Add(btnNext);

            Content = layout;
        }

        private async void BtnNext_Clicked(object sender, EventArgs e)
        {
            // Küsime kasutajalt alustajat
            string action = await DisplayActionSheet("Kes alustab mängu?", "Tühista", null, "Mängija X", "Mängija O", "Juhuslik");

            if (action == "Tühista" || string.IsNullOrEmpty(action)) return;

            if (action == "Mängija X") _game.SetStartingPlayer("X");
            else if (action == "Mängija O") _game.SetStartingPlayer("O");
            else if (action == "Juhuslik") _game.SetStartingPlayer(new Random().Next(0, 2) == 0 ? "X" : "O");

            _game.ResetGame();

            // Navigeerime peamisele mängulehele ja eemaldame reeglite lehe ajaloost, 
            // et mängu seest "tagasi" vajutades rakendus ei sulguks ega hüppaks uuesti reeglitesse
            var mainPage = new MainPage(_game);
            await Navigation.PushAsync(mainPage);

            // Valikuline: eemaldab reeglite lehe pinust, et mängija jääks otse mängu vaatesse
            Navigation.RemovePage(this);
        }
    }
}