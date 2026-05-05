using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetNHL_Web_Api.Data;
using BetNHL_Web_Api.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BetNHL_Web_Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
    //    var user = await _userManager.Users
    //.Include(u => u.Bets)
    //.FirstOrDefaultAsync(u => u.Id == id);
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.Bets)
                .ToListAsync();

            var userDTOs = users.Select(u => new UserDTO
            {
                ID = u.Id,
                Username = u.UserName,
                Balance = u.Balance,
                BetsWon = u.BetsWon,
                BetsLost = u.BetsLost,
                TotalMoneyBet = u.TotalMoneyBet,
                TotalMoneyWon = u.TotalMoneyWon,
                TotalMoneyLost = u.TotalMoneyLost,

                Bets = u.Bets.Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    Won = b.Won,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID
                }).ToList()
            }).ToList();

            return Ok(userDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserByID(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Bets)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var dto = new UserDTO
            {
                ID = user.Id,
                Username = user.UserName,
                Balance = user.Balance,
                BetsWon = user.BetsWon,
                BetsLost = user.BetsLost,
                TotalMoneyBet = user.TotalMoneyBet,
                TotalMoneyWon = user.TotalMoneyWon,
                TotalMoneyLost = user.TotalMoneyLost,

                Bets = user.Bets.Select(b => new BetDTO
                {
                    ID = b.ID,
                    DatePlaced = b.DatePlaced,
                    AmountBet = b.AmountBet,
                    Odds = b.Odds,
                    Won = b.Won,
                    GameId = b.GameId,
                    Type = b.Type,
                    TeamPickedID = b.TeamPickedID,
                    PlayerPickedID = b.PlayerPickedID
                }).ToList()
            };

            return Ok(dto);
        }


        [HttpGet("me")]
        public async Task<ActionResult<UserDTO>> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            return await GetUserByID(userId);
        }


        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetLeaderboard()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDTOs = users.Select(u => new UserDTO
            {
                ID = u.Id,
                Username = u.UserName,
                Balance = u.Balance,
                BetsWon = u.BetsWon,
                BetsLost = u.BetsLost
            })
            .OrderByDescending(u => u.Balance)
            .ToList();

            return Ok(userDTOs);
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }

}

