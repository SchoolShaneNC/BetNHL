using BetNHL_Web_Api.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System;
using System.Net.Http;
using System.Text.Json;
namespace BetNHL_Web_Api.Services
{  // week 1
    public class NhlService : INhlService
    {
        //private readonly HttpClient _httpClient;
        //private readonly IOddsService _oddsService;

        //public NhlService(HttpClient http, IOddsService oddsService)
        //{
        //    _httpClient = http;
        //    _oddsService = oddsService;
        //}

        private readonly HttpClient _httpClient;

        public NhlService(HttpClient http)
        {
            _httpClient = http;
        }
      //  week 1
        public async Task<NhlGamePlayersDTO> GetGameWithPlayersAsync(int gameId)
        {
            var games = await GetTodaysGamesAsync();

            var game = games.FirstOrDefault(g => g.Id == gameId);
            if (game == null)
                return null;

            var players = new List<NhlPlayerDTO>();

            // Fetch both team rosters
            var homePlayers = await GetPlayersFromTeam(game.HomeTeam.Abbreviation, game.HomeTeam.Id);
            var awayPlayers = await GetPlayersFromTeam(game.AwayTeam.Abbreviation, game.AwayTeam.Id);

            players.AddRange(homePlayers);
            players.AddRange(awayPlayers);

            return new NhlGamePlayersDTO
            {
                GameId = game.Id,
                HomeTeam = game.HomeTeam,
                AwayTeam = game.AwayTeam,
                StartTime = game.StartTime,
                Status = game.Status,
                Players = players
            };
        }

        public async Task<NhlGamePlayersDTO> GetGameByIDAsync(int gameId)
        {
            var games = await GetTodaysGamesAsync();

            var game = games.FirstOrDefault(g => g.Id == gameId);
            if (game == null)
                return null;

            var players = new List<NhlPlayerDTO>();

            // Fetch both team rosters
            var homePlayers = await GetPlayersFromTeam(game.HomeTeam.Abbreviation, game.HomeTeam.Id);
            var awayPlayers = await GetPlayersFromTeam(game.AwayTeam.Abbreviation, game.AwayTeam.Id);

            players.AddRange(homePlayers);
            players.AddRange(awayPlayers);

            return new NhlGamePlayersDTO
            {
                GameId = game.Id,
                HomeTeam = game.HomeTeam,
                AwayTeam = game.AwayTeam,
                StartTime = game.StartTime,
                Status = game.Status,
                Players = players
            };
        }




        // week 1
        public Task<NhlPlayerDTO> GetPlayerAsync(int playerId)
        {
            return FetchPlayerById(playerId);


        }
        // week 1
        private async Task<NhlPlayerDTO> FetchPlayerById(int playerId)
        {
            var playerJson = await _httpClient.GetStringAsync($"https://api-web.nhle.com/v1/player/{playerId}/landing");

            using var doc = JsonDocument.Parse(playerJson);

            var player = doc.RootElement;

            return new NhlPlayerDTO
            {
                ID = player.GetProperty("playerId").GetInt32(),
                FirstName = player.GetProperty("firstName").GetProperty("default").GetString(),
                LastName = player.GetProperty("lastName").GetProperty("default").GetString(),
                Position = player.GetProperty("position").GetString(),
                Headshot = player.GetProperty("headshot").GetString(),
                HeroImage = player.GetProperty("heroImage").GetString(),
                TeamID = player.GetProperty("currentTeamId").GetInt32()
            };
        }

