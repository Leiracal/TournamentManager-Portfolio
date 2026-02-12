using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Data;
using TournamentManager.Models;
using TournamentManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

// This part is for the database connection.
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Modified this part below so that we can use ApplicationUser which inherits default IdentityUser.
// It also uses custom Roles.

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;   // No need to confirm Email address when log in.
    options.Password.RequiredLength = 6;              // At least 6 characters in Password.
    options.Password.RequireDigit = true;             // At least 1 number in Password.  
    options.Password.RequireNonAlphanumeric = true;   // At least 1 special characters in Password.
    options.Password.RequireUppercase = true;         // At least 1 uppercase letter in Password.
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//.AddDefaultUI();     // The above one line was originally this.
// But this was for buil-in Identity registration or login pages.
// We use our custom designed View/Account/Register page instead.

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BracketService>();
builder.Services.AddScoped<TournamentService>();
builder.Services.AddScoped<EloService>();
builder.Services.AddScoped<SeedUserService>();
builder.Services.AddScoped<SeedTournamentService>();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();    
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope()) 
{
    var services = scope.ServiceProvider;
    var adminService = services.GetRequiredService<AdminService>();
    await adminService.EnsureDefaultRolesAsync();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Ensure roles exist
    var adminService = services.GetRequiredService<AdminService>();
    await adminService.EnsureDefaultRolesAsync();

    // Apply migrations automatically (optional but helpful)
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    // Run user and tournament seeding
    var userSeeder = services.GetRequiredService<SeedUserService>();
    await userSeeder.CreateUsers();
    await userSeeder.CreatePlayers();

    var tournamentSeeder = services.GetRequiredService<SeedTournamentService>();
    await tournamentSeeder.SeedTournamentsAsync();
}

app.Run();