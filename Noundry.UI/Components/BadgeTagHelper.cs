using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-badge")]
public class BadgeTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Badge variant (default, success, warning, error, info)
    /// </summary>
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Badge size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Whether to show as outline style
    /// </summary>
    public bool Outline { get; set; } = false;

    /// <summary>
    /// Whether to show as rounded pill style
    /// </summary>
    public bool Pill { get; set; } = false;

    /// <summary>
    /// Icon to display (optional)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "span";

        var classes = GetBadgeClasses();
        AddCssClasses(output, classes);

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var badgeContent = content.GetContent();
        
        if (!string.IsNullOrEmpty(Icon))
        {
            var iconContent = GetIconContent();
            var iconHtml = CreateIcon(iconContent, GetIconSizeClass());
            badgeContent = $"""<span class="inline-flex items-center gap-1">{iconHtml}{content}</span>""";
        }

        output.Content.SetHtmlContent(badgeContent);
    }

    private string[] GetBadgeClasses()
    {
        var baseClasses = new List<string>
        {
            "inline-flex", "items-center", "justify-center", "font-medium", "leading-none"
        };

        // Size classes
        var sizeClasses = Size switch
        {
            "sm" => new[] { "px-2", "py-1", "text-xs", "h-5" },
            "lg" => new[] { "px-4", "py-2", "text-sm", "h-8" },
            _ => new[] { "px-2.5", "py-1", "text-xs", "h-6" } // md
        };

        // Shape classes
        var shapeClasses = Pill ? new[] { "rounded-full" } : new[] { "rounded-md" };

        // Color classes based on variant and outline
        var colorClasses = GetColorClasses();

        return baseClasses
            .Concat(sizeClasses)
            .Concat(shapeClasses)
            .Concat(colorClasses)
            .ToArray();
    }

    private string[] GetColorClasses()
    {
        if (Outline)
        {
            return Variant switch
            {
                "success" => new[] { "bg-transparent", "text-green-700", "border", "border-green-300" },
                "warning" => new[] { "bg-transparent", "text-yellow-700", "border", "border-yellow-300" },
                "error" => new[] { "bg-transparent", "text-red-700", "border", "border-red-300" },
                "info" => new[] { "bg-transparent", "text-blue-700", "border", "border-blue-300" },
                _ => new[] { "bg-transparent", "text-gray-700", "border", "border-gray-300" }
            };
        }

        return Variant switch
        {
            "success" => new[] { "bg-green-100", "text-green-800" },
            "warning" => new[] { "bg-yellow-100", "text-yellow-800" },
            "error" => new[] { "bg-red-100", "text-red-800" },
            "info" => new[] { "bg-blue-100", "text-blue-800" },
            _ => new[] { "bg-gray-100", "text-gray-800" }
        };
    }

    private string GetIconContent()
    {
        return Icon switch
        {
            "check" => Icons.Check,
            "close" => Icons.Close,
            "info" => Icons.Info,
            "warning" => Icons.Warning,
            "error" => Icons.Error,
            "success" => Icons.Success,
            "star" => Icons.Star,
            "user" => Icons.User,
            _ => Icons.Info
        };
    }

    private string GetIconSizeClass()
    {
        return Size switch
        {
            "sm" => "w-3 h-3",
            "lg" => "w-5 h-5",
            _ => "w-4 h-4"
        };
    }
}