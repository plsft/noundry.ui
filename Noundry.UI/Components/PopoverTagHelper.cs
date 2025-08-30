using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-popover")]
public class PopoverTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Popover title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Trigger text or content
    /// </summary>
    public string TriggerText { get; set; } = "Click me";

    /// <summary>
    /// Popover position (top, bottom, left, right)
    /// </summary>
    public string Position { get; set; } = "bottom";

    /// <summary>
    /// Whether to show arrow
    /// </summary>
    public bool ShowArrow { get; set; } = true;

    /// <summary>
    /// Trigger mode (click, hover)
    /// </summary>
    public string Trigger { get; set; } = "click";

    /// <summary>
    /// Whether to close on outside click
    /// </summary>
    public bool CloseOnOutsideClick { get; set; } = true;

    /// <summary>
    /// Popover width
    /// </summary>
    public string Width { get; set; } = "w-[300px]";

    /// <summary>
    /// Maximum width
    /// </summary>
    public string MaxWidth { get; set; } = "max-w-lg";

    /// <summary>
    /// Additional CSS classes for trigger
    /// </summary>
    public string? TriggerCssClass { get; set; }

    /// <summary>
    /// Additional CSS classes for popover content
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("popoverOpen", false)
            .AddMethod("togglePopover() { this.popoverOpen = !this.popoverOpen; }")
            .AddMethod("closePopover() { this.popoverOpen = false; }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "relative", "inline-block");

        var triggerEvents = GetTriggerEvents();
        var (positionClasses, arrowClasses) = GetPositionClasses();

        var triggerClasses = new List<string>
        {
            "inline-flex", "items-center", "justify-center", "px-4", "py-2", "text-sm", 
            "font-medium", "text-gray-700", "bg-white", "border", "border-gray-300", 
            "rounded-md", "hover:bg-gray-50", "focus:outline-none", "focus:ring-2", 
            "focus:ring-blue-500", "focus:ring-offset-2"
        };

        if (!string.IsNullOrEmpty(TriggerCssClass))
        {
            triggerClasses.Add(TriggerCssClass);
        }

        var contentClasses = new List<string>
        {
            "absolute", "z-50", Width, MaxWidth, "p-4", "bg-white", "border", "border-gray-200", 
            "rounded-lg", "shadow-lg"
        };

        contentClasses.AddRange(positionClasses.Split(' '));

        if (!string.IsNullOrEmpty(CssClass))
        {
            contentClasses.Add(CssClass);
        }

        var popoverHtml = $@"
            <button type=""button"" {triggerEvents} class=""{string.Join(" ", triggerClasses)}"">
                {EscapeJavaScriptString(TriggerText)}
            </button>
            
            <div x-show=""popoverOpen""
                 {(CloseOnOutsideClick ? "@click.away=\"closePopover()\"" : "")}
                 x-transition:enter=""transition ease-out duration-200""
                 x-transition:enter-start=""opacity-0 scale-95""
                 x-transition:enter-end=""opacity-100 scale-100""
                 x-transition:leave=""transition ease-in duration-75""
                 x-transition:leave-start=""opacity-100 scale-100""
                 x-transition:leave-end=""opacity-0 scale-95""
                 class=""{string.Join(" ", contentClasses)}""
                 x-cloak>
                
                {(!string.IsNullOrEmpty(Title) ? $@"
                <div class=""mb-3 pb-2 border-b border-gray-200"">
                    <h3 class=""text-lg font-semibold text-gray-900"">{EscapeJavaScriptString(Title)}</h3>
                </div>
                " : "")}
                
                <div class=""text-sm text-gray-700"">
                    {content}
                </div>
                
                {(ShowArrow ? $@"<div class=""{arrowClasses}""></div>" : "")}
            </div>";

        output.Content.SetHtmlContent(popoverHtml);
    }

    private string GetTriggerEvents()
    {
        return Trigger switch
        {
            "hover" => @"@mouseenter=""popoverOpen = true"" @mouseleave=""popoverOpen = false""",
            _ => @"@click=""togglePopover()""" // click
        };
    }

    private (string positionClasses, string arrowClasses) GetPositionClasses()
    {
        return Position switch
        {
            "top" => (
                "bottom-full left-1/2 transform -translate-x-1/2 mb-2",
                "absolute top-full left-1/2 transform -translate-x-1/2 w-0 h-0 border-l-8 border-r-8 border-t-8 border-transparent border-t-white"
            ),
            "left" => (
                "right-full top-1/2 transform -translate-y-1/2 mr-2",
                "absolute left-full top-1/2 transform -translate-y-1/2 w-0 h-0 border-t-8 border-b-8 border-l-8 border-transparent border-l-white"
            ),
            "right" => (
                "left-full top-1/2 transform -translate-y-1/2 ml-2",
                "absolute right-full top-1/2 transform -translate-y-1/2 w-0 h-0 border-t-8 border-b-8 border-r-8 border-transparent border-r-white"
            ),
            _ => ( // bottom
                "top-full left-1/2 transform -translate-x-1/2 mt-2",
                "absolute bottom-full left-1/2 transform -translate-x-1/2 w-0 h-0 border-l-8 border-r-8 border-b-8 border-transparent border-b-white"
            )
        };
    }
}