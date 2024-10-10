using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
namespace YMCAProject.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string username {get; set;} 
    public string usertype {get; set;}  

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        if(User.Identity.IsAuthenticated){
            username = User.Identity.Name;
            usertype = User.FindFirst("UserType")?.Value;
        }else{
            username = "failed";
            usertype = "failed";
        }

    }
}
