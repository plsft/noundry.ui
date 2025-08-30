using System.ComponentModel.DataAnnotations;

namespace Noundry.UI.Demo.Models;

public class DemoViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Enable Notifications")]
    public bool EnableNotifications { get; set; }

    [Display(Name = "Birth Date")]
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Favorite Color")]
    public string? FavoriteColor { get; set; }

    [Display(Name = "Selected Options")]
    public List<string> SelectedOptions { get; set; } = new();

    [Display(Name = "Bio")]
    [DataType(DataType.MultilineText)]
    public string? Bio { get; set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}