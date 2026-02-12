using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [NotMapped]   // This means this one is not saved in the database. This is for convenience.  The ?? "" is if it were to be null, use empty string.
        public string FullName => $"{FirstName ?? ""} {LastName ?? ""}".Trim();

        // Each ApplicationUser is initially assigned the Player role.
        // Requests for Referee or Organizer roles will then be reviewed and assigned by the Admin.
        
        // NOTE: The 'UserRole' Property is not in this class because 
        // it is handled with the default IdentityRole and the AspNetUserRoles Join Table. 

        [Required]
        [StringLength(50)]
        public string RequestedRole { get; set; } = "Player";
        
        // Display Name is chosen by the User.
        [StringLength(30)]
        public string? DisplayName { get; set; }

        // Note: Some User may not input the DisplayName, so DisplayName could be null.
        // Therefore, when you show it on View or use it in the Controller, try to use NameToShow property instead.
        // (Unless it is a User Registration form.)
        // A ? B : C is like if A is true, use B : or use C if false.
        // The below ?? is if it is not null, use left side.

        public bool IsUserDeleted { get; set; } = false;
        // This property meant to be used when one Player is deleted, but the Tournament info is not deleted. 
        // Then we could use this property to show "Player deleted" instead of null Player causing error for TournamentPlayer and Tournament.


        public string NameToShow =>
            string.IsNullOrWhiteSpace(DisplayName)
                ? FullName ?? "Unknown Player" : DisplayName;

        // Note: The above code means
        // If Player had input DisplayName, use it as NameToShow property.
        // If Player is not null, but does not have DisplayName,
        // then give me the FullName as NameToShow.
        // If Player itself is null, NameToShow is "Unknown Player".

    }
}
