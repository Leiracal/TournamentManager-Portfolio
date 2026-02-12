using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrganizerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, Organizer")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        [Authorize(Roles = "Admin, Organizer")]
        public async Task<IActionResult> RegisteredPlayers(string Player)
        {
            var role = await _roleManager.FindByNameAsync("Player");
            
            if (role == null)
            {
                return NotFound("Role not Found");
            }

            var regPlayers = new List<UserRoleViewModel>();
            
            foreach (var user in await _userManager.Users.OrderBy(r => r.LastName).ToListAsync())
            {
                var roles = await _userManager.GetRolesAsync(user);
                var users = _userManager.Users.ToList();

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var userViewModel = new UserRoleViewModel
                    {
                        User = user,
                        Roles = roles.ToList()
                    };
                    regPlayers.Add(userViewModel);
                }
            }
                return View(regPlayers);
        }
    }
}
