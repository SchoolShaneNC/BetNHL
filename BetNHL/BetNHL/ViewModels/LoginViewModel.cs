using BetNHL.Utilities;
using BetNHL.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BetNHL.ViewModels
{
    public class LoginViewModel
    {
        private readonly AuthService _authService;

        public string Username { get; set; }
        public string Password { get; set; }

        public Command LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }


        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            LoginCommand = new Command(async () => await Login());
            GoToRegisterCommand = new Command(async () => await GoToRegister());

        }

        private async Task Login()
        {
            var success = await _authService.LoginAsync(Username, Password);

            if (success)
            {

                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Invalid login", "OK");
            }
        }


        private async Task Logout()
        {
            SecureStorage.Remove("auth_token");

            Application.Current.MainPage = new AuthShell();
        }

        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("//Register");
        }


    }
}



