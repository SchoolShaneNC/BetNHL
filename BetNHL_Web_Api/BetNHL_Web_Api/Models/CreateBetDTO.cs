using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    [MetadataType(typeof(BetMetaData))]
    public class CreateBetDTO : IValidatableObject
    {
        public decimal AmountBet { get; set; }
        public decimal Odds { get; set; }
        public int GameId { get; set; }
        public BetType Type { get; set; }
        public int? TeamPickedID { get; set; }
        public string? TeamPickedAbbr { get; set; }
        public int? PlayerPickedID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Cannot have both
            if ((TeamPickedID.HasValue || !string.IsNullOrEmpty(TeamPickedAbbr)) && PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "A bet cannot have both a team and a player.");
            }

            // Must have one
            if (!TeamPickedID.HasValue && string.IsNullOrEmpty(TeamPickedAbbr) && !PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "A bet must have either a team or a player.");
            }

            // Team bet validation
            if (Type == BetType.TeamWin)
            {
                if (!TeamPickedID.HasValue && string.IsNullOrEmpty(TeamPickedAbbr))
                {
                    yield return new ValidationResult(
                        "Team bet requires a team.",
                        new[] { nameof(TeamPickedAbbr) });
                }
            }

            // Player bet validation
            if (Type == BetType.PlayerGoal)
            {
                if (!PlayerPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Player bet requires a player.",
                        new[] { nameof(PlayerPickedID) });
                }
            }
        }
    }
}
