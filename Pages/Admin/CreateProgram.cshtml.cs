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

        [BindProperty, Required(ErrorMessage = "The Program Name is required")]
        public string ClassName { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "The Program Description is required")]
        public string? ClassDescription { get; set; }

        [BindProperty, Required(ErrorMessage = "The Location is required")]
        public string Location { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "The Price for Members is required"), Range(1, int.MaxValue, ErrorMessage = "The price must be greater than 0")]
        public double PriceMember { get; set; }

        [BindProperty, Required(ErrorMessage = "The Price for Non-members is required"), Range(1, int.MaxValue, ErrorMessage = "The price must be greater than 0")]
        public double PriceNonmember { get; set; }

        [BindProperty, Required(ErrorMessage = "Capacity is required"), Range(1, int.MaxValue, ErrorMessage = "The capacity must be greater than 0")]
        public int Capacity { get; set; }

        // [BindProperty, Required(ErrorMessage = "Staff ID is required")]
        // public int StaffId { get; set; } = 1;

        [BindProperty, Required(ErrorMessage = "The Day of Week is required")]
        public List<string> Days { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "Start Date is required"), CustomValidation(typeof(CreateProgram), nameof(ValidateStartDate))]
        public DateTime StartDate { get; set; }

        [BindProperty, Required(ErrorMessage = "End Date is required"), CustomValidation(typeof(CreateProgram), nameof(ValidateEndDate))]
        public DateTime EndDate { get; set; }

        [BindProperty, Required(ErrorMessage = "Start Time is required")]
        public DateTime StartTime { get; set; }

        [BindProperty, Required(ErrorMessage = "End Time is required"), CustomValidation(typeof(CreateProgram), nameof(ValidateEndTime))]
        public DateTime EndTime { get; set; }

        public List<Models.Staff> StaffList { get; set; } = new List<Models.Staff>();


        /*
        Author: Kylie Trousil
        Date: 11/13/24
        Parameters: start date, validation context
        Function: Data validation - ensure start date of program is after today
        returns: ValidationResult - success or error
        */
        public static ValidationResult ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            if (startDate <= DateTime.Today)
            {   
                Console.WriteLine($"Start Date: {startDate}");
                return new ValidationResult("Start date must be after today");
            }

            return ValidationResult.Success;
        }

        /*
        Author: Kylie Trousil
        Date: 11/13/24
        Parameters: end date, validation context
        Function: Data validation - ensure end date of program is after the start date
        returns: ValidationResult - success or error
        */
        public static ValidationResult ValidateEndDate(DateTime endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as CreateProgram;
            if (instance != null && instance.StartDate >= endDate)
            {
                Console.WriteLine($"Start Date: {instance.StartDate}");
                Console.WriteLine($"End Date: {endDate}");
                return new ValidationResult("End date must be after start date");
            }

            return ValidationResult.Success;
        }

        /*
        Author: Kylie Trousil
        Date: 11/13/24
        Parameters: end time, validation context
        Function: Data validation - ensure end time is after start time
        returns: ValidationResult - success or error
        */
        public static ValidationResult ValidateEndTime(DateTime endTime, ValidationContext context)
        {
            var instance = context.ObjectInstance as CreateProgram;

            if (instance != null && instance.StartTime >= endTime)
            {
                Console.WriteLine($"Start Time: {instance.StartTime}");
                Console.WriteLine($"End Time: {endTime}");
                return new ValidationResult("End time must be after start time");
            }

            return ValidationResult.Success;
        }

        public void OnGet()
        {        }

        /*
        Author: Kylie Trousil
        Date: 10/9/24
        Parameters: none
        Function: on submit button click, add new program to database
        returns: void
        */
        public void OnPost()
        {
            // data validation - ensure day of week is selected
            if (Days.Count < 1 || Days[0] is null)
            {
                ModelState.AddModelError("Days", "The Day of Week is required");
            }
            // data validation - check model state has no errors
            if (!ModelState.IsValid){
                Console.WriteLine("Error: Model State is not valid");
                return;
            }

            string daysAsString = string.Join(",", Days);

            // create new program
            try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();

                    string sql = "Insert INTO Programs " +
                        "(class_name, class_description, staff_id, price_member, price_nonmember, capacity, start_date, end_date, start_time, end_time, location, days, status) VALUES " +
                        "(@ClassName, @ClassDescription, @StaffId, @PriceMember, @PriceNonmember, @Capacity, @StartDate, @EndDate, @StartTime, @EndTime, @Location, @Days, @Status)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@ClassName", ClassName);
                        command.Parameters.AddWithValue("@ClassDescription", ClassDescription); 
                        command.Parameters.AddWithValue("@StaffId", 1);
                        command.Parameters.AddWithValue("@PriceMember", PriceMember); 
                        command.Parameters.AddWithValue("@PriceNonmember", PriceNonmember); 
                        command.Parameters.AddWithValue("@Capacity", Capacity); 
                        command.Parameters.AddWithValue("@StartDate", StartDate); 
                        command.Parameters.AddWithValue("@EndDate", EndDate); 
                        command.Parameters.AddWithValue("@StartTime", StartTime); 
                        command.Parameters.AddWithValue("@EndTime", EndTime);
                        command.Parameters.AddWithValue("@Location", Location); 
                        command.Parameters.AddWithValue("@Days", daysAsString);
                        command.Parameters.AddWithValue("@Status", 1);
                        
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error: " + ex.Message);
                return;
            }

            // redirect to programs page
            Response.Redirect("/Programs");
        }
    }
}