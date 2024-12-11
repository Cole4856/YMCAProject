using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using YMCAProject.Models;

namespace YMCAProject.Pages;

public class ProgramsModel : PageModel
{
    // sql connection
    private readonly IConfiguration _configuration;
    public ProgramsModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // program page filters
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

    // check is staff member
    public bool isStaff = false;
    // list of family members
    public List<Member> FamilyMembers { get; set; } = new();
    // list of programs to display
    public List<Programs> programList {get; set;} = new List<Programs>();


    /*
    Author: Kylie Trousil
    Date: 10/9/24
    Parameters: member id, class name, program id, start date, start time, days of week
    Function: On register button click, verify no schedule conflicts and register user for program
    returns: IActionResult - redirect to same page and display success or error message
    */
    public IActionResult OnPostRegisterClass(int memberId, string className, int programId, DateTime startDate, DateTime startTime, string days)
    {
        // select member who is registering for course
        LoadFamilyMembers();
        Member mem = FamilyMembers.FirstOrDefault(m => m.MemberId == memberId);

        try{
            // connection to databse
            string connectionString = _configuration.GetConnectionString("Default");

            using (MySqlConnection connection = new MySqlConnection(connectionString)){
                connection.Open();

                // check if member is already registered for the course
                string sql = "SELECT Count(*) FROM Member_Programs " +
                    "WHERE MemberId = @MemberId AND ProgramID = @ProgramId ";

                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@MemberId", mem.MemberId);
                    command.Parameters.AddWithValue("@ProgramId", programId); 

                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        reader.Read();
                        int count = reader.GetInt32(0);

                        if (count > 0){
                            // Show a failure message 
                            TempData["RegisterMessage"] = $"Error: {mem.FirstName} {mem.LastName} is already registered for {className}";
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
                            "AND (TIME(@startTime) BETWEEN TIME(p.start_time) AND TIME(p.end_time))";

                string[] dayArray = days.Split(',');

                using (MySqlCommand command = new MySqlCommand(sql, connection)) {
                    command.Parameters.AddWithValue("@MemberId", mem.MemberId);
                    command.Parameters.AddWithValue("@startDate", startDate);         
                    command.Parameters.AddWithValue("@startTime", startTime); 


                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        while(reader.Read()){
                            string curClass = reader.GetString(1);
                            string programDays = reader.GetString(2);

                            foreach (var day in dayArray)
                            {
                                int index = programDays.IndexOf(day);
                                if (index != -1){
                                    // Show a failure message 
                                    TempData["RegisterMessage"] = $"Error: unable to register for {className} because it overlaps with {curClass}";
                                    TempData["MessageType"] = "error";
                                        
                                    // Redirect to the same page to show the message
                                    return RedirectToPage();
                                }

                            }
                        }
                    }
                }

                // register member for program
                sql = "Insert INTO Member_Programs " +
                    "(MemberId, ProgramId) VALUES " +
                    "(@MemberId, @ProgramId)";

                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@MemberId", mem.MemberId); 
                    command.Parameters.AddWithValue("@ProgramId", programId); 
                    
                    command.ExecuteNonQuery();
                }
            }
        }
        
        catch(Exception ex){
            Console.WriteLine("We have a sql error in register class: " + ex.Message);
        }

        // Show a success message 
        TempData["RegisterMessage"] = $"{mem.FirstName} {mem.LastName} is successfully registered for {className}!";
        TempData["MessageType"] = "success";
        
