using BetNHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BetNHL.Utilities
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                username,
                password
            });

            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            await SecureStorage.SetAsync("auth_token", result.Token);


            return true;
        }

        public async Task<bool> Register(string username, string password, string confirmPassword)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new
            {
                Username = username,
                Password = password,
                ConfirmPassword = confirmPassword
            });

            return response.IsSuccessStatusCode;
        }


        //the AddAuthHeader method is called before each API request to ensure the JWT token is included in the Authorization header.
        //This way, the server can authenticate the user for protected endpoints.

        public async Task AddAuthHeader()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            await AddAuthHeader();
            return await _httpClient.GetAsync(endpoint);
        }


    }
}


