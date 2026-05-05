using System.ComponentModel.DataAnnotations;

namespace BetNHL.Models
{       // week 1
    public enum BetType { TeamWin, PlayerGoal }
    public class Bet
    {
        public int ID { get; set; }

        public DateTime DatePlaced { get; set; }
        public decimal AmountBet { get; set; }
        public decimal Odds { get; set; }

        public bool? Won { get; set; }

        public int GameId { get; set; }

        public string UserID { get; set; }

        public BetType Type { get; set; }

        public int? TeamPickedID { get; set; }

        public int? PlayerPickedID { get; set; }
    }
}
