using BetNHL.ViewModels;

namespace BetNHL.Views
{
    public partial class GameDetailsPage : ContentPage
    {
        public GameDetailsPage(GameDetailsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }

}


