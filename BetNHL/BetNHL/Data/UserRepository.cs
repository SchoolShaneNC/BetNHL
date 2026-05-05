using BetNHL.Models;
using BetNHL.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Data
{
        public class UserRepository : IUserRepository
        {
            private readonly HttpClient client = new HttpClient();

            public UserRepository()
            {
                client.BaseAddress = NhlApiService.DBUri;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }

            // attach JWT token
            public async Task AddAuthHeader()
            {
                var token = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // GET: api/user
            public async Task<List<UserDTO>> GetUsers()
            {
                await AddAuthHeader();

                var response = await client.GetAsync("api/user");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserDTO>>(json);
                }

                throw NhlApiService.CreateApiException(response);
            }

            // GET: api/user/{id}
            public async Task<UserDTO> GetUserById(string id)
            {
                await AddAuthHeader();

                var response = await client.GetAsync($"api/user/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserDTO>(json);
                }

                throw NhlApiService.CreateApiException(response);
            }

            // GET: api/user/me
            public async Task<UserDTO> GetMyProfile()
            {
                await AddAuthHeader();

                var response = await client.GetAsync("api/user/me");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserDTO>(json);
                }

                throw NhlApiService.CreateApiException(response);
            }

            // GET: api/user/leaderboard
            public async Task<List<UserDTO>> GetLeaderboard()
            {
                await AddAuthHeader();

                var response = await client.GetAsync("api/user/leaderboard");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserDTO>>(json);
                }

                throw NhlApiService.CreateApiException(response);
            }

            // DELETE: api/user/{id}
            public async Task DeleteUser(string id)
            {
                await AddAuthHeader();

                var response = await client.DeleteAsync($"api/user/{id}");

                if (!response.IsSuccessStatusCode)
                    throw NhlApiService.CreateApiException(response);
            }

    }
    }
