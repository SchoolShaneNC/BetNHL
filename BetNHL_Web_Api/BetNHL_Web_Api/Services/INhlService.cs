using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetNHL_Web_Api.Services
{       // week 1
    public interface INhlService
    {
        Task<List<NhlGameDTO>> GetTodaysGamesAsync();
        Task<NhlGamePlayersDTO> GetGameWithPlayersAsync(int gameId);
        Task<NhlPlayerDTO> GetPlayerAsync(int playerId);      
        Task<NhlTeamDTO> GetTeamAsync(int teamId);
        Task<NhlPlayerStatsDTO> GetPlayerStatsAsync(int playerId);
        Task<List<NhlTeamStandingDTO>> GetStandingsAsync();
        Task<NhlTeamStandingDTO> GetTeamStandingAsync(string teamAbbr);
        Task<GameTeamStandingsDTO> GetGameTeamStandingsAsync(string homeTeamAbbr, string awayTeamAbbr);
        Task<List<GameOddsDTO>> GetTodaysGamesWithOddsAsync();


        ////new stuff
        Task<NhlGameResultsDTO> FetchGameResultsByID(int gameId);
        Task<NhlGamePlayersDTO> GetGameByIDAsync(int gameId);
        Task<List<PlayerOddsDTO>> GetGamePlayerOddsAsync(int gameId);

    }
}
