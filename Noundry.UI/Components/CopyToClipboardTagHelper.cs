using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-copy-to-clipboard")]
public class CopyToClipboardTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Text to copy to clipboard
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Button text
    /// </summary>
    public string ButtonText { get; set; } = "Copy";

    /// <summary>
    /// Success message to show after copying
    /// </summary>
    public string SuccessMessage { get; set; } = "Copied!";

    /// <summary>
    /// Duration to show success message (ms)
    /// </summary>
    public int SuccessDuration { get; set; } = 2000;

    /// <summary>
    /// Whether to show the text being copied
    /// </summary>
    public bool ShowText { get; set; } = true;

    /// <summary>
    /// Whether to show copy icon
    /// </summary>
    public bool ShowIcon { get; set; } = true;

    /// <summary>
    /// Button variant
    /// </summary>
    public string Variant { get; set; } = "secondary";

    /// <summary>
    /// Button size
    /// </summary>
    public string Size { get; set; } = "sm";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        var textToCopy = !content.IsEmptyOrWhiteSpace() ? content.GetContent() : Text;

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("copied", false)
            .AddStringProperty("textToCopy", textToCopy)
            .AddMethod($"copyToClipboard() {{ navigator.clipboard.writeText(this.textToCopy).then(() => {{ this.copied = true; setTimeout(() => {{ this.copied = false; }}, {SuccessDuration}); }}).catch(err => {{ console.error('Failed to copy: ', err); }}); }}")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);

        var classes = new List<string> { "relative", "inline-flex", "items-center" };
        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var buttonClasses = GetButtonClasses();
        var copyIcon = ShowIcon ? CreateIcon(Icons.Copy, GetIconSize()) : null;
        var checkIcon = CreateIcon(Icons.Check, GetIconSize());

        var copyHtml = $@"
            {(ShowText ? $@"
            <div class=""flex items-center space-x-2"">
                <code class=""px-2 py-1 text-xs bg-gray-100 rounded border"">{EscapeJavaScriptString(textToCopy)}</code>
            </div>
            " : "")}
            
            <button type=""button"" @click=""copyToClipboard()"" class=""{string.Join(" ", buttonClasses)} {(ShowText ? "ml-2" : "")}"">
                <span x-show=""!copied"" class=""flex items-center"">
                    {(ShowIcon ? copyIcon : "")}
                    <span class=""{(ShowIcon ? "ml-1" : "")}"">{EscapeJavaScriptString(ButtonText)}</span>
                </span>
                <span x-show=""copied"" class=""flex items-center"" x-cloak>
                    {checkIcon}
                    <span class=""ml-1"">{EscapeJavaScriptString(SuccessMessage)}</span>
                </span>
            </button>";

        output.Content.SetHtmlContent(copyHtml);
    }

    private string[] GetButtonClasses()
    {
        var baseClasses = new[]
        {
            "inline-flex", "items-center", "justify-center", "font-medium", "rounded-md",
            "transition-colors", "focus:outline-none", "focus:ring-2", "focus:ring-offset-2"
        };

        var sizeClasses = Size switch
        {
            "xs" => new[] { "px-2", "py-1", "text-xs" },
            "sm" => new[] { "px-3", "py-1.5", "text-xs" },
            "lg" => new[] { "px-4", "py-2", "text-base" },
            _ => new[] { "px-3", "py-2", "text-sm" } // md
        };

        var variantClasses = Variant switch
        {
            "primary" => new[] { "bg-blue-600", "text-white", "hover:bg-blue-700", "focus:ring-blue-500" },
            "success" => new[] { "bg-green-600", "text-white", "hover:bg-green-700", "focus:ring-green-500" },
            "danger" => new[] { "bg-red-600", "text-white", "hover:bg-red-700", "focus:ring-red-500" },
            _ => new[] { "bg-white", "text-gray-700", "border", "border-gray-300", "hover:bg-gray-50", "focus:ring-gray-500" }
        };

        return baseClasses.Concat(sizeClasses).Concat(variantClasses).ToArray();
    }

    private string GetIconSize()
    {
        return Size switch
        {
            "xs" => "w-3 h-3",
            "sm" => "w-4 h-4",
            "lg" => "w-5 h-5",
            _ => "w-4 h-4" // md
        };
    }
}