        // Redirect to the same page to show the message
        return RedirectToPage();
    }

    /*
    Author: Kylie Trousil
    Date: 11/13/24
    Parameters: program id, class name
    Function: On cancel button click, soft delete class by updating class status
    returns: IActionResult - redirect to same page and display success or error message
    */
    public IActionResult OnPostCancelClass(int programId, string className)
    {
        try{
            // connection to sql database
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

            // Show a success message 
            TempData["RegisterMessage"] = $"Success: {className} has been canceled and all members will be notified on their dashboard";
            TempData["MessageType"] = "success";
        }
        catch(Exception ex){
            Console.WriteLine("We have an error: " + ex.Message);
            // Show an error message 
            TempData["RegisterMessage"] = $"Error: An exception has occured when trying to cancel {className}";
            TempData["MessageType"] = "error";
        }
        
        // Redirect to the same page and show the message
        return RedirectToPage();
    }

    /*
    Author: Kylie Trousil
    Date: 10/9/24 (last updated 12/9/24)
    Parameters: 
    Function: On page load, load all programs and family members
    returns: void
    */
    public void OnGet()
    {
        // check if user on staff
        if ((User.FindFirst("UserType")?.Value?.Equals("Admin") ?? false) || (User.FindFirst("UserType")?.Value?.Equals("Staff") ?? false)){
            isStaff = true;
        }
        // if member or non-member, load family members
        if ((User.FindFirst("UserType")?.Value?.Equals("Member") ?? false) || (User.FindFirst("UserType")?.Value?.Equals("non-member") ?? false)){
            LoadFamilyMembers();
        }

        try{
            // sql database connection
            string connectionString = _configuration.GetConnectionString("Default");

            using (MySqlConnection connection = new MySqlConnection(connectionString)){
                connection.Open();
                // Get Programs list
                string sql = "SELECT p.*, (p.capacity - COALESCE(m.registered_count, 0)) AS `spotsLeft`" +
                    "FROM ymca.Programs p LEFT JOIN ( " +
                        "SELECT ProgramId, COUNT(MemberId) AS registered_count FROM Member_Programs GROUP BY ProgramId" +
                    ") m ON p.program_id = m.ProgramId WHERE status = @Status ORDER BY p.class_name;";

                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@Status", Status);

                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()){
                            Programs classInfo = new Programs();

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

                            // check filter values; assume class will be displayed until told otherwise
                            bool addClass = true;
                            // keyword value
                            if ((SearchName != null) && classInfo.ClassName.IndexOf(SearchName, StringComparison.OrdinalIgnoreCase) == -1){
                                addClass = false;
                            }
                            // day of the week
                            else if (DayOfWeek.Count >= 1 && DayOfWeek[0] is not null){
                                foreach (var day in DayOfWeek){
                                    int index = classInfo.Days.IndexOf(day);
                                    if (index == -1){
                                        addClass = false;
                                    }
                                }
                            }
                            // start date in range
                            else if (StartDateFrom.HasValue && classInfo.StartDate < StartDateFrom){
                                addClass = false;
                            }
                            else if (StartDateTo.HasValue && classInfo.StartDate > StartDateTo){
                                addClass = false;
                            }

                            // if pass all filters, add class
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

    /*
    Author: Kylie Trousil
    Date: 12/6/24
    Parameters: 
    Function: load all family members of logged in user
    returns: void
    */
    private void LoadFamilyMembers(){
        try{
            // sql database connection
            string connectionString = _configuration.GetConnectionString("Default");

            using (MySqlConnection connection = new MySqlConnection(connectionString)){
                connection.Open();
                // Get Family Members
                string sql = "SELECT * FROM Members " +
                    "WHERE FamilyID = (SELECT FamilyID FROM Members WHERE MemberID = @id);";

                using (MySqlCommand command = new MySqlCommand(sql, connection)){
                    command.Parameters.AddWithValue("@id", int.Parse(User.FindFirst("UserId")?.Value));

                    using (MySqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()){
                            Member newMem = new Member();

                            newMem.MemberId = reader.GetInt32(0);
                            newMem.FirstName = reader.GetString(1);
                            newMem.LastName = reader.GetString(2);
                            newMem.Email = reader.GetString(3);
                            newMem.PasswordHash = reader.GetString(4);
                            newMem.IsActive = reader.GetBoolean(5);
                            newMem.IsMember = reader.GetBoolean(6);
                            newMem.FamilyId = reader.GetInt32(7);

                            if (newMem.IsActive){
                                FamilyMembers.Add(newMem);
                            }
                        }
                    }
                }
            }
        }
        catch(Exception ex){
            Console.WriteLine("We have an error when loading family members: " + ex.Message);
        }
    }
}