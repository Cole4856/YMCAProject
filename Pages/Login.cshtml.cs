using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using YMCAProject.Models;

namespace YMCAProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PasswordHasher<string> _passwordHasher;

        public LoginModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<string>();
        }



       [BindProperty, Required(ErrorMessage = "Email is required")]
        public string Email {get; set;} = null!;
         [BindProperty, Required(ErrorMessage = "Password is required")]
        public string Password {get; set;} = null!;
        


        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Member member = null;
            Staff staff = null;
            if (action == "user"){
                member = await _dbContext.Members
                .FirstOrDefaultAsync(m => m.Email == Email);
            } else {
                staff = await _dbContext.Staff
                .FirstOrDefaultAsync(s => s.Email == Email);
            }
            // Retrieve user from the database
            // var member = await _dbContext.Members
            //     .FirstOrDefaultAsync(m => m.Email == Email);

            // var staff = await _dbContext.Staff
            //     .FirstOrDefaultAsync(s => s.Email == Email);

            //Validate user credentials
            if (member != null && VerifyPassword(Password, member.PasswordHash))
            {
                if(member.IsActive){
                    if(member.IsMember){
                        SignInUser(member.Email, "Member", member.MemberId.ToString());
                        return RedirectToPage("/Index"); // Redirect after successful login
                    }else{
                        SignInUser(member.Email, "non-member", member.MemberId.ToString());
                        return RedirectToPage("/Index");
                    }
                }
            }
            else 
            if (staff != null && VerifyPassword(Password, staff.PasswordHash))
            {
                if(staff.is_active){
                    if(!staff.is_admin){
                        SignInUser(staff.Email, "Staff", staff.StaffId.ToString());
                        return RedirectToPage("/Index"); // Redirect after successful login
                    }else{
                        SignInUser(staff.Email, "Admin", staff.StaffId.ToString());
                        return RedirectToPage("Index");
                    }
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

        private bool VerifyPassword(string providedPassword, string storedHash)
        {
            try{
                var result = _passwordHasher.VerifyHashedPassword(null, storedHash, providedPassword);
                if(result == PasswordVerificationResult.Success){
                    return true;
                }
            }catch(Exception ex){
                Console.WriteLine("Hashing Error: " + ex.Message);
            }
            
            return false;
        }



        public void OnGet()
        {
        }
    }
}
