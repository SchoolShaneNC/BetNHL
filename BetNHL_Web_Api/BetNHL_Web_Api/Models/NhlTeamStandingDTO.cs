namespace BetNHL_Web_Api.Models
{
    public class NhlTeamStandingDTO
    {
        public int TeamId { get; set; }
        public string Abbreviation { get; set; }
        public string TeamName { get; set; }
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Otl { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifferential { get; set; }
        public double PointsPercentage { get; set; }
    }
}

