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

            // Kui loendur on 5 või rohkem, muuda nupu värvi
            if (count >= 5)
            {
                CounterBtn.BackgroundColor = Colors.Red;
                CounterBtn.TextColor = Colors.White;
            }

            if (count <= 10)
            {
                BotImage.Scale += 0.1;
            }

            if (count >= 10)
            {
                BotImage.IsVisible = false; // Peidab pildi
                CounterLabel.Text = "Pilt kadus ära! Vajuta Reset.";
            }

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

            BotImage.Scale = 1;

            // VÕI teeme loogika: kui on vasakul, liiguta paremale, ja vastupidi
            if (BotImage.HorizontalOptions == LayoutOptions.Start)
            {
                BotImage.HorizontalOptions = LayoutOptions.End;
            }
            else
            {
                BotImage.HorizontalOptions = LayoutOptions.Start;
            }

            BotImage.Rotation = 0;

            CounterBtn.ClearValue(BackgroundColorProperty);// eemaldab counter nupu taustavärvi
            CounterBtn.ClearValue(Button.TextColorProperty);// eemaldab counter nupu tekstivärvi


            BotImage.IsVisible = true; // Toob pildi tagasi
        }
    }
}
