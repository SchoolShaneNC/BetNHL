using Microsoft.Maui.Graphics;
using BetNHL.Models;
using BetNHL.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using BetNHL;
using BetNHL.Utilities;
using System.Numerics;
using System.Text;
using BetNHL.Data;
using System.Runtime.CompilerServices;


namespace BetNHL.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            //creates the vm on apparing so each time you nav to mainpage it updates the games
            base.OnAppearing();

            if (BindingContext is MainViewModel vm)
            {
                await vm.LoadData();
            }
        }

    }

}
