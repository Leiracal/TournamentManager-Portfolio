using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<TournamentChampionViewModel>? Champions { get; set; }
        public Tournament? ActiveTournament { get; set; }
        public BracketViewModel? ActiveBracket { get; set; }
    }

    public class TournamentChampionViewModel
    {
        public string TournamentName { get; set; } = "";
        public string WinnerName { get; set; } = "";
    }
}
