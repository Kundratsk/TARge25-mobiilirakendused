using Microsoft.Maui.Controls;
using System;
using TripsTrapsTrull;

namespace TripsTrapsTrull
{
    public class MainPage : ContentPage
    {
        private GameLogic _game;
        private Grid _grid;
        private Label _statusLabel;

        // Võtame loodud mänguloogika läbi konstruktori vastu
        public MainPage(GameLogic game)
        {
            _game = game;
            Title = "Mänguväli";
            BackgroundColor = Colors.GhostWhite;

            var mainLayout = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _statusLabel = new Label
            {
                Text = $"Mängija {_game.CurrentPlayer} alustab!",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.DarkSlateGray
            };
            mainLayout.Add(_statusLabel);

            _grid = new Grid
            {
                HeightRequest = 450,
                WidthRequest = 450,
                BackgroundColor = Colors.LightGray,
                Padding = 5,
                RowSpacing = 5,
                ColumnSpacing = 5
            };

            for (int i = 0; i < 3; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            CreateButtons();
            mainLayout.Add(_grid);

            var buttonLayout = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };

            var btnReset = new Button { Text = "Uus mäng", BackgroundColor = Colors.CornflowerBlue, TextColor = Colors.White };
            btnReset.Clicked += (s, e) => RestartGame();

            var btnWhoStarts = new Button { Text = "Kes alustab?", BackgroundColor = Colors.Crimson, TextColor = Colors.White };
            btnWhoStarts.Clicked += BtnWhoStarts_Clicked;

            // Muudetud nupp: avab hüpikaknana praeguse mängu statistika
            var btnStats = new Button { Text = "Statistika", BackgroundColor = Colors.Gray, TextColor = Colors.White };
            btnStats.Clicked += async (s, e) => {
                await DisplayAlert("Mängu statistika",
                    $"X võidud: {_game.WinsX}\n" +
                    $"O võidud: {_game.WinsO}\n" +
                    $"Viigid: {_game.Draws}", "Sulge");
            };

            buttonLayout.Add(btnReset);
            buttonLayout.Add(btnWhoStarts);
            buttonLayout.Add(btnStats);

            mainLayout.Add(buttonLayout);
            Content = mainLayout;
        }

        private void CreateButtons()
        {
            _grid.Children.Clear();
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var button = new Button
                    {
                        Text = "",
                        FontSize = 32,
                        FontAttributes = FontAttributes.Bold,
                        BackgroundColor = Colors.White,
                        TextColor = Colors.Black
                    };

                    int row = r;
                    int col = c;
                    button.Clicked += (s, e) => MakeMove(button, row, col);

                    _grid.Add(button, col, row);
                }
            }
        }

        private async void MakeMove(Button btn, int row, int col)
        {
            if (_game.MakeMove(row, col))
            {
                btn.Text = _game.CurrentPlayer;
                btn.TextColor = _game.CurrentPlayer == "X" ? Colors.Crimson : Colors.DarkTurquoise;

                string result = _game.CheckWinner();
                if (result != null)
                {
                    if (result == "Viik")
                    {
                        _game.Draws++;
                        await DisplayAlert("Mäng läbi", "Mäng jäi viiki!", "Uus mäng");
                    }
                    else
                    {
                        if (result == "X") _game.WinsX++; else _game.WinsO++;
                        await DisplayAlert("Võitja!", $"{result} võitis! Kas soovid veel mängida?", "Jah");
                    }
                    RestartGame();
                }
                else
                {
                    _game.TogglePlayer();
                    _statusLabel.Text = $"Mängija {_game.CurrentPlayer} kord";
                }
            }
        }

        private void RestartGame()
        {
            _game.ResetGame();
            CreateButtons();
            _statusLabel.Text = $"Mängija {_game.CurrentPlayer} kord";
        }

        private async void BtnWhoStarts_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Vali alustaja:", "Tühista", null, "Mängija X", "Mängija O", "Juhuslik");

            if (action == "Mängija X") _game.SetStartingPlayer("X");
            else if (action == "Mängija O") _game.SetStartingPlayer("O");
            else if (action == "Juhuslik") _game.SetStartingPlayer(new Random().Next(0, 2) == 0 ? "X" : "O");

            RestartGame();
        }
    }
}
