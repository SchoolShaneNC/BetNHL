using BetNHL.Models;
using BetNHL.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BetNHL.Data
{
    public  class GameRepository : IGameRepository
    {
        private readonly HttpClient client = new HttpClient();
        //  private readonly AuthService _authService;
        public GameRepository()
        {
            //  _authService = authService;
            client.BaseAddress = NhlApiService.DBUri;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task AddAuthHeader()
        {
            var token = await SecureStorage.GetAsync("auth_token");
          //var token = AuthService.DebugToken; // For testing purposes, use the static DebugToken instead of fetching from SecureStorage

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

        }
        public async Task<List<NhlGameDTO>> GetGames()
        {
           await  AddAuthHeader();

            HttpResponseMessage response = await client.GetAsync("api/NHL/Games");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<NhlGameDTO>>(json);
            
            }
            else
            {
                var ex = NhlApiService.CreateApiException(response);
                throw ex;
            }
        }

        public async Task<List<NhlGamePlayersDTO>> GetGamesWithPlayers(int gameID)
        {
            await AddAuthHeader();
            HttpResponseMessage response = await client.GetAsync($"api/NHL/Games/{gameID}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<NhlGamePlayersDTO>>(json);
            }
            else
            {
                var ex = NhlApiService.CreateApiException(response);
                throw ex;
            }
        }
        public async Task<List<GameOddsDTO>> GetGamesWithOdds()
        {
            await AddAuthHeader();

            var response = await client.GetAsync("api/NHL/Games/WithOdds");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<GameOddsDTO>>(json);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(content);
                throw NhlApiService.CreateApiException(response);
            }
        }
        public async Task<List<PlayerOddsDTO>> GetPlayerOdds(int gameId)
        {
            await AddAuthHeader();

            var response = await client.GetAsync($"api/NHL/Games/{gameId}/PlayerOdds");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PlayerOddsDTO>>(json);
            }
            else
            {
                throw NhlApiService.CreateApiException(response);
            }

        }



        public async Task<GameTeamStandingsDTO> GetGameStandings(int gameId)
        {
            var response = await client.GetFromJsonAsync<GameTeamStandingsDTO>(
                $"api/nhl/GameStandings/{gameId}"
            );

            return response;
        }

        public async Task<List<GameOddsDTO>> GetTodaysGamesWithOdds()
        {
            var response = await client.GetFromJsonAsync<List<GameOddsDTO>>(
                "api/nhl/todaysgameswithodds"
            );

            return response;
        }


    }

}
