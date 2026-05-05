using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetNHL_Web_Api.Data;
using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using BetNHL_Web_Api.Services;

namespace BetNHL_Web_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        private readonly BetNHLContext _context;
        //   private readonly NhlService _nhlService; old circular
        private readonly INhlService _nhlService;
        private readonly IOddsService _oddsService;
        public BetController(BetNHLContext context, IOddsService oddsService, INhlService nhlService)
        {
            _context = context;
            _oddsService = oddsService;
            _nhlService = nhlService;
        }

        // GET: api/Bet
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<BetDTO>>> GetBets()
        {
            var betDTOs = await _context.Bets
                 .Include(b => b.User)
                 .Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    Won = b.Won,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID,
                    UserID = b.UserID,
                    Username = b.User.UserName

                })
                .ToListAsync();

            if (betDTOs.Count == 0)
                return NotFound(new { message = "No betting records found." });

            return betDTOs;
        }

        // GET: api/Bet/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BetDTO>> GetBet(int id)
        {
            var betDTO = await _context.Bets
                .Include(b => b.User)
                .Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    Won = b.Won,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID,
                    UserID = b.UserID,
                    Username = b.User.UserName

                })
                .FirstOrDefaultAsync(b => b.ID == id);

            if (betDTO == null)
                return NotFound();

            return betDTO;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<BetDTO>>> GetBetsByUser(string userId)
        {
            var bets = await _context.Bets
                .Where(b => b.UserID == userId)
                .Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    Won = b.Won,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID,
                })
                .ToListAsync();

            if (!bets.Any()) return NotFound("No bets found for this user.");
            return Ok(bets);
        }

        [HttpGet("unresolved")]
        public async Task<ActionResult<List<BetDTO>>> GetUnresolvedBets()
        {
            var bets = await _context.Bets
                .Where(b => b.Won == null)
                .Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID
                })
                .ToListAsync();

            return Ok(bets);
        }

        // PUT: api/Bet/5
