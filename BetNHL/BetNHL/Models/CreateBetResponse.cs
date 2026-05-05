using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Models
{
    public class CreateBetResponse
    {
        public int BetId { get; set; }
        public decimal NewBalance { get; set; }
    }
}