        private async Task<NhlPlayerStatsDTO> FetchPlayerStatsById(int playerId)
        {
            var playerJson = await _httpClient.GetStringAsync($"https://api-web.nhle.com/v1/player/{playerId}/landing");

            using var doc = JsonDocument.Parse(playerJson);
            var player = doc.RootElement;

            // Safe position read
            string position = player.TryGetProperty("position", out var posProp)
                ? posProp.GetString()
                : "F";

            //var subSeason = player
            //    .GetProperty("featuredStats")
            //    .GetProperty("regularSeason")
            //    .GetProperty("subSeason");
            JsonElement subSeason = default;
            bool hasStats =
                player.TryGetProperty("featuredStats", out var featuredStats) &&
                featuredStats.TryGetProperty("regularSeason", out var regularSeason) &&
                regularSeason.TryGetProperty("subSeason", out subSeason);

            //If stats don't exist return safe defaults
            if (!hasStats)
            {
                return new NhlPlayerStatsDTO
                {
                    ID = player.GetProperty("playerId").GetInt32(),
                    Position = position,
                    GoalsThisSeason = 0,
                    AssistsThisSeason = 0,
                    ShotsThisSeason = 0,
                    GamesPlayedThisSeason = 0,
                    PlusMinusThisSeason = 0
                };
            }

            // If goalie return zeroed player stats
            if (position == "G")
            {
                if (position == "G")
                {
                    return new NhlPlayerStatsDTO
                    {
                        ID = player.GetProperty("playerId").GetInt32(),
                        Position = position,
                        GoalsThisSeason = 0,
                        AssistsThisSeason = 0,
                        ShotsThisSeason = 0,
                        GamesPlayedThisSeason = subSeason.TryGetProperty("gamesPlayed", out var gp) ? gp.GetInt32() : 0,
                        PlusMinusThisSeason = 0
                    };
                }
            }

         
            return new NhlPlayerStatsDTO
            {
                ID = player.GetProperty("playerId").GetInt32(),
                Position = position,
                GoalsThisSeason = subSeason.TryGetProperty("goals", out var g) ? g.GetInt32() : 0,
                AssistsThisSeason = subSeason.TryGetProperty("assists", out var a) ? a.GetInt32() : 0,
                ShotsThisSeason = subSeason.TryGetProperty("shots", out var s) ? s.GetInt32() : 0,
                GamesPlayedThisSeason = subSeason.TryGetProperty("gamesPlayed", out var gp2) ? gp2.GetInt32() : 0,
                PlusMinusThisSeason = subSeason.TryGetProperty("plusMinus", out var pm) ? pm.GetInt32() : 0
            };
        }

        public Task<NhlTeamDTO> GetTeamAsync(int teamId)
        {
            throw new NotImplementedException();
        }

        public async Task<NhlGameResultsDTO> FetchGameResultsByID(int gameId)
        {
            var json = await _httpClient.GetStringAsync($"https://api-web.nhle.com/v1/gamecenter/{gameId}/landing");

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var gameResult = new NhlGameResultsDTO
            {
                GameState = root.GetProperty("gameState").GetString(),

                AwayTeam = new TeamScoreDTO
                {
                    Id = root.GetProperty("awayTeam").GetProperty("id").GetInt32(),
                    Score = root.GetProperty("awayTeam").GetProperty("score").GetInt32()
                },

                HomeTeam = new TeamScoreDTO
                {
                    Id = root.GetProperty("homeTeam").GetProperty("id").GetInt32(),
                    Score = root.GetProperty("homeTeam").GetProperty("score").GetInt32()
                },

                Summary = new SummaryDTO
                {
                    Scoring = new List<ScoringPeriodDTO>()
                }
            };

            var scoringArray = root
                .GetProperty("summary")
                .GetProperty("scoring");

            foreach (var period in scoringArray.EnumerateArray())
            {
                var periodDTO = new ScoringPeriodDTO
                {
                    Goals = new List<GoalDTO>()
                };

                var goals = period.GetProperty("goals");

                foreach (var goal in goals.EnumerateArray())
                {
                    periodDTO.Goals.Add(new GoalDTO
                    {
                        PlayerId = goal.GetProperty("playerId").GetInt32()
                    });
                }

                gameResult.Summary.Scoring.Add(periodDTO);
            }

            return gameResult;
        }