//       
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBet(int id, Bet bet)
        {
            if (id != bet.ID)
                return BadRequest();

            _context.Entry(bet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BetExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostBet(CreateBetDTO dto)
        {

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            if (user.Balance < dto.AmountBet)
                return BadRequest("Insufficient balance for this bet");

            decimal odds;

            if (dto.Type == BetType.TeamWin)
            {
                var game = await _nhlService.GetGameByIDAsync(dto.GameId);

                var homeAbbrev = game.HomeTeam.Abbreviation;
                var awayAbbrev = game.AwayTeam.Abbreviation;

                var picked = dto.TeamPickedAbbr;

                if (string.IsNullOrEmpty(picked))
                {
                    picked = game.HomeTeam.Id == dto.TeamPickedID
                        ? homeAbbrev
                        : awayAbbrev;
                }

                var standings = await _nhlService.GetStandingsAsync();

                odds = _oddsService.CalculateTeamWinOdds(
                    standings,
                    homeAbbrev,
                    awayAbbrev,
                    picked
                );
            }
            else
            {
                var player = await _nhlService.GetPlayerAsync(dto.PlayerPickedID.Value);
                var stats = await _nhlService.GetPlayerStatsAsync(dto.PlayerPickedID.Value);

                odds = _oddsService.CalculatePlayerGoalOdds(player, stats);
            }



            var bet = new Bet
            {
                DatePlaced = DateTime.UtcNow,
                AmountBet = dto.AmountBet,
                Odds = odds,
                GameId = dto.GameId,
                Type = dto.Type,
                TeamPickedID = dto.TeamPickedID,
                PlayerPickedID = dto.PlayerPickedID,
                UserID = userId
            };

            user.Balance -= dto.AmountBet;
            user.TotalMoneyBet += dto.AmountBet;

            _context.Bets.Add(bet);
            await _context.SaveChangesAsync();

            return Ok(new
            {
              
                betId = bet.ID,
                newBalance = user.Balance,
                oddsDecimal = bet.Odds,
                oddsAmerican = _oddsService.ConvertToDisplayOdds(bet.Odds)
               
            });
        }

        [Authorize]
        [HttpPost("resolve/user")]
        public async Task<IActionResult> ResolveUserBets()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var bets = await _context.Bets
                .Include(b => b.User)
                .Where(b => b.UserID == userId && b.Won == null)
                .ToListAsync();

            if (!bets.Any())
                return Ok(new { message = "No unresolved bets" });

            var betsByGame = bets.GroupBy(b => b.GameId).ToList();


            int resolvedCount = 0;

            foreach (var group in betsByGame)
            {
                var game = await _nhlService.FetchGameResultsByID(group.Key);

                //Skip if game not finished
                if (game.GameState != "OFF")
                    continue;

                // Determine winning team
                var winningTeamId = game.HomeTeam.Score > game.AwayTeam.Score
                    ? game.HomeTeam.Id
                    : game.AwayTeam.Id;

                //Get all goal scorers 
                var goalScorers = game.Summary.Scoring
                    .SelectMany(p => p.Goals)
                    .Select(g => g.PlayerId)
                    .ToList();

                foreach (var bet in group)
                {
                    bool isWin = false;

                    if (bet.Type == BetType.TeamWin)
                    {
                        isWin = bet.TeamPickedID == winningTeamId;
                    }
                    else if (bet.Type == BetType.PlayerGoal)
                    {
                        isWin = bet.PlayerPickedID.HasValue &&
                                goalScorers.Contains(bet.PlayerPickedID.Value);
                    }

                    bet.Won = isWin;

                    if (isWin)
                    {
                        var payout = bet.AmountBet * bet.Odds;

                        bet.User.Balance += payout;
                        bet.User.BetsWon++;
                        bet.User.TotalMoneyWon += payout;
                    }
                    else
                    {
                        bet.User.BetsLost++;
                        bet.User.TotalMoneyLost += bet.AmountBet;
                    }

                    resolvedCount++;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User bets processed",
                resolvedBets = resolvedCount
            });
        }



        // DELETE: api/Bet/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBet(int id)
        {
            var bet = await _context.Bets.FindAsync(id);
            if (bet == null)
                return NotFound();

            _context.Bets.Remove(bet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BetExists(int id)
        {
            return _context.Bets.Any(e => e.ID == id);
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using BetNHL_Web_Api.Data;
//using BetNHL_Web_Api.Models;

//namespace BetNHL_Web_Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BetController : ControllerBase
//    {
//        private readonly BetNHLContext _context;

//        public BetController(BetNHLContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Bet
//        [HttpGet]
//        public async Task<ActionResult<List<BetDTO>>> GetBet()
//        {
//            var betDTOs = await _context.Bets
//                .Select(b => new BetDTO
//                {
//                    ID = b.ID,
//                    DatePlaced = b.DatePlaced,
//                    AmountBet = b.AmountBet,
//                    Odds = b.Odds,
//                    Won = b.Won,
//                    GameId = b.GameId,
//                    Type = b.Type,
//                    TeamPickedID = b.TeamPickedID,
//                    PlayerPickedID = b.PlayerPickedID,
//                    UserId = b.UserId
//                })
//                .ToListAsync();

//            if (betDTOs.Count() > 0)
//            {
//                return betDTOs;
//            }
//            else
//            {
//                return NotFound(new { message = "Error: No betting records found in the database." });
//            }
//        }

//        // GET: api/Bet/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<BetDTO>> GetBet(int id)
//        {
//            //getting singular member 
//            var betDTO = await _context.Bets
//                     .Select(b => new BetDTO
//                     {
//                         ID = b.ID,
//                         DatePlaced = b.DatePlaced,
//                         AmountBet = b.AmountBet,
//                         Odds = b.Odds,
//                         Won = b.Won,
//                         GameId = b.GameId,
//                         Type = b.Type,
//                         TeamPickedID = b.TeamPickedID,
//                         PlayerPickedID = b.PlayerPickedID,
//                         UserId = b.UserId
//                     })
//                .FirstOrDefaultAsync(p => p.ID == id);

//            if (betDTO == null)
//                return NotFound();
//            else
//                return betDTO;

//        }

//        // PUT: api/Bet/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutBet(int id, Bet bet)
//        {
//            if (id != bet.ID)
//            {
//                return BadRequest();
//            }

//            _context.Entry(bet).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BetExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/Bet
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<Bet>> PostBet(Bet bet)
//        {
//            _context.Bet.Add(bet);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetBet", new { id = bet.ID }, bet);
//        }

//        // DELETE: api/Bet/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteBet(int id)
//        {
//            var bet = await _context.Bet.FindAsync(id);
//            if (bet == null)
//            {
//                return NotFound();
//            }

//            _context.Bet.Remove(bet);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool BetExists(int id)
//        {
//            return _context.Bet.Any(e => e.ID == id);
//        }
//    }
//}
