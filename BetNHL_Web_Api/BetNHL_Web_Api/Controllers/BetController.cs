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

        public BetController(
            BetNHLContext context,
            INhlService nhlService,
            IOddsService oddsService)
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
