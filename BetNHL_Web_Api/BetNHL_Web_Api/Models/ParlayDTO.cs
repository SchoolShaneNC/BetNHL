namespace BetNHL_Web_Api.Models
{
    public class ParlayDTO
    {
        public int Id { get; set; }

        public string Username { get; set; } = "";

        public DateTime DatePlaced { get; set; }

        public decimal Stake { get; set; }

        public decimal CombinedOdds { get; set; }

        public decimal PotentialPayout { get; set; }

        public bool? Won { get; set; }

        public List<ParlayLegDTO> Legs { get; set; } = new();
    }
}