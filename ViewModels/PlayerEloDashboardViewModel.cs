using System;
using System.Collections.Generic;
using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    public class PlayerEloDashboardViewModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public int CurrentElo { get; set; }

        public List<EloHistory> History { get; set; } = new();
    }
}
