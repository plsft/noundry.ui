using Microsoft.AspNetCore.Mvc;
using Noundry.UI.Demo.Models;
using System.Diagnostics;

namespace Noundry.UI.Demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new DemoViewModel
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            EnableNotifications = true,
            BirthDate = DateTime.Now.AddYears(-25),
            FavoriteColor = "blue",
            SelectedOptions = new List<string> { "option1", "option3" }
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(DemoViewModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["Success"] = "Form submitted successfully!";
            return RedirectToAction("Index");
        }

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}