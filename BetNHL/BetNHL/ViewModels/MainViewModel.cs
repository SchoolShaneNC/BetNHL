using BetNHL.Data;
using BetNHL.Models;
using BetNHL.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace BetNHL.ViewModels
{
    public class MainViewModel
    {
        private readonly IGameRepository _repo;

        //creates games collection
        public ObservableCollection<GameOddsDTO> Games { get; set; } = new();

        private int _selectedGameId;
        public ICommand GoToGameCommand { get; }

        public MainViewModel(IGameRepository repo)
        {
            _repo = repo;

            //gets game id and brings game data over to details page
            GoToGameCommand = new Command<int>(async (gameId) =>
            {
                await Shell.Current.GoToAsync($"{nameof(GameDetailsPage)}?gameId={gameId}");
            });
        }

    
        public async Task LoadData()
        {
            //loads all data to page
            var games = await _repo.GetGamesWithOdds();

            Games.Clear();

            foreach (var game in games)
                Games.Add(game);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
