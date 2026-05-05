using BetNHL.ViewModels;
namespace BetNHL.Views;

public partial class Register : ContentPage
{
    public Register(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm; 
    }
}