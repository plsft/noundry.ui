using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Noundry.UI.Demo.Pages;

public class ComponentsModel : PageModel
{
    private readonly ILogger<ComponentsModel> _logger;

    public ComponentsModel(ILogger<ComponentsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Components page accessed");
    }
}