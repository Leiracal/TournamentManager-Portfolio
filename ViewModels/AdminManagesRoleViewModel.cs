using System.Collections.Generic;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager.ViewModels
{
    public class AdminManagesRoleViewModel
    {
        public ApplicationUser User { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
        public string? RequestedRole { get; set; }

        // Moved this logic in the AdminService.cs.
        //public bool IsPending =>
        //    (RequestedRole == "Organizer" || RequestedRole == "Referee") && Roles.Contains("Player");
    }
}