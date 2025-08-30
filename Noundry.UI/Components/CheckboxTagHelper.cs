using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-checkbox")]
public class CheckboxTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Label text for the checkbox
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Help text displayed below the checkbox
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Whether the checkbox is initially checked
    /// </summary>
    public bool Checked { get; set; }

    /// <summary>
    /// Checkbox size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Color theme (blue, green, red, gray)
    /// </summary>
    public string Color { get; set; } = "blue";

    /// <summary>
    /// Whether to use indeterminate state
    /// </summary>
    public bool Indeterminate { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var checkboxId = GetInputId();
        var inputName = GetInputName();
        var isChecked = Checked || (AspFor?.Model as bool? ?? false);
        var labelText = Label ?? GetDisplayName();

        output.TagName = "div";
        AddCssClasses(output, "flex", "items-start", "space-x-3");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var (checkboxClasses, colorClasses) = GetCheckboxClasses();

        var checkboxHtml = $@"
            <div class=""flex items-center"">
                <input 
                    type=""checkbox""
                    id=""{checkboxId}""
                    name=""{inputName}""
                    value=""true""
                    {(isChecked ? "checked" : "")}
                    {(Disabled ? "disabled" : "")}
                    {(Required ? "required" : "")}
                    {(Indeterminate ? @"data-indeterminate=""true""" : "")}
                    class=""{string.Join(" ", checkboxClasses)} {colorClasses}""
                />
            </div>
            
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <div class=""flex-1"">
                <label for=""{checkboxId}"" class=""text-sm font-medium text-gray-700 cursor-pointer"">
                    {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
                </label>
                {(!string.IsNullOrEmpty(HelpText) ? $@"
                <p class=""mt-1 text-xs text-gray-500"">{EscapeJavaScriptString(HelpText)}</p>
                " : "")}
            </div>
            " : "")}";

        output.Content.SetHtmlContent(checkboxHtml);

        if (Indeterminate)
        {
            // Add script to set indeterminate state
            var script = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        const checkbox = document.getElementById('{checkboxId}');
                        if (checkbox) checkbox.indeterminate = true;
                    }});
                </script>";
            output.PostContent.SetHtmlContent(script);
        }
    }

    private (string[] checkboxClasses, string colorClasses) GetCheckboxClasses()
    {
        var sizeClasses = Size switch
        {
            "sm" => new[] { "w-3", "h-3" },
            "lg" => new[] { "w-5", "h-5" },
            _ => new[] { "w-4", "h-4" } // md
        };

        var baseClasses = new[]
        {
            "bg-gray-100", "border-gray-300", "rounded", "focus:ring-2", "focus:ring-offset-2",
            "disabled:opacity-50", "disabled:cursor-not-allowed", "cursor-pointer"
        };

        var colorClasses = Color switch
        {
            "green" => "text-green-600 focus:ring-green-500",
            "red" => "text-red-600 focus:ring-red-500",
            "gray" => "text-gray-600 focus:ring-gray-500",
            _ => "text-blue-600 focus:ring-blue-500" // blue
        };

        return (baseClasses.Concat(sizeClasses).ToArray(), colorClasses);
    }
}