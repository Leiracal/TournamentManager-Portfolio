using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    public class SeedUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SeedUserService _service;  

        public SeedUserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SeedUserService service)
        {
            _userManager = userManager;
            _context = context;
            _service = service;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CreateUsers()
        {   
            var message = await _service.CreateUsers();

            ViewBag.Message = message;

            return View("~/Views/Shared/CreateUsers.cshtml");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> CreatePlayers()
        {
            var message = await _service.CreatePlayers();
            ViewBag.Message = message;

            return View("~/Views/Admin/CreatePlayers.cshtml");
        }
    }
}
