using System.ComponentModel.DataAnnotations;

namespace TournamentManager.ViewModels
{
    public class PlayerViewModel
    {
        // Note: You probably have to create your own custom ViewModel for your view files instead of this one.
        // If you were to use this PlayerViewModel for displaying Player(s) in views for all users or public,
        // Please only show 'NameToShow, Bio, Elo'.
        // Use "NameToShow" instead of FirstName or LastName or FullName.

        // Identity key.
        public string ApplicationUserId { get; set; }
                
        [Required(ErrorMessage = "Please enter your first name.")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [StringLength(30, ErrorMessage = "Display Name cannot be longer than 30 characters.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Display Name cannot be blank spaces.")]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }        
                
        [Display(Name = "Name To Show")]
        public string NameToShow { get; set; }  // Computed in Models/ApplicationUser.cs
        
        [StringLength(500, ErrorMessage ="Bio cannot exceed 500 characters.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }
        
        [Range(0, 3000, ErrorMessage ="Elo rating must be between 0 and 3000.")]
        [Display(Name = "Elo Rating")]
        public int ELO { get; set; } = 1000;  // Default Elo.

        // Role info is display only. Not editable by non-Admin user.

        [Display(Name = "Current Role")]
        public string UserRole { get; set; }

        [Display(Name = "Requested Role")]
        public string RequestedRole { get; set; }

        // Convenience Property for User Interface.
        [Display(Name = "Role Status")]
        public string RoleStatus =>
            UserRole == RequestedRole ? UserRole : $"{UserRole} (Requested: {RequestedRole})";

    }
}
