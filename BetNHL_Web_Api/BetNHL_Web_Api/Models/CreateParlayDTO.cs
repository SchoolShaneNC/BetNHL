using BetNHL_Web_Api.Models;
using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    [MetadataType(typeof(ParlayMetaData))]
    public class CreateParlayDTO : IValidatableObject
    {
        public decimal Stake { get; set; }

        public List<CreateParlayLegDTO> Legs { get; set; } = new();

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
                .Any(g => g.Count() > 8);

            if (sameGameCount)
            {
                yield return new ValidationResult("Too many selections from the same game.");
            }
        }
    }
}
