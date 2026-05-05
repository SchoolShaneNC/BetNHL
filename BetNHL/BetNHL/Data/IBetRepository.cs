using BetNHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Data
{
    public interface IBetRepository
    {
        Task AddAuthHeader();

        //get all bets
        Task<List<Bet>> GetBets();

        // get bet by id
        Task<Bet> GetBetByID(int id);

        // get user bets
        Task<List<Bet>> GetBetsByUser(string userId);

        //get unresolved bvets
        Task<List<Bet>> GetUnresolvedBets();

        // create bet
        Task<(int betId, decimal newBalance)> CreateBet(CreateBetDTO dto);

        // put bet
        Task UpdateBet(int id, Bet bet);

        // delete bet
        Task DeleteBet(int id);

        // resolve user bets
        Task<int> ResolveUserBets();
    }
}

