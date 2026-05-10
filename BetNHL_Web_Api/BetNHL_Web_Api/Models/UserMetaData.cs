using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{   // week 1
    public class UserMetaData
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int BetsWon { get; set; }

        [Range(0, int.MaxValue)]
        public int BetsLost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalMoneyBet { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalMoneyWon { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalMoneyLost { get; set; }
    }
}