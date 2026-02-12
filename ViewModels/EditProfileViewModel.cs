using System.ComponentModel.DataAnnotations;

namespace TournamentManager.ViewModels
{
    public class EditProfileViewModel
    {
        [StringLength(30, ErrorMessage = "Display Name cannot be longer than 30 characters.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Display Name cannot be blank spaces.")]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        // Read-only display
        [Display(Name = "Elo Rating")]
        public int ELO { get; set; }

        [Display(Name = "Name To Show")]
        public string NameToShow { get; set; } = string.Empty;

    }
}
