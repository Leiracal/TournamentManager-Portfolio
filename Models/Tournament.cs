using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.Models
{
    public class Tournament
    {
        [Key]  // This is to configure this primary key of an Tournament Entity.
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "Please enter the tournament name.")]
        [StringLength(200)]
        public string TournamentName { get; set; }

        [Required(ErrorMessage = "Please enter the tournament date.")]
        [DataType(DataType.Date)]
        public DateTime TournamentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Format { get; set; } = "SingleElimination";

        // Keep this number as 450 to match the default IdentityUser Id nvarchar(450).
        // This should be nullable, so '?' is needed here.        
        [StringLength(450)]  
        public string? OrganizerId { get; set; }

        // NOTE: Role checks should be done in Controllers/Services (Organizer and Admin allowed)
        public ApplicationUser? Organizer { get; set; }

        [Required]
        public TournamentStatus Status { get; set; } = TournamentStatus.Open;

        [DataType(DataType.DateTime)]
        public DateTime? RegistrationClosesAt { get; set; }

        public ICollection<TournamentPlayer> Players { get; set; } = new List<TournamentPlayer>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();

        public bool IsTournamentDeleted { get; set; } = false;

        [NotMapped]  // This means it won't save this property in database, but it's for convenience.
        public bool IsRegistrationOpen =>
            Status == TournamentStatus.Open &&
            (RegistrationClosesAt == null || DateTime.UtcNow <= RegistrationClosesAt.Value);


        // The above code returns 'true' if the registration for this tournament is still open.
        // But it returns 'false' if the tournament is closed already.
        // You could use this property in Views or Controllers with the code like this: 
        // if (tournament.IsRegistrationOpen){ ...code for showing Open or Closed... }

    }
}
