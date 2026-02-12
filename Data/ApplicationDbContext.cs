using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TournamentManager.Models;

namespace TournamentManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        {
        }

        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentPlayer> TournamentPlayers { get; set; }
        public DbSet<Match> Matches { get; set; }        
        public DbSet<PlayerProfile> PlayerProfiles { get; set; }
        public DbSet<EloHistory> EloHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // This parts is for Seed Ideneity Roles
            //builder.Entity<IdentityRole>().HasData(
            //    new IdentityRole { Name = "Admin", NormalizedName = "ADMIN"},
            //    new IdentityRole { Name = "Organizer", NormalizedName = "ORGANIZER" },
            //    new IdentityRole { Name = "Referee", NormalizedName = "REFEREE" },
            //    new IdentityRole { Name = "Player", NormalizedName = "PLAYER" }
            //    );

            // This part is for TournamentPlayer relationships

            builder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Tournament)
                .WithMany(t => t.Players)
                .HasForeignKey(tp => tp.TournamentId)
                .OnDelete(DeleteBehavior.NoAction);  // This is for preventing cascade delete from TP to Tournament.
            
            builder.Entity<TournamentPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany()
                .HasForeignKey(tp => tp.PlayerId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);  // Player is optional. This is to avoid Query Filter warning.

            // The above two parts are to prevent cascade delete.
            // If one player is deleted, it should not cause automatic deletion of TournamentPlayer column because 
            // it could cause error in that tournament with other players.
            // Same goes to Tournament deletion.
            // Therefore, TournamentPlayer entries should be manually deleted with caution.            

            // These parts is for Match relationships
            builder.Entity<Match>()
             .HasOne(m => m.Tournament)
             .WithMany(t => t.Matches)
             .HasForeignKey(m => m.TournamentId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
              .HasOne(m => m.PlayerA)
              .WithMany()
              .HasForeignKey(m => m.PlayerAId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
               .HasOne(m => m.PlayerB)
               .WithMany()
               .HasForeignKey(m => m.PlayerBId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
               .HasOne(m => m.Winner)
               .WithMany()
               .HasForeignKey(m => m.WinnerId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
               .HasOne(m => m.Referee)
               .WithMany()
               .HasForeignKey(m => m.RefereeId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Match>()
              .HasOne(m => m.NextMatch)
              .WithMany()
              .HasForeignKey(m => m.NextMatchId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PlayerProfile>()
               .HasOne(pp => pp.ApplicationUser)
               .WithMany()
               .HasForeignKey(pp => pp.ApplicationUserId)
               .OnDelete(DeleteBehavior.NoAction)
               .IsRequired(false);

            builder.Entity<Tournament>()
               .HasOne(t => t.Organizer)
               .WithMany()
               .HasForeignKey(t => t.OrganizerId)
               .OnDelete(DeleteBehavior.NoAction)
               .IsRequired(false);

            builder.Entity<ApplicationUser>()
               .HasMany<Tournament>()
               .WithOne(t => t.Organizer)
               .HasForeignKey(t => t.OrganizerId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
               .HasMany<TournamentPlayer>()
               .WithOne(tp => tp.Player)
               .HasForeignKey(tp => tp.PlayerId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ApplicationUser>()
               .HasOne<PlayerProfile>()
               .WithOne(pp => pp.ApplicationUser)
               .HasForeignKey<PlayerProfile>(pp => pp.ApplicationUserId)
               .OnDelete(DeleteBehavior.NoAction);            

            // The below code is to show only IsUserDeleted = false User is shown in the Player list.
            // (Global Query Fileter for Soft Deletion) Because if you just delete one User, it could cause 
            // cascade delete error for Tournament or Matches if Player is deleted.
            // Same for Tournament, Match, PlayerProfile.

            builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsUserDeleted);
            builder.Entity<TournamentPlayer>().HasQueryFilter(tp => tp.Player == null || !tp.Player.IsUserDeleted);            

            builder.Entity<Tournament>().HasQueryFilter(t => !t.IsTournamentDeleted);
            builder.Entity<Match>().HasQueryFilter(m => !m.IsMatchDeleted);

            builder.Entity<PlayerProfile>()
                .HasQueryFilter(pp => !pp.IsPlayerProfileDeleted && 
                (pp.ApplicationUser == null || !pp.ApplicationUser.IsUserDeleted));
            // Only non-deleted Profiles (orphans allowed to avoid view issue.)

            builder.Entity<EloHistory>()
                .HasOne(e => e.PlayerProfile)
                .WithMany(p => p.EloHistory)
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.NoAction);


            // Store TournamentStatus as string for database compatibility with grandfathered entries
            builder.Entity<Tournament>()
                .Property(t => t.Status)
                .HasConversion<string>(); // store enums as strings, not ints
            }
    }
}
