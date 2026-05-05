using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Models
{
    public  class NhlGameDTO
    {
        public int Id { get; set; }
        public NhlTeamDTO HomeTeam { get; set; }
        public NhlTeamDTO AwayTeam { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
       
    }


}
