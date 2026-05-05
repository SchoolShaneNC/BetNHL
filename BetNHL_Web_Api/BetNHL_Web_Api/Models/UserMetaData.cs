using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{   // week 1
    public class UserMetaData
    {

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = "";


        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!.%*?&])[^\s<>]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, include 1 uppercase letter, 1 lowercase letter, " +
            "1 number, and 1 special character. (@$!.%*?&")]
        public string PasswordHash { get; set; } = "";


        [Required]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = "";

        // Betting stats
        [Range(0, int.MaxValue)]
        public int BetsWon { get; set; }

        [Range(0, int.MaxValue)]
        public int BetsLost { get; set; }

        // Money tracking
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
