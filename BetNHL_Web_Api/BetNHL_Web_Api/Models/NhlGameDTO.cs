namespace BetNHL_Web_Api.Models
{  // week 1
    public class NhlGameDTO
    {
        public int Id { get; set; }
        public NhlTeamDTO HomeTeam { get; set; }
        public NhlTeamDTO AwayTeam { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
    }
}
