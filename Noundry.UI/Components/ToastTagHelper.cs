using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-toast")]
public class ToastTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Toast type (info, success, warning, error)
    /// </summary>
    public string Type { get; set; } = "info";

    /// <summary>
    /// Toast title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Position of toast (top-right, top-left, bottom-right, bottom-left, top-center, bottom-center)
    /// </summary>
    public string Position { get; set; } = "top-right";

    /// <summary>
    /// Whether toast auto-dismisses
    /// </summary>
    public bool AutoDismiss { get; set; } = true;

    /// <summary>
    /// Auto-dismiss delay in milliseconds
    /// </summary>
    public int Delay { get; set; } = 5000;

    /// <summary>
    /// Whether to show dismiss button
    /// </summary>
    public bool Dismissible { get; set; } = true;

    /// <summary>
    /// Whether to show icon
    /// </summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Trigger button text
    /// </summary>
    public string TriggerText { get; set; } = "Show Toast";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        
        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("toastVisible", false)
            .AddMethod("showToast() { this.toastVisible = true; " + (AutoDismiss ? $"setTimeout(() => {{ this.toastVisible = false; }}, {Delay});" : "") + " }")
            .AddMethod("hideToast() { this.toastVisible = false; }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);

        var (bgColor, borderColor, textColor, iconColor, icon) = GetTypeStyles();
        var positionClasses = GetPositionClasses();
        var toastIcon = ShowIcon ? CreateIcon(icon, $"w-5 h-5 {iconColor}") : null;
        var closeIcon = Dismissible ? CreateIcon(Icons.Close, "w-4 h-4") : null;

        var toastHtml = $"""
            <button @click="showToast()" class="inline-flex items-center justify-center px-4 py-2 text-sm font-medium tracking-wide text-white transition-colors duration-200 rounded-md bg-neutral-950 hover:bg-neutral-900 focus:ring-2 focus:ring-offset-2 focus:ring-neutral-900 focus:shadow-outline focus:outline-none">
                {EscapeJavaScriptString(TriggerText)}
            </button>

            <template x-teleport="body">
                <div x-show="toastVisible"
                    x-transition:enter="transform ease-out duration-300 transition"
                    x-transition:enter-start="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-4"
                    x-transition:enter-end="translate-y-0 opacity-100 sm:translate-x-0"
                    x-transition:leave="transition ease-in duration-100"
                    x-transition:leave-start="opacity-100"
                    x-transition:leave-end="opacity-0"
                    class="fixed {positionClasses} z-50 w-full max-w-sm mx-auto sm:mx-0"
                    x-cloak>
                    <div class="relative p-4 {bgColor} {borderColor} border rounded-lg shadow-lg">
                        <div class="flex items-start">
                            {(ShowIcon ? $"""<div class="flex-shrink-0">{toastIcon}</div>""" : "")}
                            <div class="{(ShowIcon ? "ml-3" : "")} flex-1">
                                {(!string.IsNullOrEmpty(Title) ? $"""<p class="font-medium {textColor}">{EscapeJavaScriptString(Title)}</p>""" : "")}
                                <div class="{(!string.IsNullOrEmpty(Title) ? "mt-1 " : "")}text-sm {textColor}">
                                    {content}
                                </div>
                            </div>
                            {(Dismissible ? $"""
                            <div class="ml-4 flex-shrink-0">
                                <button @click="hideToast()" type="button" class="inline-flex {textColor} hover:opacity-75 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500">
                                    <span class="sr-only">Close</span>
                                    {closeIcon}
                                </button>
                            </div>
                            """ : "")}
                        </div>
                    </div>
                </div>
            </template>
            """;

        output.Content.SetHtmlContent(toastHtml);
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

    private string GetPositionClasses()
    {
        return Position switch
        {
            "top-left" => "top-0 left-0 mt-4 ml-4",
            "top-center" => "top-0 left-1/2 transform -translate-x-1/2 mt-4",
            "top-right" => "top-0 right-0 mt-4 mr-4",
            "bottom-left" => "bottom-0 left-0 mb-4 ml-4",
            "bottom-center" => "bottom-0 left-1/2 transform -translate-x-1/2 mb-4",
            "bottom-right" => "bottom-0 right-0 mb-4 mr-4",
            _ => "top-0 right-0 mt-4 mr-4" // default to top-right
        };
    }
}