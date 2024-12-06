using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages;

public class UserManualModel : PageModel
{
    private readonly ILogger<UserManualModel> _logger;

    public UserManualModel(ILogger<UserManualModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}