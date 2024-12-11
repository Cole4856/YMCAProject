using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace YMCAProject.Pages
{
    public class CreateMember : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<string> _passwordHasher;
        public CreateMember(IConfiguration configuration)
        {
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<string>();
        }

        [BindProperty, Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "An Email is required")]
        public string Email { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "A Password is required")]
        public string Password { get; set; } = null!;

        [BindProperty, Required(ErrorMessage = "A membership option is required")]
        public int IsMember {get; set; } = 1;


        public void OnGet()
        {
        }

        /*
        Author: Kylie Trousil
        Date: 11/13/24
        Parameters: none
        Function: on submit button click, create new member
        returns: void
        */
        public void OnPost()
        {
            // data validation - check model state is valid
            if (!ModelState.IsValid){
                Console.WriteLine("Error: Model State is not valid");
                return;
            }

            //create new member
            try{
                string connectionString = _configuration.GetConnectionString("Default");

                using (MySqlConnection connection = new MySqlConnection(connectionString)){
                    connection.Open();
                    // check if email is already being used
                    string sql = "SELECT COUNT(*) FROM ymca.Members WHERE Email = @Email;";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@Email", Email);

                        using (MySqlDataReader reader = command.ExecuteReader()) {
                            reader.Read();
                            int memberExists = reader.GetInt16(0);

                            if (memberExists == 1){
                                ModelState.AddModelError("Email", "This email already has an account. Please login or choose another email.");
                                return;
                            }
                        }
                    }

                    // hash password
                    string hashedPassword = _passwordHasher.HashPassword(null, Password);

                    // add new member to database
                    sql = "Insert INTO Members " +
                        "(FirstName, LastName, Email, PasswordHash, IsActive, IsMember) VALUES " +
                        "(@FirstName, @LastName, @Email, @PasswordHash, @IsActive, @IsMember)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection)){
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName); 
                        command.Parameters.AddWithValue("@Email", Email); 
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword); 
                        command.Parameters.AddWithValue("@IsActive", 1); 
                        command.Parameters.AddWithValue("@IsMember", IsMember); 
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex){
                Console.WriteLine("We have an error when creating new member: " + ex.Message);
                return;
            }

            // redirect to membership page
            Response.Redirect("/Membership");
        }
    }
}