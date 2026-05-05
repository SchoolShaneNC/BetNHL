namespace BetNHL.Models
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public int BetsWon { get; set; }
        public int BetsLost { get; set; }

        public decimal Balance { get; set; }

        public decimal TotalMoneyBet { get; set; }
        public decimal TotalMoneyWon { get; set; }
        public decimal TotalMoneyLost { get; set; }
    }
}
