using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using YMCAProject.Models;

namespace YMCAProject.Pages;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Models.Staff> staffList {get; set;} = [];

    public string Email { get; set; }

    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {

    }

    public IActionResult OnPost() {
        Console.WriteLine("OnPost method triggered");

        string email = Email;
        string password = Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
            ErrorMessage = "Please enter both email and password";
            return Page();
        }

        string connectionString = _configuration.GetConnectionString("Default");

        try {
            using(var connection = new MySqlConnection(connectionString)) {
                connection.Open();

                string queryMember = "SELECT * FROM Member WHERE Email = @Email AND Password = @Password";

                using (var command = new MySqlCommand(queryMember, connection)) {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    Console.WriteLine(Email);
                    Console.WriteLine(password);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            Console.WriteLine("Login successful for user: " + Email);
                            return RedirectToPage("/Programs");
                        }
                        else {
                            ErrorMessage = "Invalid email or password.";
                            return Page();
                        }
                    }
                }
            }
        }
        catch (Exception ex){
            ErrorMessage = "An error occurred while connecting to the database: " + ex.Message;
            return Page();
        }
    }
}