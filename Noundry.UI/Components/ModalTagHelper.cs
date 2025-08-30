using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-modal")]
public class ModalTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Modal title
    /// </summary>
    public string Title { get; set; } = "Modal Title";

    /// <summary>
    /// Button text to open modal
    /// </summary>
    public string ButtonText { get; set; } = "Open";

    /// <summary>
    /// Button variant for trigger button
    /// </summary>
    public string ButtonVariant { get; set; } = "primary";

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
    /// Maximum width class for modal
    /// </summary>
    public string MaxWidth { get; set; } = "sm:max-w-lg";

    /// <summary>
    /// Additional CSS classes for modal content
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        var modalId = GenerateUniqueId("modal");

        output.TagName = "div";
        
        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("modalOpen", false)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        
        if (CloseOnEscape)
        {
            output.Attributes.SetAttribute("@keydown.escape.window", "modalOpen = false");
        }

        AddCssClasses(output, "relative", "z-50", "w-auto", "h-auto");

        var buttonClasses = GetButtonClasses();
        var closeIcon = CreateIcon(Icons.Close, "w-5 h-5");

        var modalHtml = $"""
            <button @click="modalOpen=true" class="{string.Join(" ", buttonClasses)}">{ButtonText}</button>
            <template x-teleport="body">
                <div x-show="modalOpen" class="fixed top-0 left-0 z-[99] flex items-center justify-center w-screen h-screen" x-cloak>
                    <div x-show="modalOpen"
                        x-transition:enter="ease-out duration-300"
                        x-transition:enter-start="opacity-0"
                        x-transition:enter-end="opacity-100"
                        x-transition:leave="ease-in duration-300"
                        x-transition:leave-start="opacity-100"
                        x-transition:leave-end="opacity-0"
                        {(CloseOnBackdrop ? "@click=\"modalOpen=false\"" : "")} 
                        class="absolute inset-0 w-full h-full bg-black/40"></div>
                    <div x-show="modalOpen"
                        x-trap.inert.noscroll="modalOpen"
                        x-transition:enter="ease-out duration-300"
                        x-transition:enter-start="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
                        x-transition:enter-end="opacity-100 translate-y-0 sm:scale-100"
                        x-transition:leave="ease-in duration-200"
                        x-transition:leave-start="opacity-100 translate-y-0 sm:scale-100"
                        x-transition:leave-end="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
                        class="relative px-7 py-6 w-full bg-white {MaxWidth} sm:rounded-lg {CssClass}">
                        <div class="flex justify-between items-center pb-2">
                            <h3 class="text-lg font-semibold">{EscapeJavaScriptString(Title)}</h3>
                            {(Closable ? $"""<button @click="modalOpen=false" class="flex absolute top-0 right-0 justify-center items-center mt-5 mr-5 w-8 h-8 text-gray-600 rounded-full hover:text-gray-800 hover:bg-gray-50">{closeIcon}</button>""" : "")}
                        </div>
                        <div class="relative w-auto">
                            {content}
                        </div>
                    </div>
                </div>
            </template>
            """;

        output.Content.SetHtmlContent(modalHtml);
    }

    private string[] GetButtonClasses()
    {
        var baseClasses = new[]
        {
            "inline-flex", "justify-center", "items-center", "px-4", "py-2", "h-10", "text-sm", "font-medium",
            "rounded-md", "border", "transition-colors", "focus:outline-none", "focus:ring-2", "focus:ring-offset-2",
            "disabled:opacity-50", "disabled:pointer-events-none"
        };

        var variantClasses = ButtonVariant switch
        {
            "secondary" => new[] { "bg-white", "text-neutral-700", "border-neutral-300", "hover:bg-neutral-100", "active:bg-white", "focus:bg-white", "focus:ring-neutral-200/60" },
            "danger" => new[] { "bg-red-600", "text-white", "border-red-600", "hover:bg-red-700", "focus:ring-red-500" },
            "success" => new[] { "bg-green-600", "text-white", "border-green-600", "hover:bg-green-700", "focus:ring-green-500" },
            _ => new[] { "bg-neutral-950", "text-white", "border-neutral-950", "hover:bg-neutral-900", "focus:ring-neutral-900" }
        };

        return baseClasses.Concat(variantClasses).ToArray();
    }
}