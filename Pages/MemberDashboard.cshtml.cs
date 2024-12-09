using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace YMCAProject.Pages;

public class MemberDashboard : PageModel
{
    // private readonly ILogger<MemberDashboard> _logger;

    // public MemberDashboard(ILogger<MemberDashboard> logger)
    // {
    //     _logger = logger;
    // }

    private readonly IConfiguration _configuration;
    public MemberDashboard(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Models.Programs> programList {get; set;} = [];
    public int IsMember { get; set; } //Property to store the isMember value
    public String FirstName {get; set;} //Store first name
    public int MemberId { get; set; }

     /* 
     * Cancel Class Button
     *
     * Input: programID, userId
     * Return:
     */
    public IActionResult OnPostCancelRegistration(int userId, int programId)
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    // change status to 0
                    string sql = "DELETE FROM Member_Programs WHERE MemberId = @UserId AND ProgramId = @ProgramId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@ProgramId", programId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["RegisterMessage"] = "Success: Your registration has been canceled.";
                            TempData["MessageType"] = "success";
                        }
                        else
                        {
                            TempData["RegisterMessage"] = "Error: No registration found to cancel.";
                            TempData["MessageType"] = "error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {       
                Console.WriteLine("Error: " + ex.Message);
                TempData["RegisterMessage"] = "Error: An exception occurred while trying to cancel your registration.";
                TempData["MessageType"] = "error";
            }

            return RedirectToPage();
    }

    //display users programs that they are signed up for
    public void OnGet(string? id = null)
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){

                    connection.Open();

                    //get the currently logged in users ID
                    int memberId = int.Parse(User.FindFirst("UserId")?.Value);
                    MemberId = memberId;

                    //Query to get isMember status
                    string memberQuery = "SELECT isMember FROM ymca.Members WHERE MemberId = @UserId";
                    using (MySqlCommand memberCommand = new MySqlCommand(memberQuery, connection))
                    {
                        memberCommand.Parameters.AddWithValue("@UserId", memberId);
                        IsMember = Convert.ToInt32(memberCommand.ExecuteScalar());
                    }

                    //Query to get first name
                    string nameQuery = "SELECT FirstName FROM ymca.Members WHERE MemberId = @UserId";
                    using (MySqlCommand memberCommand = new MySqlCommand(nameQuery, connection))
                    {
                        memberCommand.Parameters.AddWithValue("@UserId", memberId);
                        FirstName = memberCommand.ExecuteScalar()?.ToString();
                    }

                    //Query to get program details
                    string sql = @"SELECT p.*, 
                       (p.capacity - COALESCE(m.registered_count, 0)) AS `spotsLeft`
                FROM ymca.Programs p
                INNER JOIN Member_Programs mp ON p.program_id = mp.ProgramId
                LEFT JOIN (
                    SELECT ProgramId, COUNT(MemberId) AS registered_count 
                    FROM Member_Programs 
                    GROUP BY ProgramId
                ) m ON p.program_id = m.ProgramId
                WHERE mp.MemberId = @MemberId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        //add users MemberId as a parameter to the query
                        if(string.IsNullOrEmpty(id)){
                            command.Parameters.AddWithValue("@MemberId", memberId);
                        }else 
                        {
                            command.Parameters.AddWithValue("@MemberId", int.Parse(id));
                        }

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()){

                                Models.Programs classInfo = new Models.Programs();

                                classInfo.ProgramId = reader.GetInt32(0);
                                classInfo.ClassName = reader.GetString(1);
                                classInfo.ClassDescription = reader.GetString(2);
                                classInfo.StaffId = reader.GetInt16(3);
                                classInfo.Capacity = reader.GetInt32(6);
                                classInfo.StartDate = reader.GetDateTime(7);
                                classInfo.EndDate = reader.GetDateTime(8);
                                classInfo.StartTime = reader.GetDateTime(9);
                                classInfo.EndTime = reader.GetDateTime(10);
                                classInfo.Location = reader.GetString(11);
                                classInfo.Days = reader.GetString(12);
                                classInfo.SpotsLeft = reader.GetInt32(13);
                                classInfo.Status = reader.GetInt32(13);

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