using System.Dynamic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;

namespace YMCAProject.Pages.Admin
{

public class AdminDashboard : PageModel
{
    private readonly IConfiguration _configuration;
    public AdminDashboard(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Models.Member> memberList {get; set;} = [];

    // Remove User button
    public IActionResult OnPostDeleteUser(int memberId, string fname, string lname){

        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    // adjust isActive member value
                    string sql = "UPDATE Members " +
                        "SET IsActive = 0 " +
                        "WHERE MemberId = @MemberId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@MemberId", memberId);
                        
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error: " + ex.Message);
            }

        // Show a success message 
        TempData["RegisterMessage"] = $"{fname} {lname} has been successfully removed";
        TempData["MessageType"] = "success";
        
        // Redirect to the same page to show the message
        return RedirectToPage();
    }

    public void OnGet(){
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){

                    connection.Open();

                    string sql = "SELECT * FROM Members";
                    
                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()){
                                Models.Member member = new Models.Member();

                                member.MemberId = reader.GetInt32(0);
                                member.FirstName = reader.GetString(1);
                                member.LastName = reader.GetString(2);
                                member.Email = reader.GetString(3);
                                member.PasswordHash = reader.GetString(4);
                                var active = reader.GetInt16(5);
                                var mem = reader.GetInt16(6);

                                if(active == 1){
                                    member.IsActive = true;
                                }else{
                                    member.IsActive = false;
                                }

                                if(mem == 1){
                                    member.IsMember = true;
                                }else{
                                    member.IsMember = false;
                                }

                                memberList.Add(member);

                            }
                        }
                    }
                }
        } catch(Exception ex)
        {
            Console.WriteLine("We have an error: " + ex.Message);
        }
    }
}
}