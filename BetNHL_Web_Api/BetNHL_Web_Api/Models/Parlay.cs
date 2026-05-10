using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    public class Parlay : IValidatableObject
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime DatePlaced { get; set; }

        public decimal Stake { get; set; }

        public decimal CombinedOdds { get; set; }

        public decimal PotentialPayout { get; set; }

        public bool? Won { get; set; }

        public List<ParlayLeg> Legs { get; set; } = new();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Legs == null || !Legs.Any())
            {
                yield return new ValidationResult("Parlay must contain at least one leg.");
            }
            if (Legs.Count > 15)
            {
                yield return new ValidationResult("Maximum 15 legs allowed.");
            }

            var duplicates = Legs
                .GroupBy(l => new { l.GameId, l.TeamPickedID, l.PlayerPickedID, l.Category, l.Metric, l.Condition, l.Context })
                .Where(g => g.Count() > 1)
                .Any();

            if (duplicates)
            {
                yield return new ValidationResult("Duplicate selections are not allowed.");
            }

            var sameGameCount = Legs
                .GroupBy(l => l.GameId)
                .Any(g => g.Count() > 8); // arbitrary cap

            if (sameGameCount)
            {
                yield return new ValidationResult("Too many selections from the same game.");
            }

        }
    }

}
