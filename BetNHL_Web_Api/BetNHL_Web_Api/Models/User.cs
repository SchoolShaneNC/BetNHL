using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public int BetsWon { get; set; } = 0;
    public int BetsLost { get; set; } = 0;

    public decimal Balance { get; set; } = 5000;
    public decimal TotalMoneyBet { get; set; } = 0;
    public decimal TotalMoneyWon { get; set; } = 0;
    public decimal TotalMoneyLost { get; set; } = 0;

    public List<Bet> Bets { get; set; } = new();
}

