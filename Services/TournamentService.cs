using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services
{
    public class TournamentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TournamentService> _logger;

        public TournamentService(ApplicationDbContext context, ILogger<TournamentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Get all tournaments with organizer and players
        public async Task<List<Tournament>> GetAllTournamentsAsync()
        {
            try
            {
                return await _context.Tournaments
                    .Include(t => t.Organizer)
                    .Include(t => t.Players)
                        .ThenInclude(tp => tp.Player)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tournaments.");
                throw;
            }
        }

        // Get details for a single tournament
        public async Task<Tournament?> GetTournamentDetailsAsync(int tournamentId)
        {
            try
            {
                return await _context.Tournaments
                    .Include(t => t.Organizer)
                    .Include(t => t.Players)
                        .ThenInclude(tp => tp.Player)
                    .Include(t => t.Matches)
                        .ThenInclude(m => m.PlayerA)
                    .Include(t => t.Matches)
                        .ThenInclude(m => m.PlayerB)
                    .Include(t => t.Matches)
                        .ThenInclude(m => m.Winner)
                    .FirstOrDefaultAsync(t => t.TournamentId == tournamentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tournament details for TournamentId {TournamentId}", tournamentId);
                throw;
            }
        }

        // Register a player for a tournament
        public async Task RegisterPlayerAsync(int tournamentId, string playerId)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Players)
                    .FirstOrDefaultAsync(t => t.TournamentId == tournamentId);

                if (tournament == null)
                    throw new InvalidOperationException("Tournament not found.");

                if (tournament.Status != TournamentStatus.Open)
                    throw new InvalidOperationException("Registration for this tournament is closed.");

                var alreadyRegistered = await _context.TournamentPlayers
                    .AnyAsync(tp => tp.TournamentId == tournamentId && tp.PlayerId == playerId);

                if (alreadyRegistered)
                    throw new InvalidOperationException("You are already registered for this tournament.");

                var tournamentPlayer = new TournamentPlayer
                {
                    TournamentId = tournamentId,
                    PlayerId = playerId
                };

                _context.TournamentPlayers.Add(tournamentPlayer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error registering player {PlayerId} for tournament {TournamentId}",
                    playerId,
                    tournamentId);
                throw;
            }
        }

        // Add a player to a tournament (admin function)
        public async Task AddPlayerAsync(int tournamentId, string playerId)
        {
            try
            {
                var exists = await _context.TournamentPlayers
                    .AnyAsync(tp => tp.TournamentId == tournamentId && tp.PlayerId == playerId);

                if (exists)
                    return; // player already added

                var tpEntry = new TournamentPlayer
                {
                    TournamentId = tournamentId,
                    PlayerId = playerId
                };

                await _context.TournamentPlayers.AddAsync(tpEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error adding player {PlayerId} to tournament {TournamentId}",
                    playerId,
                    tournamentId);
                throw;
            }
        }

        // Remove a player from a tournament (admin function)
        public async Task RemovePlayerAsync(int tournamentId, string playerId)
        {
            try
            {
                var tpEntry = await _context.TournamentPlayers
                    .FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.PlayerId == playerId);

                if (tpEntry == null)
                    return; // nothing to remove

                _context.TournamentPlayers.Remove(tpEntry);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error removing player {PlayerId} from tournament {TournamentId}",
                    playerId,
                    tournamentId);
                throw;
            }
        }

        // Set tournament registration status
        public async Task SetRegistrationStatusAsync(int tournamentId, TournamentStatus newStatus)
        {
            try
            {
                var tournament = await _context.Tournaments.FindAsync(tournamentId);
                if (tournament == null)
                    throw new InvalidOperationException("Tournament not found.");

                tournament.Status = newStatus;
                _context.Update(tournament);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error setting registration status to {Status} for tournament {TournamentId}",
                    newStatus,
                    tournamentId);
                throw;
            }
        }

        // Assign seeds based on Elo ratings
        public async Task AssignSeedsAsync(int tournamentId)
        {
            try
            {
                var players = await _context.TournamentPlayers
                    .Where(tp => tp.TournamentId == tournamentId)
                    .Include(tp => tp.Player)
                    .ToListAsync();

                if (!players.Any())
                    throw new InvalidOperationException("No players registered for seeding.");

                // Load Elo ratings from PlayerProfiles
                var playerIds = players
                    .Where(tp => tp.PlayerId != null)
                    .Select(tp => tp.PlayerId!)
                    .ToList();

                var profiles = await _context.PlayerProfiles
                    .Where(pp => playerIds.Contains(pp.ApplicationUserId!))
                    .ToDictionaryAsync(pp => pp.ApplicationUserId!, pp => pp.Elo);

                // Order by Elo descending, fallback to 1000 if profile missing
                var ordered = players
                    .OrderByDescending(tp => tp.PlayerId != null && profiles.ContainsKey(tp.PlayerId)
                                            ? profiles[tp.PlayerId]
                                            : 1000)
                    .ToList();

                for (int i = 0; i < ordered.Count; i++)
                    ordered[i].Seed = i + 1;

                _context.TournamentPlayers.UpdateRange(ordered);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error assigning seeds for tournament {TournamentId}",
                    tournamentId);
                throw;
            }
        }

        // Create a new tournament
        public async Task CreateTournamentAsync(Tournament tournament)
        {
            try
            {
                if (tournament == null)
                    throw new ArgumentNullException(nameof(tournament));

                _context.Tournaments.Add(tournament);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating tournament {TournamentName}",
                    tournament?.TournamentName);
                throw;
            }
        }

        // Update an existing tournament
        public async Task UpdateTournamentAsync(Tournament tournament)
        {
            try
            {
                if (tournament == null)
                    throw new ArgumentNullException(nameof(tournament));

                var existing = await _context.Tournaments.FindAsync(tournament.TournamentId);
                if (existing == null)
                    throw new InvalidOperationException("Tournament not found.");

                // Copy over only editable fields
                existing.TournamentName = tournament.TournamentName;
                existing.TournamentDate = tournament.TournamentDate;
                existing.Format = tournament.Format;
                existing.Status = tournament.Status;
                existing.RegistrationClosesAt = tournament.RegistrationClosesAt;

                _context.Tournaments.Update(existing);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating tournament {TournamentId}",
                    tournament?.TournamentId);
                throw;
            }
        }

        // Automatically assign Elo-based seeds when registration closes
        public async Task CloseRegistrationAndSeedAsync(int tournamentId)
        {
            try
            {
                await SetRegistrationStatusAsync(tournamentId, TournamentStatus.Closed);
                await AssignSeedsAsync(tournamentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error closing registration and seeding tournament {TournamentId}",
                    tournamentId);
                throw;
            }
        }
    }
}
