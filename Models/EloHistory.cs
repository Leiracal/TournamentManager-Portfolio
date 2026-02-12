using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.Models
{
    public class EloHistory
    {
        [Key]
        public int EloHistoryId { get; set; }

        public int? ProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int OldRating { get; set; }
        public int NewRating { get; set; }

        public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    }
}
