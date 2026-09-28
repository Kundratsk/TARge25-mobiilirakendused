using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TripsTrapsTrull;

namespace TripsTrapsTrull
{
    public class MainPage : ContentPage
    {
        private GameLogic _game;
        private Grid _grid;
        private Label _statusLabel;
        private int _gridSize = 3;
        private ToolbarItem _playerTurnToolbarItem;

        // 1. UUS: Kahemõõtmeline massiiv nuppude hoidmiseks, et arvuti saaks nupud üles leida
        private Button[,] _buttons;

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

            CreateButtons();
            mainLayout.Add(_grid);

            var buttonLayout = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };

            var btnReset = new Button { Text = "Uus mäng", BackgroundColor = Colors.CornflowerBlue, TextColor = Colors.White };
            btnReset.Clicked += (s, e) => RestartGame();

            var btnWhoStarts = new Button { Text = "Kes alustab?", BackgroundColor = Colors.Crimson, TextColor = Colors.White };
            btnWhoStarts.Clicked += BtnWhoStarts_Clicked;

            var btnSize = new Button { Text = "Suurus", BackgroundColor = Colors.Purple, TextColor = Colors.White };
            btnSize.Clicked += BtnSize_Clicked;

            var btnStats = new Button { Text = "Statistika", BackgroundColor = Colors.Gray, TextColor = Colors.White };
            btnStats.Clicked += async (s, e) => {
                await DisplayAlert("Mängu statistika",
                    $"X võidud: {_game.WinsX}\n" +
                    $"O võidud: {_game.WinsO}\n" +
                    $"Viigid: {_game.Draws}", "Sulge");
            };

            // Loome paremale üles nurka elemendi
            _playerTurnToolbarItem = new ToolbarItem
            {
                Text = $"Kord: {_game.CurrentPlayer}",
                Priority = 0,
                Order = ToolbarItemOrder.Primary
            };
            ToolbarItems.Add(_playerTurnToolbarItem);

            buttonLayout.Add(btnReset);
            buttonLayout.Add(btnWhoStarts);
            buttonLayout.Add(btnSize);
            buttonLayout.Add(btnStats);

            mainLayout.Add(buttonLayout);
            Content = mainLayout;
        }

        private void CreateButtons()
        {
            _grid.Children.Clear();
            _grid.RowDefinitions.Clear();
            _grid.ColumnDefinitions.Clear();

            // Algatame nuppude massiivi uue suurusega
            _buttons = new Button[_gridSize, _gridSize];

            for (int i = 0; i < _gridSize; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                {
                    var button = new Button
                    {
                        Text = "",
                        FontSize = _gridSize == 3 ? 32 : (_gridSize == 4 ? 24 : 18),
                        FontAttributes = FontAttributes.Bold,
                        BackgroundColor = Colors.White,
                        TextColor = Colors.Black
                    };

                    int row = r;
                    int col = c;
                    button.Clicked += (s, e) => MakeMove(button, row, col);

                    // Salvestame nupu viite massiivi
                    _buttons[row, col] = button;
                    _grid.Add(button, col, row);
                }
            }
        }

        private async void MakeMove(Button btn, int row, int col)
        {
            // Kontrollime, et keegi ei saaks vajutada arvuti "mõtlemise" ajal või kui mäng on läbi
            if (_game.Board[row, col] != null) return;

            if (_game.MakeMove(row, col))
            {
                btn.Text = _game.CurrentPlayer;
                btn.TextColor = _game.CurrentPlayer == "X" ? Colors.Crimson : Colors.DarkTurquoise;

                if (await CheckGameResult())
                {
                    return; // Mäng sai läbi
                }

                // Vahetame mängijat (Mängija -> Arvuti)
                _game.TogglePlayer();
                UpdateStatusLabels();

                // 2. UUS: Kui järgmine mängija on "O", teeb arvuti oma käigu
                if (_game.CurrentPlayer == "O")
                {
                    await Task.Delay(400); // Väike ooteaeg, et tunduks loomulikum
                    await ComputerMove();
                }
            }
        }

        // 3. UUS: Loogika, mis otsib dünaamiliselt vabad ruudud ja teeb suvalise käigu
        private async Task ComputerMove()
        {
            var emptyCells = new List<(int Row, int Col)>();

            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                {
                    if (string.IsNullOrEmpty(_game.Board[r, c]))
                    {
                        emptyCells.Add((r, c));
                    }
                }
            }

            if (emptyCells.Count > 0)
            {
                var random = new Random();
                var (compRow, compCol) = emptyCells[random.Next(emptyCells.Count)];

                var compBtn = _buttons[compRow, compCol];

                if (_game.MakeMove(compRow, compCol))
                {
                    compBtn.Text = _game.CurrentPlayer;
                    compBtn.TextColor = _game.CurrentPlayer == "X" ? Colors.Crimson : Colors.DarkTurquoise;

                    if (await CheckGameResult())
                    {
                        return; // Mäng sai läbi
                    }

                    // Vahetame mängijat tagasi (Arvuti -> Mängija)
                    _game.TogglePlayer();
                    UpdateStatusLabels();
                }
            }
        }

        // 4. UUS: Abimeetod tulemuse kontrollimiseks ja teavituste kuvamiseks
        private async Task<bool> CheckGameResult()
        {
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
                return true;
            }
            return false;
        }

        // 5. UUS: Abimeetod siltide teksti uuendamiseks
        private void UpdateStatusLabels()
        {
            _statusLabel.Text = $"Mängija {_game.CurrentPlayer} kord";
            _playerTurnToolbarItem.Text = $" {_game.CurrentPlayer}";
        }

        private void RestartGame()
        {
            _game.ResetGame();
            CreateButtons();
            UpdateStatusLabels();
        }

        private async void BtnWhoStarts_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Vali alustaja:", "Tühista", null, "Mängija X", "Mängija O", "Juhuslik");

            if (action == "Mängija X") _game.SetStartingPlayer("X");
            else if (action == "Mängija O") _game.SetStartingPlayer("O");
            else if (action == "Juhuslik") _game.SetStartingPlayer(new Random().Next(0, 2) == 0 ? "X" : "O");

            RestartGame();
        }

        private async void BtnSize_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Vali mängulaua suurus:", "Tühista", null, "3x3", "4x4", "5x5");

            if (action == "3x3") _gridSize = 3;
            else if (action == "4x4") _gridSize = 4;
            else if (action == "5x5") _gridSize = 5;
            else return;

            _game.UpdateGridSize(_gridSize);
            RestartGame();
        }
    }
}
