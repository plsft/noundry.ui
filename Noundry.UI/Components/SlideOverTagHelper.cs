using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-slide-over")]
public class SlideOverTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Slide over title
    /// </summary>
    public string Title { get; set; } = "Slide Over";

    /// <summary>
    /// Button text to open slide over
    /// </summary>
    public string ButtonText { get; set; } = "Open";

    /// <summary>
    /// Button variant for trigger button
    /// </summary>
    public string ButtonVariant { get; set; } = "primary";

    /// <summary>
    /// Slide position (left, right)
    /// </summary>
    public string Position { get; set; } = "right";

    /// <summary>
    /// Slide over size (sm, md, lg, xl)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Whether to show close button
    /// </summary>
    public bool Closable { get; set; } = true;

    /// <summary>
    /// Whether to close on backdrop click
    /// </summary>
    public bool CloseOnBackdrop { get; set; } = true;

    /// <summary>
    /// Whether to close on escape key
    /// </summary>
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Additional CSS classes for slide over content
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "div";
        
        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("slideOverOpen", false)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        
        if (CloseOnEscape)
        {
            output.Attributes.SetAttribute("@keydown.escape.window", "slideOverOpen = false");
        }

        AddCssClasses(output, "relative", "z-50");

        var buttonClasses = GetButtonClasses();
        var (positionClasses, slideClasses, widthClasses) = GetPositionAndSizeClasses();
        var closeIcon = CreateIcon(Icons.Close, "w-6 h-6");

        var slideOverHtml = $@"
            <button @click=""slideOverOpen=true"" class=""{string.Join(" ", buttonClasses)}"">
                {EscapeJavaScriptString(ButtonText)}
            </button>
            
            <template x-teleport=""body"">
                <div x-show=""slideOverOpen"" class=""fixed inset-0 z-[99] overflow-hidden"" x-cloak>
                    <div x-show=""slideOverOpen""
                         x-transition:enter=""ease-out duration-300""
                         x-transition:enter-start=""opacity-0""
                         x-transition:enter-end=""opacity-100""
                         x-transition:leave=""ease-in duration-300""
                         x-transition:leave-start=""opacity-100""
                         x-transition:leave-end=""opacity-0""
                         {(CloseOnBackdrop ? @"@click=""slideOverOpen=false""" : "")}
                         class=""absolute inset-0 bg-gray-500 bg-opacity-75""></div>
                    
                    <div class=""fixed inset-y-0 {positionClasses} flex max-w-full"">
                        <div x-show=""slideOverOpen""
                             x-transition:enter=""transform transition ease-in-out duration-300""
                             x-transition:enter-start=""{slideClasses.enter}""
                             x-transition:enter-end=""{slideClasses.end}""
                             x-transition:leave=""transform transition ease-in-out duration-300""
                             x-transition:leave-start=""{slideClasses.end}""
                             x-transition:leave-end=""{slideClasses.enter}""
                             class=""{widthClasses}"">
                            
                            <div class=""flex h-full flex-col overflow-y-scroll bg-white shadow-xl {CssClass}"">
                                <div class=""px-4 py-6 sm:px-6"">
                                    <div class=""flex items-start justify-between"">
                                        <h2 class=""text-lg font-medium text-gray-900"">{EscapeJavaScriptString(Title)}</h2>
                                        {(Closable ? $@"
                                        <div class=""ml-3 flex h-7 items-center"">
                                            <button @click=""slideOverOpen=false"" type=""button"" class=""rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"">
                                                <span class=""sr-only"">Close panel</span>
                                                {closeIcon}
                                            </button>
                                        </div>
                                        " : "")}
                                    </div>
                                </div>
                                
                                <div class=""relative flex-1 px-4 py-6 sm:px-6"">
                                    {content}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </template>";

        output.Content.SetHtmlContent(slideOverHtml);
    }

    private string[] GetButtonClasses()
    {
        var baseClasses = new[]
        {
            "inline-flex", "justify-center", "items-center", "px-4", "py-2", "text-sm", "font-medium",
            "rounded-md", "border", "transition-colors", "focus:outline-none", "focus:ring-2", "focus:ring-offset-2",
            "disabled:opacity-50", "disabled:pointer-events-none"
        };

        var variantClasses = ButtonVariant switch
        {
            "secondary" => new[] { "bg-white", "text-gray-700", "border-gray-300", "hover:bg-gray-50", "focus:ring-gray-500" },
            "danger" => new[] { "bg-red-600", "text-white", "border-red-600", "hover:bg-red-700", "focus:ring-red-500" },
            _ => new[] { "bg-blue-600", "text-white", "border-blue-600", "hover:bg-blue-700", "focus:ring-blue-500" }
        };

        return baseClasses.Concat(variantClasses).ToArray();
    }

    private (string positionClasses, dynamic slideClasses, string widthClasses) GetPositionAndSizeClasses()
    {
        var positionClasses = Position == "left" ? "left-0" : "right-0";
        
        var slideClasses = Position == "left" 
            ? new { enter = "-translate-x-full", end = "translate-x-0" }
            : new { enter = "translate-x-full", end = "translate-x-0" };

        var widthClasses = Size switch
        {
            "sm" => "w-screen max-w-sm",
            "lg" => "w-screen max-w-2xl",
            "xl" => "w-screen max-w-4xl",
            _ => "w-screen max-w-md" // md
        };

        return (positionClasses, slideClasses, widthClasses);
    }
}

[HtmlTargetElement("noundry-slide-over-content")]
public class SlideOverContentTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        output.TagName = "div";
        AddCssClasses(output, "slide-over-content");
        output.Content.SetHtmlContent(content);
    }
}