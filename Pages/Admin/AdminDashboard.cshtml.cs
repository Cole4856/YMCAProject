using System.Dynamic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
    public List<Models.MemberProgram> memberPrograms {get; set;} = [];

    public IActionResult OnPostGenerateReport(DateTime start, DateTime end){

        //set end to 30th, 31st, 29th, or 28th of month
        DateTime endDate = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month));
  
        try
        {
            string connectionString = _configuration.GetConnectionString("Default");

            using (MySqlConnection connection = new MySqlConnection(connectionString)){
                connection.Open();

                string sql = "SELECT m.MemberId, " +
                             "CONCAT(m.FirstName, ' ', m.LastNAme) AS member_name, " +
                             "p.program_id, p.class_name, p.start_date, p.end_date " +
                             "FROM Members m " + 
                             "JOIN Member_Programs mp ON m.MemberId = mp.MemberId " +
                             "JOIN Programs p ON mp.ProgramId = p.program_id " +
                             "WHERE (p.start_date <= @End) " +
                             "AND (p.end_date >= @Start) "+
                             "ORDER BY m.MemberId, p.start_date;";
                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@Start", start);
                    command.Parameters.AddWithValue("@End", endDate); 

                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        while(reader.Read()){
                            var mp = new Models.MemberProgram();

                            mp.MemberId = reader.GetInt32(0);
                            mp.MemberName = reader.GetString(1);
                            mp.ProgramId = reader.GetInt32(2);
                            mp.ClassName = reader.GetString(3);
                            mp.StartDate = reader.GetDateTime(4);
                            mp.EndDate = reader.GetDateTime(5);

                            memberPrograms.Add(mp);

                        }
                    }
                }


            }

            Console.WriteLine(memberPrograms.Count);
            var csvBuilder = new StringBuilder();

            // Write the CSV headers
             csvBuilder.AppendLine("MemberId,Name,ProgramId,ClassName,StartDate,EndDate");

            // Write the CSV rows
            foreach (var item in memberPrograms)
            {
             csvBuilder.AppendLine($"{item.MemberId}," +
                                  $"{item.MemberName}," +
                                  $"{item.ProgramId}," +
                                  $"{item.ClassName}," +
                                  $"{item.StartDate:yyyy-MM-dd}," +
                                  $"{item.EndDate:yyyy-MM-dd}");
            }

            // Convert the StringBuilder content to a byte array
            byte[] csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            // Return the CSV file as a downloadable response


            return File(csvBytes, "text/csv", "MemberPrograms.csv");

        }
        
        catch(Exception ex)
        {
            Console.WriteLine("We have an error generating report: " + ex.Message);
        }
          
        // Show a success message 
        TempData["RegisterMessage"] = $"Report Generated, please check your downloads";
        TempData["MessageType"] = "success";
        
        // Redirect to the same page to show the message
        return RedirectToPage();
    }

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

                    // remove from current or future programs
                    sql = "DELETE FROM Member_Programs " +
                            "WHERE MemberID = @MemberId AND ProgramID IN (SELECT program_id " +
                                                                            "FROM Programs " +
                                                                            "WHERE NOW() < end_date)";
                    
                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@MemberId", memberId);
                        
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error on delete user: " + ex.Message);
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