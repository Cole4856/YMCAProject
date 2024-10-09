using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using YMCAProject.Models;

namespace YMCAProject.Pages;

public class MembershipModel : PageModel
{

    private readonly IConfiguration _configuration;
    public MembershipModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Member NewMember { get; set; } = new Member();

    public List<Member> memberList { get; set; } = new();

    // Handle GET request
    public void OnGet()
    {
        
    }

    // Handle POST request
    public IActionResult OnPost() {
        if (!ModelState.IsValid)
        {
            return Page(); // Re-render the page if form validation fails
        }

        try {
            string connectionString = _configuration.GetConnectionString("Default");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Insert new member into database
                string sql = "INSERT INTO Members (MemberId, FirstName, LastName, Email, PasswordHash, IsActive, IsMember) VALUES (@MemberId, @FirstName, @LastName, @Email, @PasswordHash, @IsActive, @IsMember)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@MemberId", 1);
                    command.Parameters.AddWithValue("@FirstName", NewMember.FirstName);
                    command.Parameters.AddWithValue("@LastName", NewMember.LastName);
                    command.Parameters.AddWithValue("@Email", NewMember.Email);
                    command.Parameters.AddWithValue("@PasswordHash", NewMember.PasswordHash);
                    command.Parameters.AddWithValue("@IsActive", true);
                    command.Parameters.AddWithValue("@IsMember", true);

                    command.ExecuteNonQuery();
                }

                Console.WriteLine($"Inserting: {NewMember.FirstName}, {NewMember.LastName}, {NewMember.Email}, IsActive: true");
            }

            TempData["Message"] = "Membership sign-up successful!";
            return RedirectToPage("/Membership");  // Redirect after successful sign-up
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            TempData["ErrorMessage"] = "There was an error signing up. Please try again.";
            return Page(); // Stay on the page if there's an error
        } 

    }
}