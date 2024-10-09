using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YMCAProject.Models;
using YMCAProject.UserStore;

namespace YMCAProject.Pages
{
    public class StaffLoginModel : PageModel
    {
        
       private readonly StaffStore _staffStore;
       private readonly IPasswordHasher<Member> _passwordHasher;

       public StaffLoginModel(StaffStore staffStore, IPasswordHasher<Member> passwordHasher)
       {
           _staffStore = staffStore;
           _passwordHasher = passwordHasher;
       }
    
       [BindProperty, Required(ErrorMessage = "Email is required")]
       public string Email { get; set; } = null!;
       [BindProperty, Required(ErrorMessage = "Password is required")]
       public string Password { get; set; } = null!;



        public void OnGet(){}

        public async Task<IActionResult> OnPostAsync()
        {
           if (ModelState.IsValid)
           {
              var staff = await _staffStore.FindByNameAsync(this.Email.ToLower(), HttpContext.RequestAborted);
              if (staff != null && VerifyPassword(this.Password, staff.PasswordHash))
              {
                // Create user claims and sign in
                await SignInAsync(staff);
                return RedirectToPage("/Index"); // Redirect to home page after login
              }
              ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
           return Page();
        }
        private async Task SignInAsync(Staff staff)
        {
           var claims = new[]
           {
              new Claim(ClaimTypes.NameIdentifier, staff.staff_id.ToString()),
              new Claim(ClaimTypes.Email, staff.email),
              new Claim("UserType", "Staff") // Custom claim
           };
           var claimsIdentity = new ClaimsIdentity(claims, "Staff");

          await HttpContext.SignInAsync("StaffCookie", new ClaimsPrincipal(claimsIdentity));
        }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Verify the password using the password hasher
        var result = _passwordHasher.VerifyHashedPassword(null, passwordHash, password);
        return result == PasswordVerificationResult.Success;
    }


    }
}
