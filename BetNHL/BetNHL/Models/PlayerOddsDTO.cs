using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Models
{
    public class PlayerOddsDTO
    {
        public int PlayerId { get; set; }
        public string DisplayName { get; set; }
        public string ScoreOdds { get; set; } 
        public string TeamAbbr { get; set; }   
        public string Position { get; set; }
        public string OppAbbr { get; set; } 
        public string GameTime { get; set; }

        public string TeamLogo =>
            string.IsNullOrWhiteSpace(TeamAbbr)
                ? "default.png"
                : $"{TeamAbbr.ToLower()}.png";
    }
}
