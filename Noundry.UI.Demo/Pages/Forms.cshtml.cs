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

    [BindProperty]
    public List<UserInfo> Users { get; set; } = new();

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

        // Initialize sample users collection
        Users = new List<UserInfo>
        {
            new UserInfo { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Status = "Active", Role = "Administrator", JoinDate = DateTime.Now.AddMonths(-6) },
            new UserInfo { Id = 2, Name = "Bob Smith", Email = "bob@example.com", Status = "Inactive", Role = "User", JoinDate = DateTime.Now.AddMonths(-12) },
            new UserInfo { Id = 3, Name = "Carol Williams", Email = "carol@example.com", Status = "Active", Role = "Moderator", JoinDate = DateTime.Now.AddMonths(-3) },
            new UserInfo { Id = 4, Name = "David Brown", Email = "david@example.com", Status = "Active", Role = "User", JoinDate = DateTime.Now.AddMonths(-8) },
            new UserInfo { Id = 5, Name = "Emma Davis", Email = "emma@example.com", Status = "Pending", Role = "User", JoinDate = DateTime.Now.AddDays(-15) },
            new UserInfo { Id = 6, Name = "Frank Wilson", Email = "frank@example.com", Status = "Active", Role = "User", JoinDate = DateTime.Now.AddMonths(-2) }
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

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}