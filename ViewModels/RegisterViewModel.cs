using System.ComponentModel.DataAnnotations;

namespace TournamentManager.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
            ErrorMessage = 
            "Password must include at least one uppercase letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter your password again to confirm.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your first name.")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name.")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        // In RegEx, \S matches any non-whitespace character such as not a space, tab, or new line.
        // .* matches any zero or more of any characters including white space.  This means "John Doe" is ok but " " or "\t" is not ok. 
        // Display Name is chosen by the User.

        [StringLength(30, ErrorMessage = "Display Name cannot be longer than 30 characters.")]
        [RegularExpression(@"\S.*", ErrorMessage = "Display Name cannot be blank spaces.")]
        public string? DisplayName { get; set; }  // nullable

        // Dropdown to let users request Organizer/Referee, default Player
        public string RequestedRole { get; set; } = "Player";

    }
}
