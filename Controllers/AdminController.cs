using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TournamentManager.Models;
using TournamentManager.Services;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AdminService _adminService;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            AdminService adminService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminService = adminService;
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public ViewResult AccessDenied()
        {
            return View();
        }

        [HttpGet]  // GET  /Admin/AllUsers
        public async Task<IActionResult> AllUsers()
        {
            var users = await _adminService.GetAllUsersWithRolesAsync();
            return View(users);
        }

        [HttpGet] // Get /Admin/PendingRequests
        public async Task<IActionResult> PendingRequests()
        {
            var pending = await _adminService.GetPendingRoleRequestsAsync();
            return View(pending);
        }

        [HttpGet] // Get /Admin/EditUser/{id}
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("~/Views/Shared/Error.cshtml");
            }

            var model = await _adminService.GetEditUserAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]  // Post   /Admin/EditUser
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminEditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _adminService.UpdateUserAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Unable to update the user. Please try again later.");
                return View(model);
            }

            return RedirectToAction(nameof(AllUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                return RedirectToAction(nameof(AllUsers));
            }


            var success = await _adminService.SoftDeleteUserAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to delete this user.";
            }
            else
            {
                TempData["SuccessMessage"] = "The user you specified was deleted successfully.";
            }
            return RedirectToAction(nameof(AllUsers));
        }

        [HttpGet]
        public async Task<IActionResult> DeletedUsers()
        {
            var deletedUsers = await _adminService.GetDeletedUsersAsync();
            return View(deletedUsers);
        }               

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                return RedirectToAction(nameof(DeletedUsers));
            }

            var success = await _adminService.RestoreUserAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to restore this user account.";
                return RedirectToAction(nameof(DeletedUsers));
            }

            TempData["SuccessMessage"] = "The user you specified has been restored successfully.";     
            return RedirectToAction(nameof(AllUsers));
        }

    }
}
