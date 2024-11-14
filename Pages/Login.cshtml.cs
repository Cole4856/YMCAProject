using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace YMCAProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public LoginModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

       [BindProperty, Required(ErrorMessage = "Email is required")]
        public string Email {get; set;} = null!;
         [BindProperty, Required(ErrorMessage = "Password is required")]
        public string Password {get; set;} = null!;
        


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }

            

            // Retrieve user from the database
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.Email == Email);

            var staff = await _dbContext.Staff
                .FirstOrDefaultAsync(s => s.Email == Email);

            //Validate user credentials
            if (member != null && VerifyPassword(Password, member.PasswordHash))
            {
                if(member.IsMember){
                    SignInUser(member.Email, "Member", member.MemberId.ToString());
                    return RedirectToPage("/Index"); // Redirect after successful login
                }else{
                    SignInUser(member.Email, "non-member", member.MemberId.ToString());
                    return RedirectToPage("/Index");
                }
            }
            else 
            if (staff != null && VerifyPassword(Password, staff.PasswordHash))
            {
                if(!staff.is_admin){
                    SignInUser(staff.Email, "Staff", staff.StaffId.ToString());
                    return RedirectToPage("/Index"); // Redirect after successful login
                }else{
                    SignInUser(staff.Email, "Admin", staff.StaffId.ToString());
                    return RedirectToPage("Index");
                }
            
            }

            ModelState.AddModelError("Password", "Invalid login attempt.");
            return Page();
        }



        private async void SignInUser(string username, string userType, string userId)
        {
           var claims = new List<Claim>
           {
              new Claim(ClaimTypes.Name, username),
              new Claim("UserType", userType),
              new Claim("UserId", userId)
           };

           var identity = new ClaimsIdentity(claims, "MyCookieAuth");
           var principal = new ClaimsPrincipal(identity);

          // Sign in the user with cookie authentication
           await HttpContext.SignInAsync("MyCookieAuth", principal);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            //here to later add hashing as I'm sure the project will require at some point
            if(password.Equals(passwordHash)){
                return true;
            }
            return false;
        }



        public void OnGet()
        {
        }
    }
}
