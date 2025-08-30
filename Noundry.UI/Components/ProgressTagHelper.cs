using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-progress")]
public class ProgressTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Current progress value (0-100)
    /// </summary>
    public int Value { get; set; } = 0;

    /// <summary>
    /// Maximum value (default 100)
    /// </summary>
    public int Max { get; set; } = 100;

    /// <summary>
    /// Progress bar color (blue, green, red, yellow, gray)
    /// </summary>
    public string Color { get; set; } = "blue";

    /// <summary>
    /// Progress bar size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Whether to show percentage text
    /// </summary>
    public bool ShowPercentage { get; set; } = false;

    /// <summary>
    /// Whether to animate progress changes
    /// </summary>
    public bool Animated { get; set; } = true;

    /// <summary>
    /// Label text
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Whether to show striped pattern
    /// </summary>
    public bool Striped { get; set; } = false;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        output.TagName = "div";
        AddCssClasses(output, "w-full");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var percentage = Math.Max(0, Math.Min(100, (Value * 100) / Math.Max(1, Max)));
        var (heightClasses, bgColorClasses, textColorClasses) = GetSizeAndColorClasses();

        var progressHtml = $@"
            {(!string.IsNullOrEmpty(Label) ? $@"
            <div class=""flex items-center justify-between mb-1"">
                <span class=""text-sm font-medium text-gray-700"">{EscapeJavaScriptString(Label)}</span>
                {(ShowPercentage ? $@"<span class=""text-sm text-gray-500"">{percentage}%</span>" : "")}
            </div>
            " : "")}
            
            <div class=""relative {heightClasses} overflow-hidden rounded-full bg-neutral-100"" role=""progressbar"" aria-valuenow=""{Value}"" aria-valuemin=""0"" aria-valuemax=""{Max}"">
                <div class=""h-full {bgColorClasses} {(Animated ? "transition-all duration-300 ease-out" : "")} {(Striped ? "bg-stripes" : "")}""
                     style=""width: {percentage}%"">
                </div>
            </div>
            
            {(Striped ? @"
            <style>
                @keyframes progress-stripe {
                    0% { background-position: 0 0; }
                    100% { background-position: 40px 0; }
                }
                .bg-stripes {
                    background-image: linear-gradient(45deg, rgba(255,255,255,0.15) 25%, transparent 25%, transparent 50%, rgba(255,255,255,0.15) 50%, rgba(255,255,255,0.15) 75%, transparent 75%, transparent);
                    background-size: 40px 40px;
                    animation: progress-stripe 1s linear infinite;
                }
            </style>
            " : "")}";

        output.Content.SetHtmlContent(progressHtml);
    }

    private (string heightClasses, string bgColorClasses, string textColorClasses) GetSizeAndColorClasses()
    {
        var heightClasses = Size switch
        {
            "sm" => "h-2",
            "lg" => "h-4",
            _ => "h-3" // md
        };

        var (bgColorClasses, textColorClasses) = Color switch
        {
            "green" => ("bg-green-500", "text-green-600"),
            "red" => ("bg-red-500", "text-red-600"),
            "yellow" => ("bg-yellow-500", "text-yellow-600"),
            "gray" => ("bg-gray-500", "text-gray-600"),
            _ => ("bg-blue-500", "text-blue-600") // blue
        };

        return (heightClasses, bgColorClasses, textColorClasses);
    }
}