using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Noundry.UI.Demo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Index page accessed");
    }

    public IActionResult OnPost()
    {
        TempData["Success"] = "Form submitted successfully from the home page!";
        return RedirectToPage();
    }
}