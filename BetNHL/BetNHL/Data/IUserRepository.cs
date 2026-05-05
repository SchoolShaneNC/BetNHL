using BetNHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Data
{
    public interface IUserRepository
    {
        Task AddAuthHeader();
        Task<List<UserDTO>> GetUsers();
        Task<UserDTO> GetUserById(string id);
        Task<UserDTO> GetMyProfile();
        Task<List<UserDTO>> GetLeaderboard();
        Task DeleteUser(string id);

    }
}
