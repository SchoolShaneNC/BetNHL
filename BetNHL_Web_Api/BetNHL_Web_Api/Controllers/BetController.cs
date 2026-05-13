using BetNHL_Web_Api.Data;
using BetNHL_Web_Api.Helpers;
using BetNHL_Web_Api.Models;
using BetNHL_Web_Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BetNHL_Web_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BetController : ControllerBase
    {
        private readonly BetNHLContext _context;
        private readonly INhlService _nhlService;
        private readonly IOddsService _oddsService;

        public BetController(BetNHLContext context, INhlService nhlService, IOddsService oddsService)
        {
            _context = context;
            _nhlService = nhlService;
            _oddsService = oddsService;
        }

        // GET: api/Bet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParlayDTO>>> GetParlays()
        {
            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .OrderByDescending(p => p.DatePlaced)
                .ToListAsync();

            var dto = parlays
                .Select(DTOMapper.MapParlayDTO)
                .ToList();

            return Ok(dto);
        }

        // GET: api/Bet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParlayDTO>> GetParlay(int id)
        {
            var parlay = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parlay == null)
                return NotFound();

            return Ok(DTOMapper.MapParlayDTO(parlay));
        }

        // GET: api/Bet/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ParlayDTO>>> GetParlaysByUser(string userId)
        {
            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DatePlaced)
                .ToListAsync();

            if (!parlays.Any())
                return NotFound("No parlays found for this user.");

            var dto = parlays
                .Select(DTOMapper.MapParlayDTO)
                .ToList();

            return Ok(dto);
        }

        // GET: api/Bet/unresolved
        [HttpGet("unresolved")]
        public async Task<ActionResult<IEnumerable<ParlayDTO>>> GetUnresolvedParlays()
        {
            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .Where(p => p.Won == null)
                .ToListAsync();

            var dto = parlays
                .Select(DTOMapper.MapParlayDTO)
                .ToList();

            return Ok(dto);
        }

        // GET: api/Bet/unresolved/me
        [HttpGet("unresolved/me")]
        public async Task<ActionResult<IEnumerable<ParlayDTO>>> GetMyUnresolvedParlays()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .Where(p => p.UserId == userId && p.Won == null)
                .ToListAsync();

            var dto = parlays
                .Select(DTOMapper.MapParlayDTO)
                .ToList();

            return Ok(dto);
        }

        // POST: api/Bet
        [HttpPost]
        public async Task<IActionResult> PostParlay(CreateParlayDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return Unauthorized();

            if (dto.Legs == null || !dto.Legs.Any())
                return BadRequest("Parlay requires at least one leg.");

            if (user.Balance < dto.Stake)
                return BadRequest("Insufficient balance.");

            var parlayLegs = new List<ParlayLeg>();

            foreach (var legDto in dto.Legs)
            {
                decimal odds;

                try
                {
                    odds =  _oddsService.CalculateLegOdds(legDto);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed calculating odds: {ex.Message}");
                }

                var leg = new ParlayLeg
                {
                    Category = legDto.Category,
                    Metric = legDto.Metric,
                    Condition = legDto.Condition,
                    Context = legDto.Context,

                    GameId = legDto.GameId,

                    TeamPickedID = legDto.TeamPickedID,
                    PlayerPickedID = legDto.PlayerPickedID,

                    Line = legDto.Line,

                    Odds = odds
                };

                parlayLegs.Add(leg);
            }

            var combinedOdds = _oddsService.CalculateCombinedParlayOdds(
                    parlayLegs.Select(l => l.Odds).ToList());

            var potentialPayout = dto.Stake * combinedOdds;

            var parlay = new Parlay
            {
                UserId = userId,

                DatePlaced = DateTime.UtcNow,

                Stake = dto.Stake,

                CombinedOdds = combinedOdds,
                PotentialPayout = potentialPayout,

                Won = null,

                Legs = parlayLegs
            };

            user.Balance -= dto.Stake;
            user.TotalMoneyBet += dto.Stake;

            _context.Parlays.Add(parlay);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                parlayId = parlay.Id,

                combinedOddsDecimal = combinedOdds,

                combinedOddsAmerican =
                    _oddsService.ConvertToDisplayOdds(combinedOdds),

                potentialPayout,

                newBalance = user.Balance
            });
        }

        // POST: api/Bet/resolve/me
        [Authorize]
        [HttpPost("resolve/me")]
        public async Task<IActionResult> ResolveMyParlays()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .Where(p => p.UserId == userId && p.Won == null)
                .ToListAsync();

            if (!parlays.Any())
            {
                return Ok(new
                {
                    message = "No unresolved parlays found."
                });
            }

            int resolvedParlays = 0;

            foreach (var parlay in parlays)
            {
                bool unresolved = false;
                bool allLegsWon = true;

                foreach (var leg in parlay.Legs)
                {
                    var game = await _nhlService.FetchGameResultsByID(leg.GameId);

                    if (game.GameState != "OFF")
                    {
                        unresolved = true;
                        break;
                    }

                    bool legWon = false;

                    int totalGoals =
                        game.HomeTeam.Score +
                        game.AwayTeam.Score;

                    int winningTeamId =
                        game.HomeTeam.Score > game.AwayTeam.Score
                            ? game.HomeTeam.Id
                            : game.AwayTeam.Id;

                    var allGoals = game.Summary.Scoring
                        .SelectMany(p => p.Goals)
                        .ToList();

                    var goalCounts = allGoals
                        .GroupBy(g => g.PlayerId)
                        .ToDictionary(g => g.Key, g => g.Count());

                    var assistCounts = allGoals
                        .SelectMany(g => g.AssistPlayerIds)
                        .GroupBy(id => id)
                        .ToDictionary(g => g.Key, g => g.Count());

                    var pointCounts = allGoals
                        .SelectMany(g =>
                            g.AssistPlayerIds.Append(g.PlayerId))
                        .GroupBy(id => id)
                        .ToDictionary(g => g.Key, g => g.Count());

                    switch (leg.Category)
                    {
                        case BetCategory.Team:

                            if (leg.Metric == BetMetric.Goal)
                            {
                                if (leg.Condition == BetCondition.Anytime)
                                {
                                    legWon =
                                        leg.TeamPickedID == winningTeamId;
                                }
                            }

                            break;

                        case BetCategory.Player:

                            if (leg.PlayerPickedID.HasValue)
                            {
                                int playerId = leg.PlayerPickedID.Value;

                                int playerGoals =
                                    goalCounts.TryGetValue(playerId, out var goals)
                                        ? goals
                                        : 0;

                                int playerAssists =
                                    assistCounts.TryGetValue(playerId, out var assists)
                                        ? assists
                                        : 0;

                                int playerPoints =
                                    pointCounts.TryGetValue(playerId, out var points)
                                        ? points
                                        : 0;

                                // Filter powerplay goals if needed
                                if (leg.Context == BetContext.Powerplay)
                                {
                                    var powerplayGoals = allGoals
                                        .Where(g =>
                                            g.Strength == "pp" &&
                                            g.PlayerId == playerId)
                                        .Count();

                                    var powerplayAssists = allGoals
                                        .Where(g => g.Strength == "pp")
                                        .Count(g => g.AssistPlayerIds.Contains(playerId));

                                    switch (leg.Metric)
                                    {
                                        case BetMetric.Goal:
                                            playerGoals = powerplayGoals;
                                            break;

                                        case BetMetric.Assist:
                                            playerAssists = powerplayAssists;
                                            break;

                                        case BetMetric.Point:
                                            playerPoints =
                                                powerplayGoals + powerplayAssists;
                                            break;
                                    }
                                }

                                int stat = 0;

                                switch (leg.Metric)
                                {
                                    case BetMetric.Goal:
                                        stat = playerGoals;
                                        break;

                                    case BetMetric.Assist:
                                        stat = playerAssists;
                                        break;

                                    case BetMetric.Point:
                                        stat = playerPoints;
                                        break;
                                }

                                switch (leg.Condition)
                                {
                                    case BetCondition.Anytime:
                                        legWon = stat >= 1;
                                        break;

                                    case BetCondition.First:

                                        var firstGoal = allGoals.FirstOrDefault();

                                        legWon =
                                            leg.Metric == BetMetric.Goal &&
                                            firstGoal?.PlayerId == playerId;

                                        break;

                                    case BetCondition.Multi:

                                        if (leg.Line.HasValue)
                                        {
                                            legWon = stat >= leg.Line.Value;
                                        }

                                        break;
                                }
                            }

                            break;

                        case BetCategory.Game:

                            if (leg.Metric == BetMetric.TotalGoals &&
                                leg.Line.HasValue)
                            {
                                switch (leg.Condition)
                                {
                                    case BetCondition.Over:
                                        legWon =
                                            totalGoals > leg.Line.Value;
                                        break;

                                    case BetCondition.Under:
                                        legWon =
                                            totalGoals < leg.Line.Value;
                                        break;
                                }
                            }

                            break;
                    }

                    leg.Won = legWon;

                    if (!legWon)
                    {
                        allLegsWon = false;
                    }
                }

                if (unresolved)
                {
                    continue;
                }

                parlay.Won = allLegsWon;

                if (allLegsWon)
                {
                    decimal payout =
                        parlay.Stake * parlay.CombinedOdds;

                    parlay.User.Balance += payout;

                    parlay.User.BetsWon++;

                    parlay.User.TotalMoneyWon += payout;
                }
                else
                {
                    parlay.User.BetsLost++;

                    parlay.User.TotalMoneyLost += parlay.Stake;
                }

                resolvedParlays++;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User parlays resolved.",
                resolvedParlays
            });
        }

        // POST: api/Bet/resolve
        [Authorize]
        [HttpPost("resolve")]
        public async Task<IActionResult> ResolveAllParlays()
        {
            var parlays = await _context.Parlays
                .Include(p => p.User)
                .Include(p => p.Legs)
                .Where(p => p.Won == null)
                .ToListAsync();

            if (!parlays.Any())
            {
                return Ok(new
                {
                    message = "No unresolved parlays found."
                });
            }

            int resolvedParlays = 0;

            foreach (var parlay in parlays)
            {
                bool allLegsWon = true;

                foreach (var leg in parlay.Legs)
                {
                    var game = await _nhlService.FetchGameResultsByID(leg.GameId);

                    if (game.GameState != "OFF")
                    {
                        allLegsWon = false;
                        break;
                    }

                    bool legWon = false;

                    int totalGoals =
                        game.HomeTeam.Score +
                        game.AwayTeam.Score;

                    int winningTeamId =
                        game.HomeTeam.Score > game.AwayTeam.Score
                            ? game.HomeTeam.Id
                            : game.AwayTeam.Id;

                    var goalScorers = game.Summary.Scoring
                        .SelectMany(p => p.Goals)
                        .GroupBy(g => g.PlayerId)
                        .ToDictionary(g => g.Key, g => g.Count());

                    switch (leg.Category)
                    {
                        case BetCategory.Team:

                            if (leg.TeamPickedID == winningTeamId)
                            {
                                legWon = true;
                            }

                            break;

                        case BetCategory.Player:

                            if (leg.PlayerPickedID.HasValue)
                            {
                                int playerGoals =
                                    goalScorers.TryGetValue(
                                        leg.PlayerPickedID.Value,
                                        out var goals)
                                        ? goals
                                        : 0;

                                switch (leg.Condition)
                                {
                                    case BetCondition.Anytime:
                                        legWon = playerGoals >= 1;
                                        break;

                                    case BetCondition.First:
                                        var firstGoal =
                                            game.Summary.Scoring
                                                .SelectMany(p => p.Goals)
                                                .FirstOrDefault();

                                        legWon =
                                            firstGoal?.PlayerId ==
                                            leg.PlayerPickedID.Value;
                                        break;

                                    case BetCondition.Multi:
                                        if (leg.Line.HasValue)
                                        {
                                            legWon =
                                                playerGoals >= leg.Line.Value;
                                        }
                                        break;
                                }
                            }

                            break;

                        case BetCategory.Game:

                            if (leg.Line.HasValue)
                            {
                                switch (leg.Condition)
                                {
                                    case BetCondition.Over:
                                        legWon =
                                            totalGoals > leg.Line.Value;
                                        break;

                                    case BetCondition.Under:
                                        legWon =
                                            totalGoals < leg.Line.Value;
                                        break;
                                }
                            }

                            break;
                    }

                    leg.Won = legWon;

                    if (!legWon)
                    {
                        allLegsWon = false;
                    }
                }

                parlay.Won = allLegsWon;

                if (allLegsWon)
                {
                    decimal payout =
                        parlay.Stake * parlay.CombinedOdds;

                    parlay.User.Balance += payout;

                    parlay.User.BetsWon++;

                    parlay.User.TotalMoneyWon += payout;
                }
                else
                {
                    parlay.User.BetsLost++;

                    parlay.User.TotalMoneyLost += parlay.Stake;
                }

                resolvedParlays++;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "All parlays resolved.",
                resolvedParlays
            });
        }

        // DELETE: api/Bet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParlay(int id)
        {
            var parlay = await _context.Parlays
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parlay == null)
                return NotFound();

            _context.Parlays.Remove(parlay);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParlayExists(int id)
        {
            return _context.Parlays.Any(p => p.Id == id);
        }
    }
}
