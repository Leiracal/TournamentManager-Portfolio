using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services
{
    public class EloService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EloService> _logger;
        private const int KFactor = 32;

        public EloService(ApplicationDbContext context, ILogger<EloService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateRatingsAsync(string player1UserId, string player2UserId, string winnerUserId)
        {
            try
            {
                var player1Profile = await _context.PlayerProfiles
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == player1UserId);
                var player2Profile = await _context.PlayerProfiles
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == player2UserId);

                if (player1Profile == null || player2Profile == null)
                    throw new InvalidOperationException("One or both player profiles not found.");

                double r1 = player1Profile.Elo;
                double r2 = player2Profile.Elo;

                double e1 = 1.0 / (1.0 + Math.Pow(10, (r2 - r1) / 400.0));
                double e2 = 1.0 / (1.0 + Math.Pow(10, (r1 - r2) / 400.0));

                double s1 = (winnerUserId == player1UserId) ? 1.0 : 0.0;
                double s2 = (winnerUserId == player2UserId) ? 1.0 : 0.0;

                int oldR1 = player1Profile.Elo;
                int oldR2 = player2Profile.Elo;

                player1Profile.Elo = (int)Math.Round(r1 + KFactor * (s1 - e1));
                player2Profile.Elo = (int)Math.Round(r2 + KFactor * (s2 - e2));

                _context.EloHistories.AddRange(
                    new EloHistory
                    {
                        ProfileId = player1Profile.ProfileId,
                        OldRating = oldR1,
                        NewRating = player1Profile.Elo,
                        MatchDate = DateTime.UtcNow
                    },
                    new EloHistory
                    {
                        ProfileId = player2Profile.ProfileId,
                        OldRating = oldR2,
                        NewRating = player2Profile.Elo,
                        MatchDate = DateTime.UtcNow
                    }
                );

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating Elo ratings. P1 {Player1}, P2 {Player2}, Winner {Winner}",
                    player1UserId,
                    player2UserId,
                    winnerUserId
                );

                throw;
            }
        }
    }
}
