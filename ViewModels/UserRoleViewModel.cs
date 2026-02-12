using TournamentManager.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    public class UserRoleViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }  
    }
}
