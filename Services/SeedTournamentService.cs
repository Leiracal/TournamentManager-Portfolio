using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services
{
    public class SeedTournamentService
    {
        private readonly ApplicationDbContext _context;
        private readonly EloService _eloService;
        private readonly BracketService _bracketService;

        public SeedTournamentService(ApplicationDbContext context, EloService eloService, BracketService bracketService)
        {
            _context = context;
            _eloService = eloService;
            _bracketService = bracketService;
        }

        public async Task SeedTournamentsAsync()
        {
            // Do not reseed if tournaments already exist
            if (await _context.Tournaments.AnyAsync())
                return;

            // Ensure we have at least 16 players
            var players = await _context.Users
                .Where(u => u.RequestedRole == "Player")
                .ToListAsync();

            if (players.Count < 16)
                return; // Player seeding needs to run first

            // Organizer must exist
            var organizer = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == "organizer@example.com");

            if (organizer == null)
                return;

            // TOURNAMENT 1: OPEN (Registration Ongoing)
            var t1 = new Tournament
            {
                TournamentName = "New Year Clash",
                TournamentDate = DateTime.UtcNow.AddDays(14),
                OrganizerId = organizer.Id,
                Status = TournamentStatus.Open,
                Format = "SingleElimination"
            };
            _context.Tournaments.Add(t1);

            // TOURNAMENT 2: CLOSED (Bracket will be generated)
            var t2 = new Tournament
            {
                TournamentName = "Midwinter Cup",
                TournamentDate = DateTime.UtcNow.AddDays(2),
                OrganizerId = organizer.Id,
                Status = TournamentStatus.Closed,
                Format = "SingleElimination"
            };
            _context.Tournaments.Add(t2);
            await _context.SaveChangesAsync();

            // Register 16 players for tournament 2 (add seeding)
            for (int i = 0; i < 16; i++)
            {
                _context.TournamentPlayers.Add(new TournamentPlayer
                {
                    TournamentId = t2.TournamentId,
                    PlayerId = players[i].Id,
                    Seed = i + 1
                });
            }
            await _context.SaveChangesAsync();

            // Generate the bracket for Tournament 2
            await _bracketService.GenerateBracketAsync(t2.TournamentId);

            // TOURNAMENT 3: IN PROGRESS
            var t3 = new Tournament
            {
                TournamentName = "Autumn Open",
                TournamentDate = DateTime.UtcNow.AddDays(-3),
                OrganizerId = organizer.Id,
                Status = TournamentStatus.Closed, // closed first so bracket can generate
                Format = "SingleElimination"
            };
            _context.Tournaments.Add(t3);
            await _context.SaveChangesAsync();

            // Register 16 players
            for (int i = 0; i < 16; i++)
            {
                _context.TournamentPlayers.Add(new TournamentPlayer
                {
                    TournamentId = t3.TournamentId,
                    PlayerId = players[i].Id,
                    Seed = i + 1
                });
            }
            await _context.SaveChangesAsync();

            // Generate bracket
            await _bracketService.GenerateBracketAsync(t3.TournamentId);

            // Mark tournament 3 as In Progress (after bracket is made)
            t3.Status = TournamentStatus.InProgress;
            await _context.SaveChangesAsync();

            // TOURNAMENT 4: COMPLETED
            var t4 = new Tournament
            {
                TournamentName = "Summer Invitational",
                TournamentDate = DateTime.UtcNow.AddDays(-30),
                OrganizerId = organizer.Id,
                Status = TournamentStatus.Closed,
                Format = "SingleElimination"
            };
            _context.Tournaments.Add(t4);
            await _context.SaveChangesAsync();

            // Register 16 players
            for (int i = 0; i < 16; i++)
            {
                _context.TournamentPlayers.Add(new TournamentPlayer
                {
                    TournamentId = t4.TournamentId,
                    PlayerId = players[i].Id,
                    Seed = i + 1
                });
            }
            await _context.SaveChangesAsync();

            // Generate bracket
            var completedMatches = await _bracketService.GenerateBracketAsync(t4.TournamentId);

            // Auto-advance all matches to completion & update ELO
            foreach (var match in completedMatches.Where(m => m.PlayerAId != null && m.PlayerBId != null))
            {
                match.WinnerId = match.PlayerAId; // always let A win for seeding
                await _eloService.UpdateRatingsAsync(match.PlayerAId, match.PlayerBId, match.PlayerAId);
            }
            await _context.SaveChangesAsync();

            // Mark tournament 4 as completed
            t4.Status = TournamentStatus.Completed;
            await _context.SaveChangesAsync();
        }
    }
}
