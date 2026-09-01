
namespace TARge25MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //Loome esimese lehe StartPage
            var startPage = new StartPage();
            //Pakime selle NavigationPage sisse, et saaksime kasutada navigeerimist
            var navPage = new NavigationPage(startPage)
            {
            BarBackgroundColor = Colors.LightBlue,
            BarTextColor = Colors.White
            };
            return new Window(new AppShell());
        }
    }
}