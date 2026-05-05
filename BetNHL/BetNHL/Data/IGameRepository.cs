using BetNHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Data
{
    public interface IGameRepository
    {
        Task AddAuthHeader();
        Task<List<NhlGameDTO>> GetGames();
        Task<List<NhlGamePlayersDTO>> GetGamesWithPlayers(int gameID);

        Task<List<GameOddsDTO>> GetGamesWithOdds();
        //Task<List<PlayerOddsDTO>> GetPlayerOdds();

        Task<List<PlayerOddsDTO>> GetPlayerOdds(int gameId);
        Task<GameTeamStandingsDTO> GetGameStandings(int gameId);
        Task<List<GameOddsDTO>> GetTodaysGamesWithOdds();

    }
}
