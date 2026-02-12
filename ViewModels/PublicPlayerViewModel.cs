using System.ComponentModel.DataAnnotations;

namespace TournamentManager.ViewModels
{
    public class PublicPlayerViewModel
    {
        [Display(Name = "Name")]
        public string NameToShow { get; set; }

        [Display(Name = "Elo Rating")]
        public int ELO { get; set; } = 1000;

        [Display(Name = "Bio")]
        public string? Bio { get; set; }
    }
}
