using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Services;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    [Authorize]
    public class MatchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BracketService _bracketService;

        public MatchController(ApplicationDbContext context, BracketService bracketService)
        {
            _context = context;
            _bracketService = bracketService;
        }

        // GET links use 'id' parameter for compatibility with existing links
        // POST links use 'tournamentId' parameter for clarity

        // GET: /Match/ViewBracket/5
        [AllowAnonymous]
        public async Task<IActionResult> ViewBracket(int id) // restored to 'id'
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Players)
                    .ThenInclude(tp => tp.Player)
                .FirstOrDefaultAsync(t => t.TournamentId == id);

            if (tournament == null) return NotFound();

            var bracket = await _bracketService.GetBracketAsync(id);

            var vm = new BracketViewModel
            {
                Tournament = tournament,
                Rounds = bracket ?? new Dictionary<int, List<Match>>(),
                IsAdmin = User.IsInRole("Admin"),
                IsOrganizer = User.IsInRole("Organizer")
            };

            return View(vm);
        }

        // POST: /Match/GenerateBracket
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> GenerateBracket(int tournamentId)
        {
            try
            {
                var matches = await _bracketService.GenerateBracketAsync(tournamentId);
                TempData["Success"] = $"Bracket generated with {matches.Count} matches.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            // redirect using 'id' route parameter for compatibility with links that use id
            return RedirectToAction("ViewBracket", new { id = tournamentId });
        }

        // POST: /Match/AdvanceWinner
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin,Referee")]
        public async Task<IActionResult> AdvanceWinner(int matchId, string winnerId, int tournamentId)
        {
            try
            {
                await _bracketService.AdvanceWinnerAsync(matchId, winnerId);
                TempData["Success"] = "Match result recorded and bracket updated.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("ViewBracket", new { id = tournamentId });
        }

        // GET: /Match/ReportResult/12
        [Authorize(Roles = "Organizer,Admin,Referee")]
        public async Task<IActionResult> ReportResult(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerA)
                .Include(m => m.PlayerB)
                .Include(m => m.Tournament)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
                return NotFound();

            return View(match);
        }

        // POST: /Match/ReportResult/12
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin,Referee")]
        public async Task<IActionResult> ReportResult(int matchId, string winnerId)
        {
            try
            {
                await _bracketService.AdvanceWinnerAsync(matchId, winnerId);
                TempData["Success"] = "Match result recorded and bracket updated.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            return RedirectToAction("ViewBracket", new { id = match?.TournamentId });
        }
    }
}
