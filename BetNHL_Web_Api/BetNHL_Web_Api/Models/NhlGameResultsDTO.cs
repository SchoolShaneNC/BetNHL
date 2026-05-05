namespace BetNHL_Web_Api.Models
{
    public class NhlGameResultsDTO
    {
        public string GameState { get; set; }

        public TeamScoreDTO AwayTeam { get; set; }

        public TeamScoreDTO HomeTeam { get; set; }
        public SummaryDTO Summary { get; set; }
    }

    public class TeamScoreDTO
    {
        public int Id { get; set; }
        public int Score { get; set; }
    }

    public class SummaryDTO
    {
        public List<ScoringPeriodDTO> Scoring { get; set; }
    }

    public class ScoringPeriodDTO
    {
        public List<GoalDTO> Goals { get; set; }
    }

    public class GoalDTO
    {
        public int PlayerId { get; set; }
    }
}