using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages;

public class MembershipModel : PageModel
{
    private readonly ILogger<MembershipModel> _logger;

    public MembershipModel(ILogger<MembershipModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}