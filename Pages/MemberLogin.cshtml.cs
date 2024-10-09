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
    public class MemberLoginModel : PageModel
    {
        
       private readonly MemberStore _memberStore;
       private readonly IPasswordHasher<Member> _passwordHasher;

       public MemberLoginModel(MemberStore memberStore, IPasswordHasher<Member> passwordHasher)
       {
           _memberStore = memberStore;
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
              var member = await _memberStore.FindByNameAsync(this.Email.ToLower(), HttpContext.RequestAborted);
              if (member != null && VerifyPassword(this.Password, member.PasswordHash))
              {
                // Create user claims and sign in
                await SignInAsync(member);
                return RedirectToPage("/Index"); // Redirect to home page after login
              }
              ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
           return Page();
        }
        private async Task SignInAsync(Member member)
        {
           var claims = new[]
           {
              new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
              new Claim(ClaimTypes.Email, member.Email),
              new Claim("UserType", "Member") // Custom claim
           };
           var claimsIdentity = new ClaimsIdentity(claims, "Member");

           await HttpContext.SignInAsync("MemberCookie", new ClaimsPrincipal(claimsIdentity));
        }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // Verify the password using the password hasher
        var result = _passwordHasher.VerifyHashedPassword(null, passwordHash, password);
        return result == PasswordVerificationResult.Success;
    }


    }
}
