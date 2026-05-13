using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{
    public class CreateParlayLegDTO : IValidatableObject
    {
        public BetCategory Category { get; set; }

        public BetMetric Metric { get; set; }

        public BetCondition Condition { get; set; }

        public BetContext Context { get; set; }

        public int GameId { get; set; }
        public decimal? Line { get; set; }

        public int? TeamPickedID { get; set; }

        public string? TeamPickedAbbr { get; set; }

        public int? PlayerPickedID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Cannot select both
            if (TeamPickedID.HasValue && PlayerPickedID.HasValue)
            {
                yield return new ValidationResult(
                    "A leg cannot contain both a team and player selection.",
                    new[] { nameof(TeamPickedID), nameof(PlayerPickedID) });
            }

            // PLAYER CATEGORY
            if (Category == BetCategory.Player)
            {
                if (!PlayerPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Player bets require a player selection.",
                        new[] { nameof(PlayerPickedID) });
                }

                if (TeamPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Player bets cannot contain a team selection.",
                        new[] { nameof(TeamPickedID) });
                }

                // Invalid player metrics
                if (Metric == BetMetric.TotalGoals)
                {
                    yield return new ValidationResult(
                        "Player bets cannot use TotalGoals metric.",
                        new[] { nameof(Metric) });
                }
            }

            // TEAM CATEGORY
            if (Category == BetCategory.Team)
            {
                if (!TeamPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Team bets require a team selection.",
                        new[] { nameof(TeamPickedID) });
                }

                if (PlayerPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Team bets cannot contain a player selection.",
                        new[] { nameof(PlayerPickedID) });
                }

                // Team bets shouldn't use assist/point
                if (Metric == BetMetric.Assist ||
                    Metric == BetMetric.Point)
                {
                    yield return new ValidationResult(
                        "Invalid metric for team bets.",
                        new[] { nameof(Metric) });
                }
            }

            // GAME CATEGORY
            if (Category == BetCategory.Game)
            {
                if (PlayerPickedID.HasValue || TeamPickedID.HasValue)
                {
                    yield return new ValidationResult(
                        "Game bets cannot contain team or player selections.",
                        new[] { nameof(PlayerPickedID), nameof(TeamPickedID) });
                }

                if (Metric != BetMetric.TotalGoals)
                {
                    yield return new ValidationResult(
                        "Game bets currently only support TotalGoals.",
                        new[] { nameof(Metric) });
                }

                if (Condition != BetCondition.Over &&
                    Condition != BetCondition.Under)
                {
                    yield return new ValidationResult(
                        "Game total bets must be Over or Under.",
                        new[] { nameof(Condition) });
                }
            }

            // Powerplay validation
            if (Context == BetContext.Powerplay)
            {
                // Only makes sense for player scoring stats
                if (Category != BetCategory.Player)
                {
                    yield return new ValidationResult(
                        "Powerplay context only applies to player bets.",
                        new[] { nameof(Context) });
                }

                if (Metric != BetMetric.Goal &&
                    Metric != BetMetric.Assist &&
                    Metric != BetMetric.Point)
                {
                    yield return new ValidationResult(
                        "Powerplay context invalid for this metric.",
                        new[] { nameof(Context) });
                }
            }

            // First only makes sense for goals
            if (Condition == BetCondition.First &&
                Metric != BetMetric.Goal)
            {
                yield return new ValidationResult(
                    "First condition only applies to goals.",
                    new[] { nameof(Condition) });
            }

            // Multi only valid for stat accumulation metrics
            if (Condition == BetCondition.Multi &&
                Metric != BetMetric.Goal &&
                Metric != BetMetric.Assist &&
                Metric != BetMetric.Point)
            {
                yield return new ValidationResult(
                    "Multi condition only applies to goals, assists, or points.",
                    new[] { nameof(Condition) });
            }
        }
    }
}
