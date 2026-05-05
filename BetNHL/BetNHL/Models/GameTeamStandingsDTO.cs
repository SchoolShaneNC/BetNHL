using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Models
{
    public class GameTeamStandingsDTO
    {
        public NhlTeamStandingDTO Home { get; set; }
        public NhlTeamStandingDTO Away { get; set; }
    }
}