        public async Task<List<NhlGameDTO>> GetTodaysGamesAsync()
        {
            var json = await _httpClient.GetStringAsync("https://api-web.nhle.com/v1/schedule/now");

            using var doc = JsonDocument.Parse(json);

            var gamesList = new List<NhlGameDTO>();

            var gameWeeks = doc.RootElement.GetProperty("gameWeek");

            foreach (var day in gameWeeks.EnumerateArray())
            {
                var games = day.GetProperty("games");

                foreach (var game in games.EnumerateArray())
                {

                    DateTime gameUtc = game.GetProperty("startTimeUTC").GetDateTime();
                    DateTime gameLocal = gameUtc.ToLocalTime();
                    var today = DateTime.Now.Date;

                    if (gameLocal.Date != today)
                        continue;

                    var home = game.GetProperty("homeTeam");
                    var away = game.GetProperty("awayTeam");

                    var homeName = home.GetProperty("placeName").GetProperty("default").GetString() +
                                   " " +
                                   home.GetProperty("commonName").GetProperty("default").GetString();

                    var awayName = away.GetProperty("placeName").GetProperty("default").GetString() +
                                   " " +
                                   away.GetProperty("commonName").GetProperty("default").GetString();

                    var dto = new NhlGameDTO
                    {
                        Id = game.GetProperty("id").GetInt32(),
                        StartTime = gameUtc,
                        Status = game.GetProperty("gameState").GetString(),

                        HomeTeam = new NhlTeamDTO
                        {
                            Id = home.GetProperty("id").GetInt32(),
                            Name = homeName,
                            Abbreviation = home.GetProperty("abbrev").GetString(),
                            LogoUrl = home.GetProperty("logo").GetString()
                        },

                        AwayTeam = new NhlTeamDTO
                        {
                            Id = away.GetProperty("id").GetInt32(),
                            Name = awayName,
                            Abbreviation = away.GetProperty("abbrev").GetString(),
                            LogoUrl = away.GetProperty("logo").GetString()
                        }
                    };

                    gamesList.Add(dto);

                }

            }

            return gamesList;
            //   return gamesList.OrderBy(g => g.StartTime).ToList();  could use to ensure proper order but 
            // the API seems to return in order already and this would be an extra step that may not be necessary

        }

        private async Task<List<NhlPlayerDTO>> GetPlayersFromTeam(string abbrev, int teamId)
        {
            var url = $"https://api-web.nhle.com/v1/roster/{abbrev}/current";
            var json = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var players = new List<NhlPlayerDTO>();

            //handles possitions forwards, defensemen, goalies
            foreach (var group in root.EnumerateObject())
            {
                if (group.Value.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var player in group.Value.EnumerateArray())
                {
                    players.Add(new NhlPlayerDTO
                    {
                        ID = player.GetProperty("id").GetInt32(),
                        FirstName = player.GetProperty("firstName").GetProperty("default").GetString(),
                        LastName = player.GetProperty("lastName").GetProperty("default").GetString(),
                        Position = group.Name, 
                        TeamID = teamId
                    });
                }
            }

            return players;
        }

        private async Task<ICollection<NhlPlayerDTO>> FetchPlayersForGame(int gameId)
        {
            // Call NHL roster API, map JSON → NhlPlayerDTO
            return new List<NhlPlayerDTO>();
        }

        public Task<NhlPlayerStatsDTO> GetPlayerStatsAsync(int playerId)
        {
            return FetchPlayerStatsById(playerId);
        }


        public async Task<List<NhlTeamStandingDTO>> GetStandingsAsync()
        {
            var json = await _httpClient.GetStringAsync("https://api-web.nhle.com/v1/standings/now");

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var teams = new List<NhlTeamStandingDTO>();

            foreach (var team in root.GetProperty("standings").EnumerateArray())
            {
                int GetInt(string key) =>
                    team.TryGetProperty(key, out var val) ? val.GetInt32() : 0;

                string GetNestedString(string key) =>
                    team.TryGetProperty(key, out var val)
                        ? val.GetProperty("default").GetString()
                        : null;

                teams.Add(new NhlTeamStandingDTO
                {
                    // ⚠️ may or may not exist depending on endpoint version
                    TeamId = team.TryGetProperty("teamId", out var id) ? id.GetInt32() : 0,

                    Abbreviation = team.GetProperty("teamAbbrev")
                                        .GetProperty("default")
                                        .GetString(),

                    TeamName = team.GetProperty("teamName")
                                    .GetProperty("default")
                                    .GetString(),

                    GamesPlayed = GetInt("gamesPlayed"),
                    Wins = GetInt("wins"),
                    Losses = GetInt("losses"),
                    Otl = GetInt("otLosses"),

                    Points = GetInt("points"),

                    GoalsFor = GetInt("goalFor"),
                    GoalsAgainst = GetInt("goalAgainst"),
                    GoalDifferential = GetInt("goalDifferential"),

                    PointsPercentage = team.TryGetProperty("pointPctg", out var pct)
                        ? pct.GetDouble()
                        : 0
                });
            }

            return teams;
        }

