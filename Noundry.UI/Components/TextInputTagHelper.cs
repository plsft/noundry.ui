using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-text-input")]
public class TextInputTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Input type (text, email, password, number, url, tel)
    /// </summary>
    public string Type { get; set; } = "text";

    /// <summary>
    /// Input size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Label text
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Help text displayed below the input
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Icon to display in input (left side)
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Whether to show validation state
    /// </summary>
    public bool ShowValidation { get; set; } = true;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var inputId = GetInputId();
        var inputName = GetInputName();
        var inputValue = GetInputValue()?.ToString() ?? string.Empty;
        var labelText = Label ?? GetDisplayName();

        output.TagName = "div";
        AddCssClasses(output, "space-y-1");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var inputClasses = GetInputClasses();
        var hasError = !string.IsNullOrEmpty(ErrorMessage);
        var hasIcon = !string.IsNullOrEmpty(Icon);

        var iconContent = hasIcon ? CreateIcon(GetIconContent(Icon!), "w-5 h-5 text-gray-400") : null;

        var inputHtml = $"""
            {(!string.IsNullOrEmpty(labelText) ? $"""
            <label for="{inputId}" class="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                {EscapeJavaScriptString(labelText)}{(Required ? " <span class=\"text-red-500\">*</span>" : "")}
            </label>
            """ : "")}
            
            <div class="relative">
                {(hasIcon ? $"""
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    {iconContent}
                </div>
                """ : "")}
                
                <input 
                    type="{Type}"
                    id="{inputId}"
                    name="{inputName}"
                    value="{EscapeJavaScriptString(inputValue)}"
                    {(!string.IsNullOrEmpty(Placeholder) ? $"""placeholder="{EscapeJavaScriptString(Placeholder)}" """ : "")}
                    class="{string.Join(" ", inputClasses)} {(hasIcon ? "pl-10" : "")} {(hasError ? "border-red-300 focus:border-red-300 focus:ring-red-500" : "border-neutral-300 focus:border-neutral-300 focus:ring-neutral-400")}"
                    {(Disabled ? "disabled" : "")}
                    {(Required ? "required" : "")}
                    {(AspFor != null ? $"""aria-describedby="{inputId}-description" """ : "")}
                />
            </div>

            {(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) ? $"""
            <p id="{inputId}-description" class="text-sm text-gray-500">
                {EscapeJavaScriptString(HelpText)}
            </p>
            """ : "")}

            {(!string.IsNullOrEmpty(ErrorMessage) ? $"""
            <p id="{inputId}-description" class="text-sm text-red-600">
                {EscapeJavaScriptString(ErrorMessage)}
            </p>
            """ : "")}
            """;

        output.Content.SetHtmlContent(inputHtml);
    }

    private string[] GetInputClasses()
    {
        var baseClasses = new[]
        {
            "flex", "w-full", "px-3", "py-2", "text-sm", "bg-white", "border", "rounded-md",
            "peer", "ring-offset-background", "placeholder:text-neutral-400", "focus:outline-none",
            "focus:ring-2", "focus:ring-offset-2", "disabled:cursor-not-allowed", "disabled:opacity-50"
        };

        var sizeClasses = Size switch
        {
            "sm" => new[] { "h-8", "text-xs" },
            "lg" => new[] { "h-12", "text-base" },
            _ => new[] { "h-10" } // md
        };

        return baseClasses.Concat(sizeClasses).ToArray();
    }

    private string GetIconContent(string iconName)
    {
        return iconName switch
        {
            "user" => Icons.User,
            "search" => Icons.Search,
            "calendar" => Icons.Calendar,
            "info" => Icons.Info,
            "settings" => Icons.Settings,
            _ => Icons.Info
        };
    }
}