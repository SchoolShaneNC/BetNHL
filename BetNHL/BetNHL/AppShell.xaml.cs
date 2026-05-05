using BetNHL.Models;
using BetNHL.Views;

namespace BetNHL
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(GameDetailsPage), typeof(GameDetailsPage));
            //Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            //Routing.RegisterRoute("Register", typeof(Register));
            //Routing.RegisterRoute("MainPage", typeof(MainPage));

        }
    }
}
