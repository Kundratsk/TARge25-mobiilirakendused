
namespace MemoGame;

public partial class MainPage : ContentPage
{
    private Game _game;
    private bool _timerStarted;
    private int _timerGeneration;

    public MainPage()
    {
        InitializeComponent();

        _game = new Game(new Player("Mängija"));

        _game.StatsChanged += UpdateStats;
        _game.GameFinished += OnGameFinished;

        StartNewGame();
    }

    private void StartNewGame()
    {
        _timerGeneration++;
        _timerStarted = false;

        _game.Start();

        GameGrid.Children.Clear();

        // Loo 16 kaarti ja paiguta need 4 x 4 ruudustikku.
        for (int i = 0; i < _game.Cards.Count; i++)
        {
            Card card = _game.Cards[i];

            var button = new Button
            {
                Text = "?",
                FontSize = 27,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("#303858"),
                CornerRadius = 14,
                Padding = 0,
                MinimumHeightRequest = 0,
                MinimumWidthRequest = 0,
                HeightRequest = 72
            };

            card.Button = button;

            button.Clicked += async (sender, args) =>
            {
                if (!_timerStarted && !_game.IsFinished)
                {
                    _timerStarted = true;
                    StartTimer();
                }

                await _game.SelectCard(card);

                UpdateStats();
            };

            int row = i / 4;
            int column = i % 4;

            GameGrid.Add(button);
            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
        }

        StatusLabel.Text = "Vali kaks kaarti ja leia paar!";
        NewGameButton.Text = "↻   UUS MÄNG";

        UpdateStats();
    }

    private void UpdateStats()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MovesLabel.Text = _game.Player.Moves.ToString();
            PairsLabel.Text = $"{_game.Player.MatchedPairs} / 8";

            StatusLabel.Text =
                $"Punktid: {_game.Player.Score}    ·    Aeg: {_game.Elapsed:mm\\:ss}";
        });
    }

    private void StartTimer()
    {
        int generation = _timerGeneration;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (generation != _timerGeneration || _game.IsFinished)
                return false;

            UpdateStats();
            return true;
        });
    }

    private void OnGameFinished()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            UpdateStats();

            StatusLabel.Text =
                $"🎉 Palju õnne! Kõik paarid leitud!\n" +
                $"Punktid: {_game.Player.Score}";

            await DisplayAlert(
                "Mäng läbi!",
                $"Leidsid kõik 8 paari!\n\n" +
                $"Käigud: {_game.Player.Moves}\n" +
                $"Punktid: {_game.Player.Score}\n" +
                $"Aeg: {_game.Elapsed:mm\\:ss}",
                "Tubli!");
        });
    }

    private void OnNewGameClicked(object? sender, EventArgs e)
    {
        StartNewGame();
    }
}
