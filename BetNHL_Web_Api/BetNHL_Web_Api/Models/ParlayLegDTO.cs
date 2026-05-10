namespace BetNHL_Web_Api.Models
{
    public class ParlayLegDTO
    {
        public int ID { get; set; }
        public int ParlayId { get; set; }
        public BetCategory Category { get; set; }

        public BetMetric Metric { get; set; }

        public BetCondition Condition { get; set; }

        public BetContext Context { get; set; }

        public int GameId { get; set; }

        public int? TeamPickedID { get; set; }

        public int? PlayerPickedID { get; set; }

        public decimal Odds { get; set; }

        public decimal? Line { get; set; }

        public bool? Won { get; set; }
    }
}