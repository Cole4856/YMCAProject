using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages
{
    public class LogoutModelModel : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
           // Determine the user type from claims
           var userType = User.FindFirst("UserType")?.Value;

           if (userType == "Member")
           {
               await HttpContext.SignOutAsync("MemberCookie");
           }
           else if (userType == "Staff")
           {
               await HttpContext.SignOutAsync("StaffCookie");
           } 

           // Redirect to the home page or login page after logout
           return RedirectToPage("/Index");
        }
    }
}
