using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.Models
{
    public class Match
    {
        [Key]
        public int MatchId { get; set; }
              
        public int? TournamentId { get; set; }  // nullable FK.
        public Tournament? Tournament { get; set; }

        // These Player A and B are nullable with '?' because it may have 'byes' on either side.
        public string? PlayerAId { get; set; }
        public ApplicationUser? PlayerA { get; set; }

        public string? PlayerBId { get; set; }
        public ApplicationUser? PlayerB { get; set; }

        public string? WinnerId { get; set; }
        public ApplicationUser? Winner { get; set; }

        public string? RefereeId { get; set; }
        public ApplicationUser? Referee { get; set; }

        // Match is one specific game between two players. 
        // Round is the stage. For example, 1 means Round 1, 2 could mean Semifinals, 3 could mean Final).

        // Important Note: BracketService.cs needs to assign correct Round int number for Match object.
        // The EF Core will store as 0 and cause error if the BracketService.cs developer forget to set it.
        // It cannot be initialized as 1 in Match.cs, because it could cause misleading result. 
        // Therefore, it must be done in BracketService.cs.

        [Range(1, int.MaxValue, ErrorMessage = "Round must be 1 or greater.")]
        public int Round { get; set; }

        public int? NextMatchId { get; set; }  // It should be assigned by BracketService.cs. This is for the database, this Id is not meant to be manually typed by an Organizer. 
        
        // [ForeignKey("NextMatchId")]  // Please do not uncomment this line. [ForeignKey] is not necessary here since DbContext file already created FK column.
        public Match? NextMatch { get; set; }  // Navigation Property

        // These above two lines of code could be used by Controllers developer 
        // for the winner of this match to be advance into the next match.

        // Thanks; these are great hooks for auto-advance logic. --L
        public bool IsMatchDeleted { get; set; } = false;
    }
}
