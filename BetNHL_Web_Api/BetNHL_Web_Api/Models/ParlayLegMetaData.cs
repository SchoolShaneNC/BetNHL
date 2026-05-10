using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    public class ParlayLegMetaData
    {
        [Required]
        public int GameId { get; set; }

        [Required]
        public BetCategory Category { get; set; }

        [Required]
        public BetMetric Metric { get; set; }

        [Required]
        public BetCondition Condition { get; set; }

        [Required]
        public BetContext Context { get; set; }
    }
}
