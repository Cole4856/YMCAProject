using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages
{
    public class LogoutModel : PageModel
    {

        //Author: Cole Hansen
        //Date: 10/9/24
        //params: none
        //function: sign out the cookie
        //return: redirect to home page
        public async Task<IActionResult>OnGet()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Clear the cookie
            return RedirectToPage("Index");
        }

        //Author: Cole Hansen
        //Date: 10/9/24
        //params: none
        //function: sign out the cookie
        //return: redirect to home page
        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync("MyCookieAuth"); // Clear the cookie
            return RedirectToPage("Index");
        }


    }
}
