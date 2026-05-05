using System.ComponentModel.DataAnnotations;

namespace BetNHL_Web_Api.Models
{       // week 1
    [MetadataType(typeof(BetMetaData))]
    public class BetDTO
    {

        public int ID { get; set; }
        public DateTime DatePlaced { get; set; }
        public decimal AmountBet { get; set; }
        public decimal Odds { get; set; }
        public bool? Won { get; set; }
        public int GameId { get; set; }
        public BetType Type { get; set; }
        public int? TeamPickedID { get; set; }
        public string? TeamPickedAbbr { get; set; }
        public int? PlayerPickedID { get; set; }

        public string UserID { get; set; }
        public string Username { get; set; }
    } }


