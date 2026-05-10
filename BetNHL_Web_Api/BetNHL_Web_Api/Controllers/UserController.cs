using System.Security.Claims;
using BetNHL_Web_Api.Data;
using BetNHL_Web_Api.Helpers;
using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetNHL_Web_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BetNHLContext _context;

        public UserController(UserManager<ApplicationUser> userManager, BetNHLContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Parlays)
                    .ThenInclude(p => p.Legs)
                .AsNoTracking()
                .ToListAsync();

            var userDTOs = users
                .Select(DTOMapper.MapUserDTO)
                .ToList();

            return Ok(userDTOs);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserByID(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Parlays)
                    .ThenInclude(p => p.Legs)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var dto = DTOMapper.MapUserDTO(user);

            return Ok(dto);
        }

        // GET: api/User/me
        [HttpGet("me")]
        public async Task<ActionResult<UserDTO>> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            return await GetUserByID(userId);
        }

        // GET: api/User/leaderboard
        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetLeaderboard()
        {
            var leaderboard = await _context.Parlays
                  .Where(p => p.Won != null)
                  .GroupBy(p => p.UserId)
                  .Select(g => new
                  {
                      UserId = g.Key,

                      NetProfit = g.Sum(x =>
                          x.Won == true
                              ? x.PotentialPayout - x.Stake
                              : -x.Stake),

                      ParlaysWon = g.Count(x => x.Won == true),
                      TotalParlays = g.Count(),

                      WinRate = g.Count(x => x.Won == true) * 1.0 / g.Count()
                  })
                  .OrderByDescending(x => x.NetProfit)
                  .ThenByDescending(x => x.WinRate)
                  .ThenByDescending(x => x.ParlaysWon)
                  .Take(50)
                  .ToListAsync();

            return Ok(leaderboard);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Optional safety check
            if (currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return NoContent();
        }
    }
}