        public async Task<NhlTeamStandingDTO> GetTeamStandingAsync(string teamAbbr)
        {
            var standings = await GetStandingsAsync();

            return standings.FirstOrDefault(t => t.Abbreviation == teamAbbr);
        }
        public async Task<GameTeamStandingsDTO> GetGameTeamStandingsAsync(string homeTeamAbbr, string awayTeamAbbr)
        {
            var standings = await GetStandingsAsync();

            var home = standings.FirstOrDefault(t => t.Abbreviation == homeTeamAbbr);
            var away = standings.FirstOrDefault(t => t.Abbreviation == awayTeamAbbr);

            return new GameTeamStandingsDTO
            {
                Home = home,
                Away = away
            };
        }


        public async Task<List<GameOddsDTO>> GetTodaysGamesWithOddsAsync()
        {
            var games = await GetTodaysGamesAsync();
            var standings = await GetStandingsAsync();


            var result = new List<GameOddsDTO>();

            foreach (var game in games)
            {
                var homeAbbrev = game.HomeTeam.Abbreviation;
                var awayAbbrev = game.AwayTeam.Abbreviation;

                var homeDecimal = new OddsService().CalculateTeamWinOdds(
                    standings, homeAbbrev, awayAbbrev, homeAbbrev);

                var awayDecimal = new OddsService().CalculateTeamWinOdds(
                    standings, homeAbbrev, awayAbbrev, awayAbbrev);

                result.Add(new GameOddsDTO
                {
                    GameId = game.Id,
                    HomeTeamAbbr = homeAbbrev,
                    AwayTeamAbbr = awayAbbrev,
                    StartTime = game.StartTime,

                    HomeOddsDecimal = homeDecimal,
                    HomeDisplayOdds = new OddsService().ConvertToDisplayOdds(homeDecimal),

                    AwayOddsDecimal = awayDecimal,
                    AwayDisplayOdds = new OddsService().ConvertToDisplayOdds(awayDecimal)
    });
            }

            return result;
        }
    
        public async Task<List<PlayerOddsDTO>> GetGamePlayerOddsAsync(int gameId)
        {
            var game = await GetGameWithPlayersAsync(gameId);

            if (game == null)
                return new List<PlayerOddsDTO>();

            var allPlayers = game.Players; 

            var oddsService = new OddsService(); 

            var result = new List<PlayerOddsDTO>();

            foreach (var player in allPlayers)
            {
                await Task.Delay(120); // to avoid rate limit

                var stats = await GetPlayerStatsAsync(player.ID);

                var oddsDecimal = oddsService.CalculatePlayerGoalOdds(player, stats);

                var opponentAbbr =
                    player.TeamID == game.HomeTeam.Id
                    ? game.AwayTeam.Abbreviation
                    : game.HomeTeam.Abbreviation;

                result.Add(new PlayerOddsDTO
                {
                    PlayerId = player.ID,
                    DisplayName = $"{player.FirstName[0]}. {player.LastName}",
                    ScoreOdds = oddsService.ConvertToDisplayOdds(oddsDecimal),

                    TeamAbbr = player.TeamID == game.HomeTeam.Id ? game.HomeTeam.Abbreviation : game.AwayTeam.Abbreviation,

                    OppAbbr = opponentAbbr,
                    GameTime = game.StartTime.ToShortTimeString()
                });
            }

            return result;
        }

    }
}
