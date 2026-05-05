using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{   // week 1
    [MetadataType(typeof(UserMetaData))]
    public class UserDTO
    {
        public string ID { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }
        public int BetsWon { get; set; }
        public int BetsLost { get; set; }

   
        public decimal Balance { get; set; }
        public decimal TotalMoneyBet { get; set; }
        public decimal TotalMoneyWon { get; set; }
        public decimal TotalMoneyLost { get; set; }
        public List<BetDTO>? Bets { get; set; } = new List<BetDTO>();
    }
}
