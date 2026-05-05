using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BetNHL.Data;
using BetNHL.Models;
using BetNHL.Views;
using CommunityToolkit.Maui.Views;

namespace BetNHL.ViewModels
{
    [QueryProperty(nameof(GameId), "gameId")]
    public class GameDetailsViewModel : INotifyPropertyChanged
    {
        private readonly IGameRepository _repo;
        private readonly IBetRepository _betRepo;

        public GameDetailsViewModel(IGameRepository repo, IBetRepository betRepo)
        {
            _repo = repo;
            _betRepo = betRepo;

            OpenPlayerBetCommand = new Command<PlayerOddsDTO>(async (player) =>
            {
                var bet = new CreateBetDTO
                {
                    GameId = GameId,
                    PlayerPickedID = player.PlayerId,
                    Odds = ParseOdds(player.ScoreOdds),
                    Type = BetType.PlayerGoal
                };

                var result = await Shell.Current.ShowPopupAsync(new BetPopup(bet));

                if (result is CreateBetDTO confirmed)
                {
                    await _betRepo.CreateBet(confirmed);
                }
            });

            OpenTeamBetCommand = new Command<NhlTeamStandingDTO>(async (team) =>
            {
                if (GameOdds == null)
                    return;

                decimal odds = team.TeamId == HomeTeam.TeamId
                    ? GameOdds.HomeOddsDecimal
                    : GameOdds.AwayOddsDecimal;

                var bet = new CreateBetDTO
                {
                    GameId = GameId,
                    TeamPickedID = team.TeamId,
                    Odds = odds,
                    Type = BetType.TeamWin
                };

                var result = await Shell.Current.ShowPopupAsync(new BetPopup(bet));

                if (result is CreateBetDTO confirmed)
                {
                    await _betRepo.CreateBet(confirmed);
                }
            });
        }

        public ObservableCollection<PlayerOddsDTO> Players { get; set; } = new();

        private NhlTeamStandingDTO homeTeam;
        public NhlTeamStandingDTO HomeTeam
        {
            get => homeTeam;
            set { homeTeam = value; OnPropertyChanged(nameof(HomeTeam)); }
        }
        private GameOddsDTO gameOdds;
        public GameOddsDTO GameOdds
        {
            get => gameOdds;
            set { gameOdds = value; OnPropertyChanged(nameof(GameOdds)); }
        }

        private NhlTeamStandingDTO awayTeam;
        public NhlTeamStandingDTO AwayTeam
        {
            get => awayTeam;
            set { awayTeam = value; OnPropertyChanged(nameof(AwayTeam)); }
        }


        public ICommand OpenPlayerBetCommand { get; }
        public ICommand OpenTeamBetCommand { get; }


        private int gameId;
        public int GameId
        {
            get => gameId;
            set
            {
                gameId = value;
                _ = LoadData(value);
            }
        }

        //loads all data
        private async Task LoadData(int gameId)
        {
            await LoadPlayers(gameId);
            await LoadTeams(gameId);
            await LoadOdds(gameId); 
        }

        private async Task LoadOdds(int gameId)
        {
            var games = await _repo.GetTodaysGamesWithOdds();

            GameOdds = games.FirstOrDefault(g => g.GameId == gameId);
        }
        private async Task LoadPlayers(int gameId)
        {
            var players = await _repo.GetPlayerOdds(gameId);

            Players.Clear();
            foreach (var p in players)
                Players.Add(p);
        }

        private async Task LoadTeams(int gameId)
        {
            var result = await _repo.GetGameStandings(gameId);

            if (result == null)
                return;

            HomeTeam = result.Home;
            AwayTeam = result.Away;
        }

         //helper method
        private decimal ParseOdds(string odds)
        {
            if (string.IsNullOrWhiteSpace(odds))
                return 1;

            if (odds.StartsWith("+") && int.TryParse(odds[1..], out int val))
                return 1 + (val / 100m);

            if (odds.StartsWith("-") && int.TryParse(odds[1..], out int val2))
                return 1 + (100m / val2);

            return 1;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


