using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Services;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(AccountService accountService,
                    UserManager<ApplicationUser> userManager,
                    ApplicationDbContext context,
                    SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded)
            {
                // Do NOT Create another default PlayerProfile row here.
                // Because it is already done by AccountService RegisterAsync(). 

                // TempData will show success message even after redirect to the Login page.
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }                

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.LoginAsync(model);
            if (result.Succeeded)
            {

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("Organizer"))
                {
                    return RedirectToAction("Index", "Organizer");
                }
                else if (User.IsInRole("Referee"))
                {
                    return RedirectToAction("Index", "Tournament");
                }
                else if (User.IsInRole("Player"))
                {
                    return RedirectToAction("Dashboard", "Player");
                }

                return View();
        }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);        
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }


        [Authorize]
        [HttpGet]
        public IActionResult SoftDeleteOwnAccount() => View();


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SoftDeleteOwnAccount")]
        public async Task<IActionResult> SoftDeleteOwnAccountPost()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                TempData["ErrorMessage"] = "Your account deactivation failed. Please log in and try again.";
                return RedirectToAction("Login", "Account");
            }
            var result = await _accountService.SoftDeleteOwnAccountAsync(user.Id);

            if (result)
            {
                await _signInManager.SignOutAsync();
                TempData["SuccessMessage"] = "Your account has been successfully deactivated.";
                return RedirectToAction("Login", "Account");
            }
            TempData["ErrorMessage"] = "Unable to deactivate your account. Try again later.";
            return RedirectToAction("SoftDeleteOwnAccount", "Account");
        }

    }
}
