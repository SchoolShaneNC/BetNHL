using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Models
{
    public class GameOddsDTO
    {
        public int GameId { get; set; }

        public string HomeTeamAbbr { get; set; }
        public string AwayTeamAbbr { get; set; }

        public decimal HomeOddsDecimal { get; set; }
        public string HomeDisplayOdds { get; set; }

        public decimal AwayOddsDecimal { get; set; }
        public string AwayDisplayOdds { get; set; }

        public string HomeLogo => $"{HomeTeamAbbr}.png";
        public string AwayLogo => $"{AwayTeamAbbr}.png";
        public DateTime StartTime { get; set; }
    }
}
