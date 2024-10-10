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

    public void OnGet()
    {
        try{
                string connectionString = _configuration.GetConnectionString("Default");
                // "server=127.0.0.1;uid=ymca_proj;pwd=ymca@1234;database=ymca;";

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

                                classInfo.SpotsLeft = reader.GetInt32(11);

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