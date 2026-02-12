using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Models;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Authorize]
    public class TournamentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TournamentService _tournamentService;

        public TournamentController(
            UserManager<ApplicationUser> userManager,
            TournamentService tournamentService)
        {
            _userManager = userManager;
            _tournamentService = tournamentService;
        }

        // GET: /Tournament/
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync();
            return View(tournaments);
        }

        // GET: /Tournament/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var tournament = await _tournamentService.GetTournamentDetailsAsync(id);
            if (tournament == null)
                return NotFound();

            return View(tournament);
        }

        // POST: /Tournament/Register/5
        [HttpPost]
        [Authorize(Roles = "Player,Admin")]
        public async Task<IActionResult> Register(int tournamentId)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                await _tournamentService.RegisterPlayerAsync(tournamentId, userId);
                TempData["Success"] = "You have been registered successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: /Tournament/SetStatus/5
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> SetStatus(int tournamentId, TournamentStatus newStatus)
        {
            try
            {
                if (newStatus == TournamentStatus.Closed)
                {
                    // Close registration and assign Elo seeds automatically
                    await _tournamentService.CloseRegistrationAndSeedAsync(tournamentId);
                    TempData["Success"] = "Tournament registration closed and players have been seeded based on Elo.";
                }
                else
                {
                    // For other status changes, just set the status
                    await _tournamentService.SetRegistrationStatusAsync(tournamentId, newStatus);
                    TempData["Success"] = $"Tournament status updated to {newStatus}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // GET: /Tournament/Create
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Tournament/Create
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create(Tournament tournament)
        {
            if (!ModelState.IsValid)
                return View(tournament);

            tournament.OrganizerId = _userManager.GetUserId(User);
            tournament.Status = TournamentStatus.Open;

            try
            {
                await _tournamentService.CreateTournamentAsync(tournament);
                TempData["Success"] = "Tournament created successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(tournament);
            }

            return RedirectToAction("Index");
        }

        // GET: /Tournament/Edit/5
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var tournament = await _tournamentService.GetTournamentDetailsAsync(id);
            if (tournament == null)
                return NotFound();

            if (!User.IsInRole("Organizer") && !User.IsInRole("Admin"))
                return Forbid();

            // Populate all users for dropdown
            var allUsers = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            ViewBag.AllUsers = allUsers;

            return View(tournament);
        }


        // POST: /Tournament/Edit/5
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(int id, Tournament tournament)
        {
            if (id != tournament.TournamentId)
                return BadRequest();

            var existing = await _tournamentService.GetTournamentDetailsAsync(id);
            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(tournament);

            try
            {
                await _tournamentService.UpdateTournamentAsync(tournament);
                TempData["Success"] = "Tournament updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(tournament);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> AddPlayer(int tournamentId, string playerId)
        {
            var tournament = await _tournamentService.GetTournamentDetailsAsync(tournamentId);
            if (tournament == null) return NotFound();
          
            await _tournamentService.AddPlayerAsync(tournamentId, playerId);
            TempData["Success"] = "Player added successfully.";

            return RedirectToAction("Edit", new { id = tournamentId });
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> RemovePlayer(int tournamentId, string playerId)
        {
            var tournament = await _tournamentService.GetTournamentDetailsAsync(tournamentId);
            if (tournament == null) return NotFound();           

            await _tournamentService.RemovePlayerAsync(tournamentId, playerId);
            TempData["Success"] = "Player removed successfully.";

            return RedirectToAction("Edit", new { id = tournamentId });
        }

    }
}
