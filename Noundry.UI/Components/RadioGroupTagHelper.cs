using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-radio-group")]
public class RadioGroupTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Radio group label
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Help text displayed below the group
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Layout direction (vertical, horizontal)
    /// </summary>
    public string Direction { get; set; } = "vertical";

    /// <summary>
    /// Radio button size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Color theme (blue, green, red, gray)
    /// </summary>
    public string Color { get; set; } = "blue";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var radioContext = new RadioGroupContext 
        { 
            GroupName = GetInputName(),
            Size = Size,
            Color = Color,
            SelectedValue = GetInputValue()?.ToString() ?? string.Empty
        };
        context.Items[typeof(RadioGroupContext)] = radioContext;

        var content = await output.GetChildContentAsync();

        var groupId = GetInputId();
        var labelText = Label ?? GetDisplayName();

        output.TagName = "fieldset";
        AddCssClasses(output, "space-y-3");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        if (Disabled)
        {
            output.Attributes.SetAttribute("disabled", "disabled");
        }

        var hasError = !string.IsNullOrEmpty(ErrorMessage);
        var directionClasses = Direction == "horizontal" ? "flex flex-wrap gap-6" : "space-y-3";

        var radioGroupHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <legend class=""text-sm font-medium leading-none text-gray-900"">
                {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </legend>
            " : "")}
            
            <div class=""{directionClasses}"" role=""radiogroup"" {(Required ? @"aria-required=""true""" : "")}>
                {content}
            </div>

            {(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-gray-500"">
                {EscapeJavaScriptString(HelpText)}
            </p>
            " : "")}

            {(!string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-red-600"">
                {EscapeJavaScriptString(ErrorMessage)}
            </p>
            " : "")}";

        output.Content.SetHtmlContent(radioGroupHtml);
    }
}

[HtmlTargetElement("noundry-radio")]
public class RadioTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Radio button value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Label text for this radio option
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Description text for this option
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether this option is checked
    /// </summary>
    public bool Checked { get; set; } = false;

    /// <summary>
    /// Whether this option is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var radioContext = (RadioGroupContext?)context.Items[typeof(RadioGroupContext)];
        var content = await output.GetChildContentAsync();

        var radioId = GenerateUniqueId("radio");
        var isChecked = Checked || (radioContext?.SelectedValue == Value);
        var labelText = !content.IsEmptyOrWhiteSpace ? content.GetContent() : Label;

        output.TagName = "div";
        AddCssClasses(output, "flex", "items-start", "space-x-3");

        var (radioClasses, colorClasses) = GetRadioClasses(radioContext);

        var radioHtml = $@"
            <div class=""flex items-center"">
                <input 
                    type=""radio""
                    id=""{radioId}""
                    name=""{radioContext?.GroupName}""
                    value=""{Value}""
                    {(isChecked ? "checked" : "")}
                    {(Disabled ? "disabled" : "")}
                    class=""{string.Join(" ", radioClasses)} {colorClasses}""
                />
            </div>
            
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <div class=""flex-1"">
                <label for=""{radioId}"" class=""text-sm font-medium text-gray-700 cursor-pointer {(Disabled ? "opacity-50 cursor-not-allowed" : "")}"">
                    {EscapeJavaScriptString(labelText)}
                </label>
                {(!string.IsNullOrEmpty(Description) ? $@"
                <p class=""mt-1 text-xs text-gray-500"">{EscapeJavaScriptString(Description)}</p>
                " : "")}
            </div>
            " : "")}";

        output.Content.SetHtmlContent(radioHtml);
    }

    private (string[] radioClasses, string colorClasses) GetRadioClasses(RadioGroupContext? context)
    {
        var size = context?.Size ?? "md";
        var color = context?.Color ?? "blue";

        var sizeClasses = size switch
        {
            "sm" => new[] { "w-3", "h-3" },
            "lg" => new[] { "w-5", "h-5" },
            _ => new[] { "w-4", "h-4" } // md
        };

        var baseClasses = new[]
        {
            "bg-gray-100", "border-gray-300", "focus:ring-2", "focus:ring-offset-2",
            "disabled:opacity-50", "disabled:cursor-not-allowed", "cursor-pointer"
        };

        var colorClasses = color switch
        {
            "green" => "text-green-600 focus:ring-green-500",
            "red" => "text-red-600 focus:ring-red-500",
            "gray" => "text-gray-600 focus:ring-gray-500",
            _ => "text-blue-600 focus:ring-blue-500" // blue
        };

        return (baseClasses.Concat(sizeClasses).ToArray(), colorClasses);
    }
}

public class RadioGroupContext
{
    public string GroupName { get; set; } = string.Empty;
    public string Size { get; set; } = "md";
    public string Color { get; set; } = "blue";
    public string SelectedValue { get; set; } = string.Empty;
}