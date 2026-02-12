using Humanizer;
using System;
using System.ComponentModel.DataAnnotations;

namespace TournamentManager.Models
{
    public class PlayerProfile
    {
        [Key]
        public int ProfileId { get; set; }

        //FK to ApplicationUser
        //This has to be nullable FK.If one Player is deleted, Tournament can still exist.
        public string? ApplicationUserId { get; set; }        

        public ApplicationUser? ApplicationUser { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
        public string? Bio { get; set; }

        // Initialize with a default Elo rating 1000 for newly registered Players.
        // Because initialize as 0 or null could potentially cause error
        // if you do not handle null-handling or divide-by-zero carefully.
        [Range(0, int.MaxValue, ErrorMessage = "Elo must be a positive number.")]
        public int Elo { get; set; } = 1000;

        public ICollection<EloHistory> EloHistory { get; set; } = new List<EloHistory>();


        public bool IsPlayerProfileDeleted { get; set; } = false;

    }
}
