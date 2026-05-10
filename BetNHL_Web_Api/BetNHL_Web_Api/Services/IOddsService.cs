using BetNHL_Web_Api.Models;

namespace BetNHL_Web_Api.Services
{
    public interface IOddsService
    {
        decimal CalculateTeamWinOdds(
            List<NhlTeamStandingDTO> standings,
            string homeAbbrev,
            string awayAbbrev,
            string selectedTeamAbbr);

        decimal CalculatePlayerGoalOdds(NhlPlayerDTO player, NhlPlayerStatsDTO stats);
        string ConvertToDisplayOdds(decimal decimalOdds);


        public decimal CalculateLegOdds(CreateParlayLegDTO leg);
        public decimal CalculateCombinedParlayOdds(List<decimal> odds);
    }
}