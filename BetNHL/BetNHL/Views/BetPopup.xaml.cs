using CommunityToolkit.Maui.Views;
using BetNHL.Models;

namespace BetNHL.Views;

public partial class BetPopup : Popup
{
    private CreateBetDTO _bet;

    public BetPopup(CreateBetDTO bet)
    {
        InitializeComponent();
        _bet = bet;
        BindingContext = _bet;
    }

    private void OnConfirm(object sender, EventArgs e)
    {
        //confirm and return bet
        if (_bet.AmountBet <= 0)
            return;
        Close(_bet); 
    }

    private void OnClose(object sender, EventArgs e)
    {
        Close(null);
    }
}