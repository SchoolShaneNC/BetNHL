namespace BetNHL_Web_Api.Models
{
    //week 2
    public class NhlPlayerGameStatsDTO
    {
        public DateTime GameDate { get; set; }
        public string OpponentAbbrev {  get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points => Goals + Assists;

        public int Shots { get; set; }
        public string TimeOnIce { get; set; } 
    }
}
