using Microsoft.Maui.Graphics;
using BetNHL.Models;
using BetNHL.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using BetNHL;
using BetNHL.Utilities;
using System.Numerics;
using System.Text;
namespace BetNHL.Views { 
public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
}