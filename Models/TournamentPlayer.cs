using System.ComponentModel.DataAnnotations;

namespace TournamentManager.Models
{
    public class TournamentPlayer
    {
        // This is for the Linking Table between Tournament and ApplicationUser (Player)

        [Key]
        public string TournamentPlayerId { get; set; } = Guid.NewGuid().ToString();

        // FK to Tournament 
        public int? TournamentId { get; set; }  // Make it nullable FK.
        public Tournament? Tournament { get; set; }

        // FK to Player (ApplicationUser)      
        public string? PlayerId { get; set; }  // This has to be nullable FK. If one Player is deleted, Tournament can still exist.
        public ApplicationUser? Player { get; set; }

        // Used for bracket sorting logic --LM
        public int? Seed { get; set; }

    }
}
