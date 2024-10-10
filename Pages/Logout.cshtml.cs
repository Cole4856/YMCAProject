using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult>OnGet()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Clear the cookie
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Clear the cookie
            return RedirectToPage("Index");
        }


    }
}
