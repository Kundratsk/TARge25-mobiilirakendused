namespace TARge25MAUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;
            CounterBtn.Text = $"Vajutatud {count} korda";

            // Genereerime juhusliku värvi (R, G, B)
            var random = new Random();
            var randomColor = Color.FromRgb(
                random.Next(0, 256), // Red
                random.Next(0, 256), // Green
                random.Next(0, 256)  // Blue
            );
            BotImage.Rotation += 15;
            // Rakendame värvi teisele nupule või taustale
            ResetBtn.BackgroundColor = randomColor;
        }

        private void OnResetClicked(object? sender, EventArgs e)
        {
            count = 0;

            CounterBtn.Text = "Vajuta mind";

            CounterLabel.Text = "Alustame uuesti!";

            BotImage.Rotation = 0;
        }
    }
}
