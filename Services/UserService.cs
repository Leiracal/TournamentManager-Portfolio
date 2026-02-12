using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TournamentManager.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // List all active Players.
        public async Task<IEnumerable<PublicPlayerViewModel>> GetAllActiveProfileAsync()
        {
            try
            {
                return await _context.PlayerProfiles
                     .Include(p => p.ApplicationUser)
                     .Where(p => !p.IsPlayerProfileDeleted && !p.ApplicationUser.IsUserDeleted)
                     .OrderByDescending(p => p.Elo)
                     .Select(profile => new PublicPlayerViewModel
                     {
                         NameToShow = profile.ApplicationUser.NameToShow,
                         ELO = profile.Elo,
                         Bio = profile.Bio
                     })
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading player profiles.");

                return new List<PublicPlayerViewModel>
                {
                    new PublicPlayerViewModel
                    {
                        NameToShow = "Error occurred while loading player info.",
                        Bio = "We are having trouble fetching player data right now. Please try again later.",
                        ELO = 1000
                    }
                };
            }
        }  // End of GetAllActiveProfileAsync()


        // Get a single Player's profile (Player self-view).
        public async Task<EditProfileViewModel?> GetOwnProfileAsync(string userId)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                    .Include(p => p.ApplicationUser)
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == userId && !p.IsPlayerProfileDeleted);

                if (profile == null || profile.ApplicationUser == null)
                    return (null);

                return new EditProfileViewModel
                {
                    DisplayName = profile.ApplicationUser.DisplayName,
                    Bio = profile.Bio,
                    ELO = profile.Elo,
                    NameToShow = !string.IsNullOrWhiteSpace(profile.ApplicationUser.DisplayName)
                    ? profile.ApplicationUser.DisplayName : $"{profile.ApplicationUser.FirstName} {profile.ApplicationUser.LastName}"
                };
            }
            catch
            {
                return null;
            }
        }  // End of GetOwnProfileAsync( )


        // Update profile for player's self-edit. DisplayName and Bio only.
        // If user tries to edit to a new DisplayName but if it is a duplicated, it will be rejected.
        // However, Null and white space duplicate will not be rejected.
        // If user kept it blank, it will keep the same registered DisplayName.
        public async Task<IdentityResult> UpdateProfileAsync(string userId, EditProfileViewModel model)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                   .Include(p => p.ApplicationUser)
                   .FirstOrDefaultAsync(p => p.ApplicationUserId == userId &&
                                            !p.IsPlayerProfileDeleted);

                if (profile == null || profile.ApplicationUser == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "ProfileNotFound",
                        Description = "Profile could not be found."
                    });
                }

                if (!string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    var trimmedDisplayName = model.DisplayName.Trim();

                    bool duplicateDisplayName = await _context.Users.AnyAsync(u => u.Id != userId &&
                                                        u.DisplayName != null &&
                                                        u.DisplayName.ToLower() == trimmedDisplayName.ToLower());

                    if (duplicateDisplayName)
                    {
                        return IdentityResult.Failed(new IdentityError
                        {
                            Code = "DuplicatedDisplayName",
                            Description = "That display name is already taken. Please try again."
                        });
                    }

                    // Update ApplicationUser fields 
                    profile.ApplicationUser.DisplayName = trimmedDisplayName;
                }

                // Update PlayerProfile fields
                profile.Bio = model.Bio;

                await _context.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UpdateFailed",
                    Description = "An error occured while updating the profile."
                });
            }
        } // End of UpdateProfileAsync( )

        // Soft delete for user profile
        public async Task<bool> SoftDeleteAsync(string userId)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                    .Include(p => p.ApplicationUser)
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

                if (profile == null || profile.ApplicationUser == null)
                    return false;

                profile.IsPlayerProfileDeleted = true;
                profile.ApplicationUser.IsUserDeleted = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        } // End of SoftDeleteAsync( )


        // Get a single Player's profile by non-Player like Admin. All fields need to be shown values in view with this method.
        // So this is not suitable for player's self profile edit.
        public async Task<PlayerViewModel?> GetProfileAsync(string userId)
        {
            try
            {
                var profile = await _context.PlayerProfiles
                    .Include(p => p.ApplicationUser)
                    .FirstOrDefaultAsync(p => p.ApplicationUserId == userId && !p.IsPlayerProfileDeleted);

                if (profile == null || profile.ApplicationUser == null)
                    return (null);

                // Add the below code if we wish to get roles to display later.
                // var roles = await _userManager.GetRolesAsync(profile.ApplicationUser);

                return new PlayerViewModel
                {
                    ApplicationUserId = profile.ApplicationUserId!,
                    DisplayName = profile.ApplicationUser.DisplayName,
                    Bio = profile.Bio,
                    ELO = profile.Elo,
                    NameToShow = profile.ApplicationUser.NameToShow
                };
            }
            catch
            {
                return null;
            }
        }  // End of GetProfileAsync( )



    }  // End of class
} // End of namespace