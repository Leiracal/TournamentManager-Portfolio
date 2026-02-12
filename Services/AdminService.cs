using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<AdminService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<AdminManagesRoleViewModel>> GetAllUsersWithRolesAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .OrderBy(u => u.LastName)
                    .ToListAsync();

                var result = new List<AdminManagesRoleViewModel>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var profile = await _context.PlayerProfiles
                        .FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                    result.Add(new AdminManagesRoleViewModel
                    {
                        User = user,
                        Roles = roles.ToList(),
                        RequestedRole = user.RequestedRole
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users with roles.");
                return new List<AdminManagesRoleViewModel>();
            }
        }

        public async Task<List<AdminManagesRoleViewModel>> GetPendingRoleRequestsAsync()
        {
            try
            {
                var seedEmails = new[]
                {
                    "admin@example.com",
                    "organizer@example.com",
                    "referee@example.com"
                };

                var users = await _userManager.Users
                    .Where(u => (u.RequestedRole == "Organizer" || u.RequestedRole == "Referee") 
                    && !seedEmails.Contains(u.Email))
                    .OrderBy(u => u.LastName)
                    .ToListAsync();

                var pending = new List<AdminManagesRoleViewModel>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (!roles.Contains(user.RequestedRole))
                    {
                        var profile = await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                        pending.Add(new AdminManagesRoleViewModel
                        {
                            User = user,
                            Roles = roles.ToList(),
                            RequestedRole = user.RequestedRole
                        });
                    }
                }
                return pending;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending role requests.");
                return new List<AdminManagesRoleViewModel>();
            }
        }

        public async Task<AdminEditUserViewModel?> GetEditUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null) return null;

                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault() ?? "Player";

                var profile = await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                return new AdminEditUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    CurrentRole = currentRole,
                    NewRole = currentRole,
                    DisplayName = user.DisplayName,
                    Bio = profile?.Bio,
                    ELO = profile?.Elo ?? 1000,
                    RequestedRole = user.RequestedRole
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user for edit: {UserId}.", userId);
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(AdminEditUserViewModel model)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);

                if (user == null) return false;

                if (!string.Equals(model.NewRole, model.CurrentRole, StringComparison.OrdinalIgnoreCase))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(user, model.NewRole);
                }

                var profile = await _context.PlayerProfiles.FirstOrDefaultAsync(p => p.ApplicationUserId == user.Id);

                if (profile == null)
                {
                    profile = new PlayerProfile
                    {
                        ApplicationUserId = user.Id,
                        Bio = string.Empty,
                        Elo = 1000,
                        IsPlayerProfileDeleted = false,
                    };
                    _context.PlayerProfiles.Add(profile);
                }

                if (!string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    user.DisplayName = model.DisplayName;
                }
                if (model.Bio != null)
                {
                    profile.Bio = model.Bio;
                }
                profile.Elo = model.ELO;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}.", model.Id);
                return false;
            }
        }

        public async Task EnsureDefaultRolesAsync()
        {
            try
            {
                var rolesToEnsure = new[] { "Admin", "Organizer", "Referee", "Player" };
                foreach (var role in rolesToEnsure)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring default roles.");
            }
        }

        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

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

        public async Task<List<AdminManagesRoleViewModel>> GetDeletedUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.IsUserDeleted).ToListAsync();

                var result = new List<AdminManagesRoleViewModel>();

                foreach (var u in users)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    result.Add(new AdminManagesRoleViewModel
                    {
                        User = u,
                        Roles = roles.ToList(),
                        RequestedRole = u.RequestedRole
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving the deleted users.");
                return new List<AdminManagesRoleViewModel>();
            }
        }

        public async Task<bool> RestoreUserAsync(string id)
        {
            try
            {
                var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Invalid user id: {UserId}", id);
                    return false;
                }
                if (!user.IsUserDeleted)
                {
                    _logger.LogInformation("This user {UserId} is not soft-deleted.", id);
                    return false;
                }

                user.IsUserDeleted = false;                
                user.LockoutEnd = null;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when restoring user: {UserId}", id);
                return false;
            }
        }

    }
}
