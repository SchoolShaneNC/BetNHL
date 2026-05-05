
using BetNHL_Web_Api.Models;

namespace BetNHL_Web_Api.Services
{
    public class OddsService : IOddsService
    {
        
        public decimal CalculateTeamWinOdds( List<NhlTeamStandingDTO> standings,string homeAbbrev,string awayAbbrev,string selectedTeamAbbr)
        {
            //gets team by abbrev because no id at this endpoint
            var home = standings.FirstOrDefault(t => t.Abbreviation == homeAbbrev);
            var away = standings.FirstOrDefault(t => t.Abbreviation == awayAbbrev);

            if (home == null || away == null)
                return 2.00m; //return even odds if no team

            var homeStrength = CalculateTeamStrength(home);
            var awayStrength = CalculateTeamStrength(away);

            var total = homeStrength + awayStrength;

            if (total <= 0)
                return 2.00m;

            var homeWinChance = homeStrength / total;
            var awayWinChance = awayStrength / total;

            //to make more house type odds
            var margin = 1.05m;

            homeWinChance *= margin;
            awayWinChance *= margin;

            var adjustedTotal = homeWinChance + awayWinChance;

            homeWinChance /= adjustedTotal;
            awayWinChance /= adjustedTotal;

            decimal selectedChance =
                selectedTeamAbbr == homeAbbrev ? homeWinChance :
                selectedTeamAbbr == awayAbbrev ? awayWinChance :
                0.5m;

            selectedChance = Math.Clamp(selectedChance, 0.01m, 0.99m);

            return Math.Round(1 / selectedChance, 2);
        }

      
        private decimal CalculateTeamStrength(NhlTeamStandingDTO team)
        {
            if (team.GamesPlayed == 0)
                return 0;

            decimal winRate = (decimal)team.Wins / team.GamesPlayed;
            decimal pointsRate = (decimal)team.Points / (team.GamesPlayed * 2);

            decimal goalDiffPerGame = (decimal)(team.GoalsFor - team.GoalsAgainst) / team.GamesPlayed;

            return (winRate * 0.5m)
             + (pointsRate * 0.4m)
             + (goalDiffPerGame * 0.1m); // reduced impact again because too high in the negatives
        }


        public decimal CalculatePlayerGoalOdds(NhlPlayerDTO player, NhlPlayerStatsDTO stats)
        {
            if (player == null || stats == null)
                return 3m;

            if (player.Position == "G" || player.Position == "goalies")
                return 500m; 

            if (stats.GamesPlayedThisSeason == 0)
                return 3m;

            decimal shotsPerGame = (decimal)stats.ShotsThisSeason / stats.GamesPlayedThisSeason;
            decimal shootingPercentage = stats.ShootingPercentageThisSeason / 100m;

            decimal expectedGoalsPerGame = shotsPerGame * shootingPercentage;

            //small probablility calculation
            var probabilityOfNoGoals = Math.Exp(-(double)expectedGoalsPerGame);
            decimal probabilityToScore = 1 - (decimal)probabilityOfNoGoals;

            // little adjustments so better scorers lower payout and worse scorers higher payout
            //same with the defensive position and obvously goalies
            if (expectedGoalsPerGame > 0.5m)
                probabilityToScore *= 1.10m;

            if (expectedGoalsPerGame < 0.15m)
                probabilityToScore *= 0.85m;

            if (stats.Position == "D")
                probabilityToScore *= 0.90m;

            //clamp the range since occasionally i had issues with it going wayyyyy to high
            probabilityToScore = Math.Clamp(probabilityToScore, 0.05m, 0.55m);

            decimal odds = 1 / probabilityToScore;

            return Math.Round(odds, 2);
        }

        public string ConvertToDisplayOdds(decimal decimalOdds)
        {
            if (decimalOdds >= 2.00m)
            {
                var displayOdds = (decimalOdds - 1) * 100;
                return $"+{Math.Round(displayOdds)}";
            }
            else
            {
                var displayOdds = -100 / (decimalOdds - 1);
                return $"{Math.Round(displayOdds)}";
            }
        }


    }
}