using BetNHL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BetNHL.ViewModels
{
    public class RegisterViewModel
    {
        private readonly AuthService _authService;

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await Register());
            GoToLoginCommand = new Command(async () => await GoToLogin());

        }

        private async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
        private async Task Register()
        {
            var success = await _authService.Register(Username, Password, ConfirmPassword);

            if (success)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
