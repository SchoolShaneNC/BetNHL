namespace BetNHL.Models
{
    /// <summary> // week 1
    /// this DTO combines game details with the list of players participating in the game. 
    /// it is used for creating bets that are player-specific, such as "Player X to score a goal".
    /// Player list on days with 10-16 games would be large for just displaying what games are happening
    /// this helps seperate concerns to help performance
    /// </summary>
    public class NhlGamePlayersDTO
    {
        public int GameId { get; set; }
        public NhlTeamDTO HomeTeam { get; set; }
        public NhlTeamDTO AwayTeam { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public ICollection<NhlPlayerDTO> Players { get; set; } = new List<NhlPlayerDTO>();

    }
}
