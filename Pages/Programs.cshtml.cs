using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YMCAProject.Pages;

public class ProgramsModel : PageModel
{
    private readonly ILogger<ProgramsModel> _logger;

    public ProgramsModel(ILogger<ProgramsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}