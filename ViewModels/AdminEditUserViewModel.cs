using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TournamentManager.ViewModels
{
    public class AdminEditUserViewModel
    {
        [Required]
        public string Id { get; set; } = default!;   // ApplicationUser.Id

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = default!;

        [Display(Name = "Current role")]
        public string CurrentRole { get; set; } = default!;
        
        [Display(Name = "Display name")]
        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
        public string? DisplayName { get; set; }

        [StringLength(300)]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Elo")]
        public int ELO { get; set; } = 1000;
               
        [Display(Name = "Requested role")]
        public string RequestedRole { get; set; } = default!;

        [Required(ErrorMessage = "You must choose a role.")]
        [Display(Name = "New role")]
        public string NewRole { get; set; } = default!;
    }
}
