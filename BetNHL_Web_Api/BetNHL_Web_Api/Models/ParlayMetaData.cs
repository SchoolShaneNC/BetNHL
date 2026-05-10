using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{       // week 1
    public class ParlayMetaData
    {
        [Required]
        [Range(0.01, 10000, ErrorMessage = "Stake must be greater than zero")]
        public decimal Stake { get; set; }
        
    }
}
