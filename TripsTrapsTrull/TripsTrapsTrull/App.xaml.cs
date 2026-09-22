using Microsoft.Extensions.DependencyInjection;


namespace TripsTrapsTrull
{
    public partial class App : Application
    {
        public App()
        {
            // Esimene vaade on nüüd RulesPage, kuhu saadame tühja GameLogic objekti
            MainPage = new NavigationPage(new RulesPage(new GameLogic()));
        }
    }
}