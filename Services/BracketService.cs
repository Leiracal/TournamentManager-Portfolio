using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services
{
    public class BracketService
    {
        private readonly ApplicationDbContext _context;
        private readonly EloService _eloService;
        private readonly ILogger<BracketService> _logger;

        public BracketService(ApplicationDbContext context, EloService eloService, ILogger<BracketService> logger)
        {
            _context = context;
            _eloService = eloService;
            _logger = logger;
        }

        public async Task<List<Match>> GenerateBracketAsync(int tournamentId)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Players)
                        .ThenInclude(tp => tp.Player)
                    .FirstOrDefaultAsync(t => t.TournamentId == tournamentId);

                if (tournament == null)
                    throw new InvalidOperationException("Tournament not found.");

                if (tournament.Status != TournamentStatus.Closed &&
                    tournament.Status != TournamentStatus.PendingBracket)
                    throw new InvalidOperationException("Tournament must be closed before generating a bracket.");

                if (await _context.Matches.AnyAsync(m => m.TournamentId == tournamentId && !m.IsMatchDeleted))
                    throw new InvalidOperationException("A bracket already exists for this tournament.");

                var players = tournament.Players
                    .Where(p => p.PlayerId != null)
                    .OrderBy(p => p.Seed ?? Guid.NewGuid().GetHashCode())
                    .Select(p => p.PlayerId!)
                    .ToList();

                if (players.Count < 2)
                    throw new InvalidOperationException("At least two players are required.");

                int totalPlayers = players.Count;

                // Next power of two for bracket size
                int bracketSize = 1;
                while (bracketSize < totalPlayers)
                    bracketSize *= 2;

                int numberOfByes = bracketSize - totalPlayers;

                // Simple way to distribute byes: pad with nulls (BYE)
                var seededList = new List<string?>(players);
                for (int i = 0; i < numberOfByes; i++)
                    seededList.Add(null);

                // Round 1 creates bracketSize/2 matches
                var allRounds = new List<List<Match>>();

                var firstRound = new List<Match>();
                for (int i = 0; i < bracketSize; i += 2)
                {
                    var a = seededList[i];
                    var b = seededList[i + 1];

                    var match = new Match
                    {
                        TournamentId = tournamentId,
                        Round = 1,
                        PlayerAId = a,
                        PlayerBId = b
                    };

                    // Auto-advance bye matches
                    if (a != null && b == null)
                    {
                        match.WinnerId = a;
                    }
                    else if (a == null && b != null)
                    {
                        match.WinnerId = b;
                    }
                    else
                    {
                        match.WinnerId = null;
                    }

                    firstRound.Add(match);
                }
                allRounds.Add(firstRound);

                int totalRounds = (int)Math.Log2(bracketSize);

                // Create empty future rounds
                for (int r = 2; r <= totalRounds; r++)
                {
                    int matchCount = bracketSize / (int)Math.Pow(2, r);
                    var round = new List<Match>();
                    for (int i = 0; i < matchCount; i++)
                        round.Add(new Match
                        {
                            TournamentId = tournamentId,
                            Round = r
                        });
                    allRounds.Add(round);
                }

                // Link matches to their parents
                for (int r = 0; r < allRounds.Count - 1; r++)
                {
                    var current = allRounds[r];
                    var next = allRounds[r + 1];
                    for (int i = 0; i < current.Count; i++)
                    {
                        var child = current[i];
                        var parent = next[i / 2];
                        child.NextMatch = parent;
                    }
                }

                // Save the bracket and record indexes for first-round matches
                var flat = allRounds.SelectMany(x => x).ToList();

                // map first-round match object to index within first round so we can know left/right later
                var firstRoundIndex = new Dictionary<Match, int>();
                for (int i = 0; i < firstRound.Count; i++)
                    firstRoundIndex[firstRound[i]] = i;

                await _context.Matches.AddRangeAsync(flat);
                await _context.SaveChangesAsync();

                // After save, EF should have populated MatchId/NextMatchId. Use the saved entities (flat list).
                // Apply bye-advancements immediately to their correct parent slot (left/right).
                foreach (var kv in firstRoundIndex)
                {
                    var childBeforeSave = kv.Key;
                    var childIndex = kv.Value;

                    // find the saved child in flat (by round & players or by object identity - Match objects are the same instances)
                    var savedChild = flat.FirstOrDefault(m => m.MatchId == childBeforeSave.MatchId);
                    if (savedChild == null) continue;

                    if (string.IsNullOrEmpty(savedChild.WinnerId)) continue; // no bye/winner to advance

                    if (!savedChild.NextMatchId.HasValue) continue;

                    var parent = flat.FirstOrDefault(m => m.MatchId == savedChild.NextMatchId.Value);
                    if (parent == null) continue;

                    bool isLeftChild = (childIndex % 2 == 0); // even -> left
                    if (isLeftChild)
                    {
                        if (string.IsNullOrEmpty(parent.PlayerAId))
                            parent.PlayerAId = savedChild.WinnerId;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(parent.PlayerBId))
                            parent.PlayerBId = savedChild.WinnerId;
                    }
                }

                // Persist parent assignments
                await _context.SaveChangesAsync();

                // Set tournament in progress
                tournament.Status = TournamentStatus.InProgress;
                await _context.SaveChangesAsync();

                return flat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bracket for TournamentId {TournamentId}", tournamentId);
                throw;
            }
        }

        public async Task AdvanceWinnerAsync(int matchId, string winnerId)
        {
            try
            {
                var match = await _context.Matches
                    .Include(m => m.Tournament)
                    .Include(m => m.PlayerA)
                    .Include(m => m.PlayerB)
                    .FirstOrDefaultAsync(m => m.MatchId == matchId);

                if (match == null)
                    throw new InvalidOperationException("Match not found.");

                if (match.IsMatchDeleted)
                    throw new InvalidOperationException("Cannot update a deleted match.");

                if (match.Tournament == null || match.Tournament.Status != TournamentStatus.InProgress)
                    throw new InvalidOperationException("Tournament is not currently active.");

                // Prevent re-setting the same winner redundantly
                if (match.WinnerId == winnerId)
                {
                    // nothing to do, but return gracefully
                    return;
                }

                match.WinnerId = winnerId;
                await _context.SaveChangesAsync();

                // Elo update only if both players present for that match
                if (!string.IsNullOrEmpty(match.PlayerAId) && !string.IsNullOrEmpty(match.PlayerBId))
                {
                    await _eloService.UpdateRatingsAsync(
                        match.PlayerAId!,
                        match.PlayerBId!,
                        winnerId
                    );
                }

                // If there's a next match, place winner into the correct slot based on whether this child was left or right.
                if (match.NextMatchId.HasValue)
                {
                    // Determine this match's index within its round (order by MatchId)
                    var siblings = await _context.Matches
                        .Where(m => m.TournamentId == match.TournamentId && m.Round == match.Round && !m.IsMatchDeleted)
                        .OrderBy(m => m.MatchId)
                        .ToListAsync();

                    int childIndex = siblings.FindIndex(s => s.MatchId == match.MatchId);
                    if (childIndex < 0)
                    {
                        // fallback: just put into first empty slot
                        var fallbackParent = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == match.NextMatchId.Value);
                        if (fallbackParent != null)
                        {
                            if (string.IsNullOrEmpty(fallbackParent.PlayerAId))
                                fallbackParent.PlayerAId = winnerId;
                            else if (string.IsNullOrEmpty(fallbackParent.PlayerBId))
                                fallbackParent.PlayerBId = winnerId;
                        }
                    }
                    else
                    {
                        bool isLeftChild = (childIndex % 2 == 0); // even => left
                        var parent = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == match.NextMatchId.Value);
                        if (parent != null)
                        {
                            if (isLeftChild)
                            {
                                if (string.IsNullOrEmpty(parent.PlayerAId))
                                    parent.PlayerAId = winnerId;
                                // else: slot already filled, leave it (do not overwrite)
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(parent.PlayerBId))
                                    parent.PlayerBId = winnerId;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Check if tournament is complete (no matches without a winner)
                var tournamentId = match.TournamentId;
                var remainingActive = await _context.Matches
                    .Where(m => m.TournamentId == tournamentId && !m.IsMatchDeleted)
                    .AnyAsync(m => m.WinnerId == null);

                if (!remainingActive)
                {
                    match.Tournament!.Status = TournamentStatus.Completed;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error advancing winner. MatchId {MatchId}, WinnerId {WinnerId}",
                    matchId,
                    winnerId);
                throw;
            }
        }

        public async Task<Dictionary<int, List<Match>>> GetBracketAsync(int tournamentId)
        {
            try
            {
                var matches = await _context.Matches
                    .Include(m => m.PlayerA)
                    .Include(m => m.PlayerB)
                    .Include(m => m.Winner)
                    .Where(m => m.TournamentId == tournamentId && !m.IsMatchDeleted)
                    .OrderBy(m => m.Round)
                    .ThenBy(m => m.MatchId) // ensure deterministic ordering within rounds
                    .ToListAsync();

                return matches
                    .GroupBy(m => m.Round)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bracket for TournamentId {TournamentId}", tournamentId);
                throw;
            }
        }
    }
}
