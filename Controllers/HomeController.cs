using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get recent completed tournaments with winners
            var recentCompleted = await _context.Tournaments
                .Where(t => t.Status == TournamentStatus.Completed)
                .OrderByDescending(t => t.TournamentDate)
                .Take(3)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.Winner)
                .ToListAsync();

            // Determine actual winners (final match winner)
            var champs = recentCompleted
                .Select(t => new TournamentChampionViewModel
                {
                    TournamentName = t.TournamentName,
                    WinnerName = t.Matches
                        .OrderByDescending(m => m.Round)
                        .FirstOrDefault()?.Winner?.DisplayName ?? "Unknown"
                })
                .ToList();

            // Get newest active tournament with bracket
            var latestActive = await _context.Tournaments
                .Where(t => t.Status == TournamentStatus.InProgress)
                .OrderByDescending(t => t.TournamentDate)
                .FirstOrDefaultAsync();

            BracketViewModel? bracketVm = null;

            if (latestActive != null)
            {
                var rounds = await _context.Matches
                    .Include(m => m.PlayerA)
                    .Include(m => m.PlayerB)
                    .Include(m => m.Winner)
                    .Where(m => m.TournamentId == latestActive.TournamentId)
                    .OrderBy(m => m.Round)
                    .ThenBy(m => m.MatchId)
                    .ToListAsync();

                var grouped = rounds
                    .GroupBy(m => m.Round)
                    .ToDictionary(g => g.Key, g => g.ToList());

                bracketVm = new BracketViewModel
                {
                    Tournament = latestActive,
                    Rounds = grouped,
                    IsAdmin = User.IsInRole("Admin"),
                    IsOrganizer = User.IsInRole("Organizer")
                };
            }

            var model = new HomeIndexViewModel
            {
                Champions = champs,
                ActiveTournament = latestActive,
                ActiveBracket = bracketVm
            };

            return View(model);

        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Tournaments()
        {
            return View();
        }
        public IActionResult Bracket()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied() 
        {
            return View();
        }
    }
}
