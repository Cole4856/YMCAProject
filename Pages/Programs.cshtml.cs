using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace YMCAProject.Pages;

public class ProgramsModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string SearchName { get; set; }

    [BindProperty(SupportsGet = true)]
    public List<string> DayOfWeek { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDateFrom { get; set; } = DateTime.Today; // default to today

    [BindProperty(SupportsGet = true)]
    public DateTime? StartDateTo { get; set; } = DateTime.Today.AddYears(1); // default to year after today

    [BindProperty(SupportsGet = true)]
    public int Status { get; set; } = 1;

    private readonly IConfiguration _configuration;
    public ProgramsModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Models.Programs> programList {get; set;} = [];

    // Register button
    public IActionResult OnPostRegisterClass(string className, int programId, DateTime startDate, DateTime startTime, string days)
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    // check if already registered
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

                    // check for schedule conflicts
                    sql = "SELECT p.program_id, p.class_name, p.days, p.start_date, p.end_date, p.start_time, p.end_time " +
                            "FROM Programs p " +
                            "JOIN Member_Programs mp on p.program_id = mp.ProgramId " +
                            "WHERE mp.MemberId = @MemberId " +
                                "AND (@startDate BETWEEN p.start_date AND p.end_date) " +
                                "AND (@startTime BETWEEN TIME(p.start_time) AND TIME(p.end_time))";

                    string[] dayArray = days.Split(',');

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@MemberId", int.Parse(User.FindFirst("UserId")?.Value));
                        command.Parameters.AddWithValue("@startDate", startDate);         
                        command.Parameters.AddWithValue("@startTime", startTime); 

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            reader.Read();
                            string programDays = reader.GetString(2);

                            foreach (var day in dayArray)
                            {
                                int index = programDays.IndexOf(day);
                                if (index != -1){
                                    // Show a failure message 
                                    TempData["RegisterMessage"] = $"Error: unable to register for {className} because it overlaps with {reader.GetString(1)}";
                                    TempData["MessageType"] = "error";
                                    
                                    // Redirect to the same page to show the message
                                    return RedirectToPage();
                                }

                            }
                        }
                    }

                    // register
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

    // Cancel button
    public IActionResult OnPostCancelClass(int programId, string className)
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    // change status to 0
                    string sql = "UPDATE Programs SET status = 0 " +
                                "WHERE program_id = @ProgramId ";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@ProgramId", programId); 

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error: " + ex.Message);
            }


        // Show a success message 
        TempData["RegisterMessage"] = $"Success: {className} has been canceled and all members will be notified on their dashboard";
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
                    ") m ON p.program_id = m.ProgramId WHERE status = @Status ORDER BY p.class_name;";

                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@Status", Status);

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
                            classInfo.Status = reader.GetInt16(13);
                            classInfo.SpotsLeft = reader.GetInt32(14);

                            bool addClass = true;
                            if ((SearchName != null) && classInfo.ClassName.IndexOf(SearchName, StringComparison.OrdinalIgnoreCase) == -1){
                                addClass = false;
                            }
                            else if (DayOfWeek.Count >= 1 && DayOfWeek[0] is not null){
                                foreach (var day in DayOfWeek){
                                    int index = classInfo.Days.IndexOf(day);
                                    if (index == -1){
                                        addClass = false;
                                    }
                                }
                            }
                            else if (StartDateFrom.HasValue && classInfo.StartDate < StartDateFrom){
                                addClass = false;
                            }
                            else if (StartDateTo.HasValue && classInfo.StartDate > StartDateTo){
                                addClass = false;
                            }

                            if (addClass){
                                programList.Add(classInfo);
                            }
                            
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