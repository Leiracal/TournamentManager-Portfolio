using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SeedTournamentController : Controller
    {
        private readonly SeedTournamentService _service;

        public SeedTournamentController(SeedTournamentService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            await _service.SeedTournamentsAsync();
            TempData["Success"] = "Tournaments Seeding Complete!";
            return RedirectToAction("Index", "Home");
        }
    }
}
