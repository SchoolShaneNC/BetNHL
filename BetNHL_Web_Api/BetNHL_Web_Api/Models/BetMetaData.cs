using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{       // week 1
    public class BetMetaData
    {

        [Required]
        public DateTime DatePlaced { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount bet must be greater than zero")]
        public decimal AmountBet { get; set; }

        [Required]
        public decimal Odds { get; set; }

        [Required(ErrorMessage = "GameId is required")]
        public int GameId { get; set; }

        [Required(ErrorMessage = "Bet type is required")]
        public BetType Type { get; set; }

    }
}
