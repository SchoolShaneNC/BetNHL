using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{       // week 1
    public enum BetType { TeamWin, PlayerGoal }

    [MetadataType(typeof(BetMetaData))]
    public class Bet : IValidatableObject
    {
        public int ID { get; set; }

        public DateTime DatePlaced { get; set; }
        public decimal AmountBet { get; set; }

        public decimal Odds { get; set; }

        public bool? Won { get; set; }  

        public int GameId { get; set; }

        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
        public BetType Type { get; set; }
        public int? TeamPickedID { get; set; }
        public int? PlayerPickedID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TeamPickedID.HasValue && PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "A bet cannot have both a team and a player.",
                    new[] { nameof(TeamPickedID), nameof(PlayerPickedID) });
            }

            if (!TeamPickedID.HasValue && !PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "A bet must have either a team or a player.",
                    new[] { nameof(TeamPickedID), nameof(PlayerPickedID) });
            }

            // Ensure it matches the bet type
            if (Type == BetType.TeamWin && !TeamPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "Team bet requires a team.",
                    new[] { nameof(TeamPickedID) });
            }

            if (Type == BetType.PlayerGoal && !PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "Player bet requires a player.",
                    new[] { nameof(PlayerPickedID) });
            }

        }
    }
}
