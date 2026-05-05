using BetNHL_Web_Api.Models;
using BetNHL_Web_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetNHL_Web_Api.Controllers
{ // week 1
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NHLController : ControllerBase
    {
        // week 1
        private readonly INhlService _nhlService;

        public NHLController(INhlService nhlService)
        {
            _nhlService = nhlService;
        }

        // GET: api/Bet/5
        [Authorize]
        [HttpGet("Games")]
        public async Task<ActionResult<List<NhlGameDTO>>> GetGamesToday(int id)
        {
            var games = await _nhlService.GetTodaysGamesAsync();

            if (games == null)
            {
                return NotFound();
            }

            return Ok(games);
        }

        [Authorize]
        [HttpGet("Games/{gameId}")]   // week 1
        public async Task<ActionResult<NhlGamePlayersDTO>> GetGameWithPlayers(int gameId)
        {
            var result = await _nhlService.GetGameWithPlayersAsync(gameId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("Games/WithOdds")]
        public async Task<ActionResult<List<GameOddsDTO>>> GetGamesWithOdds()
        {
            var result = await _nhlService.GetTodaysGamesWithOddsAsync();

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("player/{playerId}")]  // week 1
        public async Task<ActionResult<NhlPlayerDTO>> GetPlayerByID(int playerId)
        {
            var result = await _nhlService.GetPlayerAsync(playerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("player/stats/{playerId}")]  // week 1
        public async Task<ActionResult<NhlPlayerStatsDTO>> GetPlayerStatsByID(int playerId)
        {
            var result = await _nhlService.GetPlayerStatsAsync(playerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("Games/{gameId}/PlayerOdds")]
        public async Task<ActionResult<List<PlayerOddsDTO>>> GetPlayerOddsByGame(int gameId)
        {
            var result = await _nhlService.GetGamePlayerOddsAsync(gameId);

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("Standings/{teamId}")]
        public async Task<ActionResult<NhlTeamStandingDTO>> GetTeamStandingById(string abbrev)
        {
            var result = await _nhlService.GetTeamStandingAsync(abbrev);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GameStandings/{gameId}")]
        public async Task<ActionResult<GameTeamStandingsDTO>> GetGameStandings(int gameId)
        {
            var game = await _nhlService.GetGameByIDAsync(gameId);

            if (game == null)
                return NotFound();

            var result = await _nhlService.GetGameTeamStandingsAsync(
                game.HomeTeam.Abbreviation,
                game.AwayTeam.Abbreviation
            );

            return Ok(result);
        }
    }
}
