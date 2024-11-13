using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;


namespace YMCAProject.Pages.Admin
{
    public class ModifyProgram : PageModel
    {
        public int ProgramId {get; set;}

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

        /*
         * Data Validation
        */
        // Start Date
        public static ValidationResult ValidateStartDate(DateTime startDate, ValidationContext context)
        {
            if (startDate <= DateTime.Today)
            {   
                Console.WriteLine($"Start Date: {startDate}");
                return new ValidationResult("Start date must be after today");
            }

            return ValidationResult.Success;
        }
        // End Date
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
        // End Time
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

        public void OnGet(int programId)
        {
            ProgramId = programId;

        }
    }
}