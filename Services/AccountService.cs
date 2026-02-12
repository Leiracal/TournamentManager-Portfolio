using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // User creation
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "DuplicateEmail",
                        Description = "Email address is already registered. Please log in instead."
                    });
                }

                // Only reject for duplicates if DisplayName is not NULL or empty string
                if (!string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    bool existingDisplayName = await _context.Users.AnyAsync(u => u.DisplayName.ToLower() == model.DisplayName.ToLower());

                    if (existingDisplayName)
                    {
                        return IdentityResult.Failed(new IdentityError 
                        {
                            Code = "DuplicatedDisplayName",
                            Description = "That display name is already taken."
                        });
                    }
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DisplayName = model.DisplayName,
                    RequestedRole = model.RequestedRole  // "Player" is the default.
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded) return result;

                // To ensure the default 4 roles exist.
                var rolesToEnsure = new[] { "Admin", "Organizer", "Referee", "Player" };
                foreach (var role in rolesToEnsure)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                // "Player" role is default for every user who uses registration form.
                await _userManager.AddToRoleAsync(user, "Player");

                // Auto-create PlayerProfile with nullable Bio and Elo = 1000,
                // so that each user has a default blank Profile even before the user edits it.
                var profile = new PlayerProfile
                {
                    ApplicationUserId = user.Id,
                    Bio = string.Empty,
                    Elo = 1000,
                    IsPlayerProfileDeleted = false
                };
                _context.PlayerProfiles.Add(profile);
                await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering this Email user: {Email}.", model.Email);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RegisterFailed",
                    Description = "Error occurred while registering this account."
                });
            }
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                return await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while login this Email user: {Email}.", model.Email);
                return SignInResult.Failed;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logout.");
            }
        }

        public async Task<bool> SoftDeleteOwnAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.IsUserDeleted = true;

                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;

                await _userManager.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting user: {UserId}.", userId);
                return false;
            }
        }

    }
}
