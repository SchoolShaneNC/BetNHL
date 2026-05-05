using BetNHL.Models;
using System.ComponentModel.DataAnnotations;

namespace BetNHL.Models
{
    public class CreateBetDTO 
    {
        public decimal AmountBet { get; set; }
        public decimal Odds { get; set; }
        public int GameId { get; set; }
        public BetType Type { get; set; }
        public int? TeamPickedID { get; set; }
        public int? PlayerPickedID { get; set; }

    }
}
