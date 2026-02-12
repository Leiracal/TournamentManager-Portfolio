using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    public class BracketViewModel
    {
        public Tournament Tournament { get; set; } = default!;
        public Dictionary<int, List<Match>> Rounds { get; set; } = new();
        public bool IsAdmin { get; set; }
        public bool IsOrganizer { get; set; }
    }
}