using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Services;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    // Note: Many methods inside this PlayerController are used for multiple roles users.
    // So please use [Authorize(Roles = "Admin, Player")] in each method please. Thank you!

    [Authorize] // Please do not specify roles here for above reason. This enables only logged in user can see.
     public class PlayerController : Controller
    {
        private readonly UserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PlayerController(UserService userService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userService = userService;
            _userManager = userManager;
            _context = context;
        }

        // Show all active players.  
        // Note: When you use this method in public view such as main Index page,
        // do not show Full Name, instead, use NameToShow. So that if player choose to show DisplayName, we respect that.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var players = await _userService.GetAllActiveProfileAsync();
            return View(players);  // Pass this list to Index.cshtml
        }

        //Show a single profile.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vm = await _userService.GetOwnProfileAsync(userId);
            if (vm == null)
            {
                ViewBag.ErrorMessage = "Profile not found.";
                return View("Error");
            }
            return View(vm); // Pass to Profile.cshtml
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Player")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Get profile via UserService
            var profileVm = await _userService.GetOwnProfileAsync(userId);
            if (profileVm == null)
            {
                ViewBag.ErrorMessage = "Profile not found.";
                return View("Error");
            }

            // Lookup PlayerProfile directly because UserService doesn't include ELO history
            var playerProfile = await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

            if (playerProfile == null)
                return NotFound("Player profile not found.");

            var history = await _context.EloHistories
                .Where(h => h.ProfileId == playerProfile.ProfileId)
                .OrderByDescending(h => h.MatchDate)
                .ToListAsync();

            var vm = new PlayerEloDashboardViewModel
            {
                DisplayName = profileVm.NameToShow,
                CurrentElo = playerProfile.Elo,
                History = history
            };

            return View(vm);
        }


        // GET: Views/Player/EditProfile.
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vm = await _userService.GetOwnProfileAsync(userId);                   

            if (vm == null)
            {
                ViewBag.ErrorMessage = "Profile not found.";
                return View("Error");
            }           
            return View(vm);   // This is for Views/Player/EditProfile.cshtml
        }

        // POST:  Views/Player/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _userService.UpdateProfileAsync(user.Id, model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile update successfully.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicatedDisplayName")
                {
                    ModelState.AddModelError(nameof(model.DisplayName), error.Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }               
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Home");
        }
    }
}
