using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Noundry.UI.Demo.Pages;

public class FormsModel : PageModel
{
    private readonly ILogger<FormsModel> _logger;

    public FormsModel(ILogger<FormsModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public ContactFormViewModel ContactForm { get; set; } = new();

    public bool IsProcessing { get; set; } = false;

    public void OnGet()
    {
        // Initialize with sample data
        ContactForm = new ContactFormViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PreferredContactDate = DateTime.Now.AddDays(7),
            SubscribeToNewsletter = true,
            PreferredContactMethod = "email"
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Simulate processing
        IsProcessing = true;
        await Task.Delay(1000);

        _logger.LogInformation("Form submitted: {@ContactForm}", ContactForm);

        TempData["Success"] = "Thank you! Your form has been submitted successfully.";
        return RedirectToPage();
    }
}

public class ContactFormViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [Display(Name = "Preferred Contact Date")]
    public DateTime? PreferredContactDate { get; set; }

    [Display(Name = "Project Start Date")]
    public DateTime? ProjectStartDate { get; set; }

    [Display(Name = "Project End Date")]
    public DateTime? ProjectEndDate { get; set; }

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Display(Name = "Services of Interest")]
    public List<string> InterestedServices { get; set; } = new();

    [Display(Name = "Team Members")]
    public List<string> TeamMembers { get; set; } = new();

    [Required]
    [Display(Name = "Message")]
    [StringLength(500, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "Subscribe to Newsletter")]
    public bool SubscribeToNewsletter { get; set; }

    [Required]
    [Display(Name = "I agree to the terms and conditions")]
    public bool AgreeToTerms { get; set; }

    [Display(Name = "Preferred Contact Method")]
    public string PreferredContactMethod { get; set; } = "email";

    [Display(Name = "Service Rating")]
    [Range(1, 5)]
    public int ServiceRating { get; set; } = 0;
}