using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-tooltip")]
public class TooltipTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Tooltip text content
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Tooltip position (top, bottom, left, right)
    /// </summary>
    public string Position { get; set; } = "top";

    /// <summary>
    /// Whether to show arrow
    /// </summary>
    public bool ShowArrow { get; set; } = true;

    /// <summary>
    /// Trigger mode (hover, click, focus)
    /// </summary>
    public string Trigger { get; set; } = "hover";

    /// <summary>
    /// Delay before showing tooltip (ms)
    /// </summary>
    public int ShowDelay { get; set; } = 100;

    /// <summary>
    /// Delay before hiding tooltip (ms)
    /// </summary>
    public int HideDelay { get; set; } = 0;

    /// <summary>
    /// Tooltip theme (dark, light)
    /// </summary>
    public string Theme { get; set; } = "dark";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        var tooltipId = GenerateUniqueId("tooltip");

        output.TagName = "div";
        
        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("tooltipVisible", false)
            .AddProperty("tooltipTimer", "null")
            .AddMethod($"showTooltip() {{ clearTimeout(this.tooltipTimer); this.tooltipTimer = setTimeout(() => {{ this.tooltipVisible = true; }}, {ShowDelay}); }}")
            .AddMethod($"hideTooltip() {{ clearTimeout(this.tooltipTimer); this.tooltipTimer = setTimeout(() => {{ this.tooltipVisible = false; }}, {HideDelay}); }}")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "relative", "inline-block");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var triggerEvents = GetTriggerEvents();
        var (positionClasses, arrowClasses) = GetPositionClasses();
        var themeClasses = GetThemeClasses();

        var tooltipHtml = $@"
            <div {triggerEvents}>
                {content}
            </div>
            
            <div x-show=""tooltipVisible""
                 x-transition:enter=""transition ease-out duration-200""
                 x-transition:enter-start=""opacity-0 scale-95""
                 x-transition:enter-end=""opacity-100 scale-100""
                 x-transition:leave=""transition ease-in duration-75""
                 x-transition:leave-start=""opacity-100 scale-100""
                 x-transition:leave-end=""opacity-0 scale-95""
                 class=""absolute {positionClasses} z-50""
                 x-cloak>
                <div class=""px-2 py-1 text-xs font-medium {themeClasses} rounded shadow-lg whitespace-nowrap"">
                    {EscapeJavaScriptString(Text)}
                    {(ShowArrow ? $@"<div class=""absolute {arrowClasses}""></div>" : "")}
                </div>
            </div>";

        output.Content.SetHtmlContent(tooltipHtml);
    }

    private string GetTriggerEvents()
    {
        return Trigger switch
        {
            "click" => @"@click=""tooltipVisible = !tooltipVisible"" @click.away=""tooltipVisible = false""",
            "focus" => @"@focus=""showTooltip()"" @blur=""hideTooltip()""",
            _ => @"@mouseenter=""showTooltip()"" @mouseleave=""hideTooltip()""" // hover
        };
    }

    private (string positionClasses, string arrowClasses) GetPositionClasses()
    {
        return Position switch
        {
            "bottom" => (
                "top-full left-1/2 transform -translate-x-1/2 mt-1",
                "bottom-full left-1/2 transform -translate-x-1/2 border-4 border-transparent border-b-gray-900"
            ),
            "left" => (
                "right-full top-1/2 transform -translate-y-1/2 mr-1",
                "left-full top-1/2 transform -translate-y-1/2 border-4 border-transparent border-l-gray-900"
            ),
            "right" => (
                "left-full top-1/2 transform -translate-y-1/2 ml-1",
                "right-full top-1/2 transform -translate-y-1/2 border-4 border-transparent border-r-gray-900"
            ),
            _ => ( // top
                "bottom-full left-1/2 transform -translate-x-1/2 mb-1",
                "top-full left-1/2 transform -translate-x-1/2 border-4 border-transparent border-t-gray-900"
            )
        };
    }

    private string GetThemeClasses()
    {
        return Theme switch
        {
            "light" => "bg-white text-gray-900 border border-gray-200",
            _ => "bg-gray-900 text-white" // dark
        };
    }
}