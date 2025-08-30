using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-banner")]
public class BannerTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Banner title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Banner description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Banner type (info, success, warning, error)
    /// </summary>
    public string Type { get; set; } = "info";

    /// <summary>
    /// Whether banner is dismissible
    /// </summary>
    public bool Dismissible { get; set; } = true;

    /// <summary>
    /// Auto show after delay in milliseconds
    /// </summary>
    public int DelayMs { get; set; } = 0;

    /// <summary>
    /// Whether to show icon
    /// </summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("bannerVisible", DelayMs == 0)
            .Build();

        if (DelayMs > 0)
        {
            alpineData = new AlpineDataBuilder()
                .AddBooleanProperty("bannerVisible", false)
                .AddMethod($"init() {{ setTimeout(() => {{ this.bannerVisible = true; }}, {DelayMs}); }}")
                .Build();
        }

        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "show", "bannerVisible");

        var (bgColor, borderColor, textColor, iconColor, icon) = GetTypeStyles();

        var classes = new List<string>
        {
            "fixed", "top-0", "left-0", "w-full", "h-auto", "py-2", "duration-300", "ease-out",
            bgColor, "shadow-sm", "sm:py-0", "sm:h-10", "z-50"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var bannerIcon = ShowIcon ? CreateIcon(icon, $"w-5 h-5 {iconColor}") : null;
        var closeIcon = Dismissible ? CreateIcon(Icons.Close, "w-4 h-4") : null;

        var bannerHtml = $@"
            <div class=""flex items-center justify-between px-4 sm:px-6"">
                <div class=""flex items-center"">
                    {(ShowIcon ? $@"<div class=""flex-shrink-0 mr-3"">{bannerIcon}</div>" : "")}
                    <div class=""flex-1"">
                        {(!string.IsNullOrEmpty(Title) ? $@"<p class=""text-sm font-medium {textColor}"">{EscapeJavaScriptString(Title)}</p>" : "")}
                        {(!string.IsNullOrEmpty(Description) ? $@"<p class=""text-sm {textColor}"">{EscapeJavaScriptString(Description)}</p>" : "")}
                        {(content.IsEmptyOrWhiteSpace ? "" : $@"<div class=""text-sm {textColor}"">{content}</div>")}
                    </div>
                </div>
                {(Dismissible ? $@"
                <div class=""flex-shrink-0"">
                    <button @click=""bannerVisible = false"" type=""button"" class=""inline-flex {textColor} hover:opacity-75 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500"">
                        <span class=""sr-only"">Dismiss</span>
                        {closeIcon}
                    </button>
                </div>
                " : "")}
            </div>";

        output.Content.SetHtmlContent(bannerHtml);
    }

    private (string bgColor, string borderColor, string textColor, string iconColor, string icon) GetTypeStyles()
    {
        return Type switch
        {
            "success" => (
                "bg-green-50", 
                "border-green-200", 
                "text-green-800", 
                "text-green-400",
                Icons.Success
            ),
            "warning" => (
                "bg-yellow-50", 
                "border-yellow-200", 
                "text-yellow-800", 
                "text-yellow-400",
                Icons.Warning
            ),
            "error" => (
                "bg-red-50", 
                "border-red-200", 
                "text-red-800", 
                "text-red-400",
                Icons.Error
            ),
            _ => ( // info
                "bg-blue-50", 
                "border-blue-200", 
                "text-blue-800", 
                "text-blue-400",
                Icons.Info
            )
        };
    }
}