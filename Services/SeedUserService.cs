using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentManager.Data;
using TournamentManager.Models;

namespace TournamentManager.Services
{
    public class SeedUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SeedUserService> _logger;

        public SeedUserService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<SeedUserService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Create Admin, Organizer, and Referee
        public async Task<string> CreateUsers()
        {
            string[] roleNames = { "Admin", "Organizer", "Referee" };
            bool madeAtLeastOne = false;

            try
            {
                foreach (var role in roleNames)
                {
                    var email = $"{role.ToLower()}@example.com";
                    var user = await _userManager.FindByNameAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = role,
                            LastName = "User",
                            DisplayName = role,
                            RequestedRole = role
                        };

                        var result = await _userManager.CreateAsync(user, "123qwe!QWE");
                        if (result.Succeeded)
                        {
                            madeAtLeastOne = true;
                            await _userManager.AddToRoleAsync(user, role);

                            _context.PlayerProfiles.Add(new PlayerProfile
                            {
                                ApplicationUserId = user.Id,
                                Bio = "",
                                Elo = 1000,
                                IsPlayerProfileDeleted = false
                            });
                        }
                    }
                }

                // Add custom permanent admin account --L
                {
                    var email = "admin@tournamentmanager.local";
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = "Admin",
                            LastName = "Tourney",
                            DisplayName = "AdminTourney",
                            RequestedRole = "Admin"
                        };

                        var result = await _userManager.CreateAsync(user, "123qwe!QWE");
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "Admin");

                            _context.PlayerProfiles.Add(new PlayerProfile
                            {
                                ApplicationUserId = user.Id,
                                Bio = "",
                                Elo = 1000,
                                IsPlayerProfileDeleted = false
                            });
                        }
                        else
                        {
                            // if user exists but not in role
                            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                                await _userManager.AddToRoleAsync(user, "Admin");
                        }

                        await _context.SaveChangesAsync();
                    }
                } //end custom admin account code

                await _context.SaveChangesAsync();

                return madeAtLeastOne
                    ? "Default Admin, Organizer, and Referee accounts created."
                    : "Default user roles already exist.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seed users.");
                return "Error creating default users.";
            }
        }



        // Create 16 Players with Unique Elo
        public async Task<string> CreatePlayers()
        {
            bool newPlayersAdded = false;
            string password = "123qwe!QWE";

            // 16 unique names + visible varied Elo
            var playerData = new (string First, string Last, int Elo)[]
            {
                ("Ava", "Nguyen", 1420), ("Leo", "Romero", 1380),
                ("Mia", "Sato", 1530), ("Eli", "Kowalski", 1470),
                ("Juno", "Park", 1200), ("Sage", "Menendez", 1280),
                ("Aria", "Patel", 1100), ("Max", "Schroeder", 1350),
                ("Kai", "Hernandez", 1600), ("Niko", "Belov", 1000),
                ("Rowan", "Mitchell", 1130), ("Tess", "Bernard", 1070),
                ("Iris", "Fujimoto", 1250), ("Jun", "Matsuda", 1450),
                ("Luna", "Carver", 1300), ("Zane", "Morales", 1385)
            };

            try
            {
                foreach (var p in playerData)
                {
                    var email = $"{p.First.ToLower()}.{p.Last.ToLower()}@example.com";
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            FirstName = p.First,
                            LastName = p.Last,
                            DisplayName = $"{p.First} {p.Last}",
                            RequestedRole = "Player"
                        };

                        var created = await _userManager.CreateAsync(user, password);
                        if (created.Succeeded)
                        {
                            newPlayersAdded = true;
                            await _userManager.AddToRoleAsync(user, "Player");

                            _context.PlayerProfiles.Add(new PlayerProfile
                            {
                                ApplicationUserId = user.Id,
                                Bio = "",
                                Elo = p.Elo,
                                IsPlayerProfileDeleted = false
                            });
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return newPlayersAdded
                    ? "16 players with unique Elo ratings have been created."
                    : "Players already exist in the database.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seed players.");
                return "Error creating default players.";
            }
        }
    }
}
