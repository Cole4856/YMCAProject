using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace YMCAProject.Pages;

public class ProgramsModel : PageModel
{
    // private readonly ILogger<ProgramsModel> _logger;

    // public ProgramsModel(ILogger<ProgramsModel> logger)
    // {
    //     _logger = logger;
    // }

    private readonly IConfiguration _configuration;
    public ProgramsModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Models.Programs> programList {get; set;} = [];

    // Register button
    public IActionResult OnPostRegisterClass(string className, int programId)
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    string sql = "SELECT Count(*) FROM Member_Programs " +
                        "WHERE MemberId = @MemberId AND ProgramID = @ProgramId ";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@MemberId", int.Parse(User.FindFirst("UserId")?.Value));            //change to memberId!!!!!!!!
                        command.Parameters.AddWithValue("@ProgramId", programId); 

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            reader.Read();
                            int count = reader.GetInt32(0);

                            if (count > 0){
                                // Show a failure message 
                                TempData["RegisterMessage"] = $"Error: {User.Identity.Name} is already registered for {className}";
                                TempData["MessageType"] = "error";
                                
                                // Redirect to the same page to show the message
                                return RedirectToPage();
                            }

                        }
                    }

                    sql = "Insert INTO Member_Programs " +
                        "(MemberId, ProgramId) VALUES " +
                        "(@MemberId, @ProgramId)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@MemberId", int.Parse(User.FindFirst("UserId")?.Value));  //change to memberId!!!!!!!!
                        command.Parameters.AddWithValue("@ProgramId", programId); 
                        
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error: " + ex.Message);
            }


        // Show a success message 
        TempData["RegisterMessage"] = $"{User.Identity.Name} successfully registered for {className}!";
        TempData["MessageType"] = "success";
        
        // Redirect to the same page to show the message
        return RedirectToPage();
    }

    public void OnGet()
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    string sql = "SELECT p.*, (p.capacity - COALESCE(m.registered_count, 0)) AS `spotsLeft`" +
                        "FROM ymca.Programs p LEFT JOIN ( " +
                            "SELECT ProgramId, COUNT(MemberId) AS registered_count FROM Member_Programs GROUP BY ProgramId" +
                        ") m ON p.program_id = m.ProgramId;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()){
                                Models.Programs classInfo = new Models.Programs();

                                classInfo.ProgramId = reader.GetInt32(0);
                                classInfo.ClassName = reader.GetString(1);
                                classInfo.ClassDescription = reader.GetString(2);
                                classInfo.StaffId = reader.GetInt16(3);
                                classInfo.PriceMember = reader.GetDouble(4);
                                classInfo.PriceNonmember = reader.GetDouble(5);
                                classInfo.Capacity = reader.GetInt32(6);
                                classInfo.StartDate = reader.GetDateTime(7);
                                classInfo.EndDate = reader.GetDateTime(8);
                                classInfo.StartTime = reader.GetDateTime(9);
                                classInfo.EndTime = reader.GetDateTime(10);
                                classInfo.Location = reader.GetString(11);
                                classInfo.Days = reader.GetString(12);


                                classInfo.SpotsLeft = reader.GetInt32(13);

                                programList.Add(classInfo);
                            }
                        }
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error: " + ex.Message);
            }
    }
}