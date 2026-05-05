
using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!.%*?&])[^\s<>]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long, include 1 uppercase letter, 1 lowercase letter, " +
                           "1 number, and 1 special character.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }


}