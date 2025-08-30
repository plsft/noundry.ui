using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-alert")]
public class AlertTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Alert type (info, success, warning, error)
    /// </summary>
    public string Type { get; set; } = "info";

    /// <summary>
    /// Alert title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Whether the alert is dismissible
    /// </summary>
    public bool Dismissible { get; set; } = false;

    /// <summary>
    /// Whether to show an icon
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

        var (bgColor, borderColor, textColor, iconColor, icon) = GetTypeStyles();

        var classes = new List<string>
        {
            "relative", "p-4", "rounded-md", "border", borderColor, bgColor
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        if (Dismissible)
        {
            var alpineData = new AlpineDataBuilder()
                .AddBooleanProperty("alertVisible", true)
                .Build();

            SetAlpineAttribute(output, "data", alpineData);
            SetAlpineAttribute(output, "show", "alertVisible");
            
            classes.AddRange(new[] { "transition-all", "duration-300" });
        }

        AddCssClasses(output, classes.ToArray());

        var alertIcon = ShowIcon ? CreateIcon(icon, $"w-5 h-5 {iconColor}") : null;
        var closeIcon = Dismissible ? CreateIcon(Icons.Close, "w-4 h-4") : null;

        var alertHtml = $"""
            <div class="flex">
                {(ShowIcon ? $"""<div class="flex-shrink-0">{alertIcon}</div>""" : "")}
                <div class="{(ShowIcon ? "ml-3" : "")} flex-1">
                    {(!string.IsNullOrEmpty(Title) ? $"""<h3 class="text-sm font-medium {textColor}">{EscapeJavaScriptString(Title)}</h3>""" : "")}
                    <div class="{(!string.IsNullOrEmpty(Title) ? "mt-2 " : "")}text-sm {textColor}">
                        {content}
                    </div>
                </div>
                {(Dismissible ? $"""
                <div class="ml-auto pl-3">
                    <div class="-mx-1.5 -my-1.5">
                        <button @click="alertVisible = false" type="button" class="inline-flex rounded-md p-1.5 {textColor} hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-gray-50 focus:ring-gray-600">
                            <span class="sr-only">Dismiss</span>
                            {closeIcon}
                        </button>
                    </div>
                </div>
                """ : "")}
            </div>
            """;

        output.Content.SetHtmlContent(alertHtml);
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