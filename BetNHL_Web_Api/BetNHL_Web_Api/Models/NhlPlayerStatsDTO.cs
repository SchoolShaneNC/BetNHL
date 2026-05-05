namespace BetNHL_Web_Api.Models
{
    //week 2
    public class NhlPlayerStatsDTO
    {
        public int ID { get; set; }           
        public string Position { get; set; } = "";  

        public int PointsThisSeason
        {
            get
            {
                return GoalsThisSeason + AssistsThisSeason;
            }
        }

        public int GoalsThisSeason { get; set; }
        public int AssistsThisSeason { get; set; }

        public int ShotsThisSeason { get; set; }
        public int GamesPlayedThisSeason { get; set; }
        public int PlusMinusThisSeason { get; set; }

        public decimal ShootingPercentageThisSeason
        {
           
            get
            {
                if (ShotsThisSeason == 0) return 0;
                return Math.Round((decimal)GoalsThisSeason / ShotsThisSeason * 100, 2);
            }
        }


    }
}
