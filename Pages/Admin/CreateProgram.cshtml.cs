using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Esf;
using Microsoft.Extensions.Configuration; // For configuration
using MySql.Data.MySqlClient;

namespace YMCAProject.Pages.Admin
{
    
    public class CreateProgram : PageModel
    {
        private readonly IConfiguration _configuration;
        public CreateProgram(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // public string ErrorMessage { get; set; }

        [BindProperty, Required(ErrorMessage = "The Program Name is required")]
        public string ClassName { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "The Program Description is required")]
        public string? ClassDescription { get; set; }

        [BindProperty, Required(ErrorMessage = "The Price for Members is required")]
        public double PriceMember { get; set; }

        [BindProperty, Required(ErrorMessage = "The Price for Non-members is required")]
        public double PriceNonmember { get; set; }

        [BindProperty, Required(ErrorMessage = "Capacity is required")]
        public int Capacity { get; set; }

        [BindProperty, Required(ErrorMessage = "Staff ID is required")]
        public int StaffId { get; set; }

        [BindProperty, Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [BindProperty, Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        [BindProperty, Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime { get; set; }

        [BindProperty, Required(ErrorMessage = "End Time is required")]
        public DateTime EndTime { get; set; }

        public List<Models.Staff> StaffList { get; set; } = new List<Models.Staff>();

        public void OnGet()
        {
            // try{
            //     string connectionString = _configuration.GetConnectionString("Default");
            //     using (MySqlConnection connection = new MySqlConnection(connectionString)){
            //         connection.Open();

            //         string sql = "SELECT * FROM staff WHERE is_active";

            //         using (MySqlCommand command = new MySqlCommand(sql, connection)){
            //             using (MySqlDataReader reader = command.ExecuteReader()) {
            //                 while (reader.Read()){
            //                     Models.Staff staffInfo = new Models.Staff();

            //                     staffInfo.StaffId = reader.GetInt32(0);
            //                     staffInfo.Fname = reader.GetString(1);
            //                     staffInfo.Lname = reader.GetString(2);

            //                     StaffList.Add(staffInfo);
            //                 }
            //             }
            //         }
            //     }

            // }
            // catch(Exception ex){
            //     Console.WriteLine("We have an error: " + ex.Message);
            // }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid){
                return;
            }

            //create new program
            try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    string sql = "Insert INTO programs " +
                        "(class_name, class_description, staff_id, price_member, price_nonmember, capacity, start_date, end_date, start_time, end_time) VALUES " +
                        "(@ClassName, @ClassDescription, @StaffId, @PriceMember, @PriceNonmember, @Capacity, @StartDate, @EndDate, @StartTime, @EndTime)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@ClassName", ClassName);
                        command.Parameters.AddWithValue("@ClassDescription", ClassDescription); 
                        command.Parameters.AddWithValue("@StaffId", StaffId);
                        command.Parameters.AddWithValue("@PriceMember", PriceMember); 
                        command.Parameters.AddWithValue("@PriceNonmember", PriceNonmember); 
                        command.Parameters.AddWithValue("@Capacity", Capacity); 
                        command.Parameters.AddWithValue("@StartDate", StartDate); 
                        command.Parameters.AddWithValue("@EndDate", EndDate); 
                        command.Parameters.AddWithValue("@StartTime", StartTime); 
                        command.Parameters.AddWithValue("@EndTime", EndTime);

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                // ErrorMessage = ex.Message;
                Console.WriteLine("We have an error: " + ex.Message);
                return;
            }

            Response.Redirect("/Programs");
        }
    }
}