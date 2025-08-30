using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-button")]
public class ButtonTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Button type (button, submit, reset)
    /// </summary>
    public string Type { get; set; } = "button";

    /// <summary>
    /// Button variant (primary, secondary, danger, success, warning)
    /// </summary>
    public string Variant { get; set; } = "primary";

    /// <summary>
    /// Button size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Whether the button is disabled
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether to show as loading state
    /// </summary>
    public bool Loading { get; set; }

    /// <summary>
    /// Loading text to display
    /// </summary>
    public string? LoadingText { get; set; } = "Loading...";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        
        output.TagName = "button";
        output.Attributes.SetAttribute("type", Type);

        var cssClasses = GetCssClasses();
        AddCssClasses(output, cssClasses);

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        if (Disabled || Loading)
        {
            output.Attributes.SetAttribute("disabled", "disabled");
            output.Attributes.SetAttribute("aria-disabled", "true");
        }

        if (Loading)
        {
            SetupLoadingState(output, content);
        }
        else
        {
            output.Content.SetHtmlContent(content);
        }
    }

    private string[] GetCssClasses()
    {
        var baseClasses = new[]
        {
            "inline-flex", "items-center", "justify-center", "font-medium", "tracking-wide",
            "transition-colors", "duration-200", "rounded-md", "focus:ring-2", "focus:ring-offset-2",
            "focus:shadow-outline", "focus:outline-none"
        };

        var sizeClasses = Size switch
        {
            "sm" => new[] { "px-3", "py-1.5", "text-xs" },
            "lg" => new[] { "px-6", "py-3", "text-base" },
            _ => new[] { "px-4", "py-2", "text-sm" }
        };

        var variantClasses = Variant switch
        {
            "secondary" => new[]
            {
                "bg-white", "text-neutral-700", "border", "border-neutral-300",
                "hover:bg-neutral-50", "focus:ring-neutral-200", "disabled:opacity-50", "disabled:pointer-events-none"
            },
            "danger" => new[]
            {
                "bg-red-600", "text-white", "hover:bg-red-700", "focus:ring-red-500",
                "disabled:opacity-50", "disabled:pointer-events-none"
            },
            "success" => new[]
            {
                "bg-green-600", "text-white", "hover:bg-green-700", "focus:ring-green-500",
                "disabled:opacity-50", "disabled:pointer-events-none"
            },
            "warning" => new[]
            {
                "bg-yellow-500", "text-white", "hover:bg-yellow-600", "focus:ring-yellow-400",
                "disabled:opacity-50", "disabled:pointer-events-none"
            },
            _ => new[] // primary
            {
                "bg-neutral-950", "text-white", "hover:bg-neutral-900", "focus:ring-neutral-900",
                "disabled:opacity-50", "disabled:pointer-events-none"
            }
        };

        return baseClasses.Concat(sizeClasses).Concat(variantClasses).ToArray();
    }

    private void SetupLoadingState(TagHelperOutput output, IHtmlContent originalContent)
    {
        var loadingSpinner = """
            <svg x-show="loading" class="animate-spin -ml-1 mr-2 h-4 w-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            """;

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("loading", true)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        
        var contentHtml = $"""
            {loadingSpinner}
            <span x-show="loading">{EscapeJavaScriptString(LoadingText)}</span>
            <span x-show="!loading">{originalContent.ToString()}</span>
            """;
        
        output.Content.SetHtmlContent(contentHtml);
    }
}