using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
namespace YMCAProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string username {get; set;} 
    public string usertype {get; set;}  
    public string userId {get; set;}

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    /*
    Author: Cole Hansen
    Date: 10/9/24
    Parameters: 
    Function: on page load, get username, type, and id. Used for verifying login works
    Return: void
    */
    public void OnGet()
    {
        // if(User.Identity.IsAuthenticated){
        //     username = User.Identity.Name;
        //     usertype = User.FindFirst("UserType")?.Value;
        //     userId = User.FindFirst("UserId")?.Value;
        // }else{
        //     username = "failed";
        //     usertype = "failed";
        //     userId = "No id found";
        // }
    }
}
