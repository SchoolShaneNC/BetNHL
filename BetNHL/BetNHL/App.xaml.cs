using BetNHL;
using BetNHL.Data;
using BetNHL.Models;
using BetNHL.Views;

namespace BetNHL
{
    //public partial class App : Application
    //{
    //    public App()
    //    {
    //        InitializeComponent();

    //        // temporary page while loading
    //        MainPage = new ContentPage();

    //        // run async startup logic
    //        InitializeApp();
    //    }

    //    private async void InitializeApp()
    //    {
    //        var token = await SecureStorage.GetAsync("auth_token");
    //        token = null; // for testing purposes, remove this line to enable auto-login

    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            MainPage = new AppShell();
    //        }
    //        else
    //        {
    //            MainPage = new NavigationPage(MauiProgram.Services.GetRequiredService<LoginPage>());
    //        }
    //    }

    //    protected override Window CreateWindow(IActivationState? activationState)
    //    {
    //        return new Window(MainPage);
    //    }
    //}





    //----------------------------------------------------------------------------shell first pass
    //public partial class App : Application
    //{
    //    public App()
    //    {
    //        InitializeComponent();

    //        // ✅ Shell must be set FIRST
    //        MainPage = new AppShell();

    //        // run async logic after Shell exists
    //        _ = InitializeApp();
    //    }

    //    private async Task InitializeApp()
    //    {
    //        var token = await SecureStorage.GetAsync("auth_token");
    //        token = null; // testing

    //        if (Shell.Current == null)
    //        {
    //            throw new Exception("Shell is NULL at navigation time");
    //        }

    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            await Shell.Current.GoToAsync("//MainPage");
    //        }
    //        else
    //        {
    //            await Shell.Current.GoToAsync("//LoginPage");
    //        }
    //    }




    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            _ = InitializeApp();
        }

        private async Task InitializeApp()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            token = null; // for testing purposes, remove this line to enable auto-login

            if (!string.IsNullOrEmpty(token))
            {
                MainPage = new AppShell();   // logged in
            }
            else
            {
                MainPage = new AuthShell();  // not logged in
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }
    }
}