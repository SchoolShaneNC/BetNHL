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
    public class PlayerRepository : IPlayerRepository
    {
            private readonly HttpClient client = new HttpClient();
            //  private readonly AuthService _authService;
            public PlayerRepository()
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
            public async Task<List<NhlPlayerDTO>> GetPlayerByID(int id)
            {
                await AddAuthHeader();

                HttpResponseMessage response = await client.GetAsync($"/player/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<NhlPlayerDTO>>(json);

                }
                else
                {
                    var ex = NhlApiService.CreateApiException(response);
                    throw ex;
                }
            }

            public async Task<List<NhlGamePlayersDTO>> GetPlayerStatsByID(int id)
            {
                await AddAuthHeader();
                HttpResponseMessage response = await client.GetAsync($"/player/stats/{id}");
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

        }
    }

