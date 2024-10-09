using Microsoft.AspNetCore.Identity;
using YMCAProject.Models;
using YMCAProject.UserStore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
string connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Register your custom stores
builder.Services.AddScoped<IUserStore<Member>, MemberStore>();
builder.Services.AddScoped<MemberStore>(provider =>
{
    // You might want to get the connection string from configuration
    return new MemberStore("server=cs341-ymca.cvss442gmfjc.us-east-2.rds.amazonaws.com;uid=admin;pwd=ymca!341!ymca;database=ymca;");
});
builder.Services.AddScoped<IUserStore<Staff>, StaffStore>();
builder.Services.AddScoped<StaffStore>(provider =>
{
    // You might want to get the connection string from configuration
    return new StaffStore("server=cs341-ymca.cvss442gmfjc.us-east-2.rds.amazonaws.com;uid=admin;pwd=ymca!341!ymca;database=ymca;");
});

// Configure Identity with your custom user store
builder.Services.AddIdentity<Member, IdentityRole>() 
    .AddUserStore<MemberStore>() // For members
    .AddDefaultTokenProviders(); // For token generation (password reset, etc.)
builder.Services.AddIdentity<Staff, IdentityRole>() 
    .AddUserStore<StaffStore>() // For staff
    .AddDefaultTokenProviders(); // For token generation (password reset, etc.)

// Optionally configure identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;

    // User settings
    options.User.RequireUniqueEmail = true;
});

// Configure authentication cookies
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies"; // Default to Cookies
})
.AddCookie("MemberCookie", options =>
{
    options.LoginPath = "/Pages/MemberLogin"; //  login path for members
    options.LogoutPath = "/Pages/Logout"; //  logout path for members
})
.AddCookie("StaffCookie", options =>
{
    options.LoginPath = "/Pages/StaffLogin"; //  login path for staff
    options.LogoutPath = "/Pages/Logout"; //  your logout path for staff
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware
app.UseRouting();
app.UseAuthentication(); // Add this line
app.UseAuthorization(); // Ensure this is after UseAuthentication

app.MapRazorPages();

app.Run();
