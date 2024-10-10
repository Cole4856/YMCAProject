using YMCAProject;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
string connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add Authentication and specify cookie settings
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login"; // Redirect to login if not authenticated
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set cookie expiration time
    });

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthentication(); // Add this line
app.UseAuthorization(); // Ensure this is after UseAuthentication

app.MapRazorPages();

app.Run();
