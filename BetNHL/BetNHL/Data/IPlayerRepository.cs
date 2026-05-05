using BetNHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Data
{
    public  interface IPlayerRepository
    {
        Task AddAuthHeader();
        Task<List<NhlPlayerDTO>> GetPlayerByID(int id);
        Task<List<NhlGamePlayersDTO>> GetPlayerStatsByID(int id);
    }
}
