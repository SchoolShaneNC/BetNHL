using BetNHL.Utilities;
using BetNHL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;


namespace BetNHL.Data
{
    public class BetRepository : IBetRepository
    {
        private readonly HttpClient client = new HttpClient();
        public BetRepository()
        {
            //  _authService = authService;
            client.BaseAddress = NhlApiService.DBUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task AddAuthHeader()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

        }
        public async Task<List<Bet>> GetBets()
        {
            await AddAuthHeader();

            HttpResponseMessage response = await client.GetAsync("api/Bet");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Bet>>(json);

            }
            else
            {
                var ex = NhlApiService.CreateApiException(response);
                throw ex;
            }
        }

        public async Task<Bet> GetBetByID(int id)
        {
            await AddAuthHeader();

            HttpResponseMessage response = await client.GetAsync($"api/Bet/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Bet>(json);

            }
            else
            {
                var ex = NhlApiService.CreateApiException(response);
                throw ex;
            }
        }
        public async Task<List<Bet>> GetBetsByUser(string userId)
        {
            await AddAuthHeader();

            HttpResponseMessage response = await client.GetAsync($"/api/bet/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Bet>>(json);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // No bets is not really an error for the ui so i just made it empty
                return new List<Bet>();
            }
            else
            {
                throw NhlApiService.CreateApiException(response);
            }
        }

        // get unresolved bets
        public async Task<List<Bet>> GetUnresolvedBets()
        {
            await AddAuthHeader();

            var response = await client.GetAsync("api/Bet/unresolved");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Bet>>(json);
            }

            throw NhlApiService.CreateApiException(response);
        }

        //Create the bet
        public async Task<(int betId, decimal newBalance)> CreateBet(CreateBetDTO dto)
        {
            await AddAuthHeader();

            var response = await client.PostAsJsonAsync("api/Bet", dto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(json);

                return (
                    (int)result.betId,
                    (decimal)result.newBalance
                );
            }

            throw NhlApiService.CreateApiException(response);
        }

        // Put bets
        public async Task UpdateBet(int id, Bet bet)
        {
            await AddAuthHeader();

            var response = await client.PutAsJsonAsync($"api/Bet/{id}", bet);

            if (!response.IsSuccessStatusCode)
            {
                throw NhlApiService.CreateApiException(response);
            }
        }

        // delete bets
        public async Task DeleteBet(int id)
        {
            await AddAuthHeader();

            var response = await client.DeleteAsync($"api/Bet/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw NhlApiService.CreateApiException(response);
            }
        }

        // Resolve user bets
        public async Task<int> ResolveUserBets()
        {
            await AddAuthHeader();

            var response = await client.PostAsync("api/Bet/resolve/user", null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(json);

                return (int)result.resolvedBets;
            }

            throw NhlApiService.CreateApiException(response);
        }

    }
}
