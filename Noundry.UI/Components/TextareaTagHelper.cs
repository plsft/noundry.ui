using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-textarea")]
public class TextareaTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Number of visible text rows
    /// </summary>
    public int Rows { get; set; } = 4;

    /// <summary>
    /// Number of visible character columns
    /// </summary>
    public int? Cols { get; set; }

    /// <summary>
    /// Maximum character length
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Whether to auto-resize based on content
    /// </summary>
    public bool AutoResize { get; set; } = false;

    /// <summary>
    /// Minimum height when auto-resizing
    /// </summary>
    public string MinHeight { get; set; } = "min-h-[80px]";

    /// <summary>
    /// Maximum height when auto-resizing
    /// </summary>
    public string MaxHeight { get; set; } = "max-h-[200px]";

    /// <summary>
    /// Label text
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Help text displayed below the textarea
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether to show character count
    /// </summary>
    public bool ShowCharacterCount { get; set; } = false;

    /// <summary>
    /// Textarea size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var textareaId = GetInputId();
        var inputName = GetInputName();
        var inputValue = GetInputValue()?.ToString() ?? string.Empty;
        var labelText = Label ?? GetDisplayName();

        output.TagName = "div";
        AddCssClasses(output, "space-y-1");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var textareaClasses = GetTextareaClasses();
        var hasError = !string.IsNullOrEmpty(ErrorMessage);

        var alpineData = AutoResize ? 
            new AlpineDataBuilder()
                .AddStringProperty("textareaValue", inputValue)
                .AddMethod("resizeTextarea() { $el.style.height = 'auto'; $el.style.height = $el.scrollHeight + 'px'; }")
                .Build() : "";

        var textareaHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <label for=""{textareaId}"" class=""text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"">
                {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </label>
            " : "")}
            
            <div class=""relative"">
                <textarea 
                    id=""{textareaId}""
                    name=""{inputName}""
                    rows=""{Rows}""
                    {(Cols.HasValue ? $@"cols=""{Cols.Value}""" : "")}
                    {(MaxLength.HasValue ? $@"maxlength=""{MaxLength.Value}""" : "")}
                    {(!string.IsNullOrEmpty(Placeholder) ? $@"placeholder=""{EscapeJavaScriptString(Placeholder)}"" " : "")}
                    {(AutoResize ? @"x-model=""textareaValue"" @input=""resizeTextarea()"" x-init=""resizeTextarea()""" : "")}
                    {(AutoResize ? $@"x-data=""{alpineData}""" : "")}
                    class=""{string.Join(" ", textareaClasses)} {(AutoResize ? $"{MinHeight} {MaxHeight} resize-none overflow-hidden" : "")} {(hasError ? "border-red-300 focus:border-red-300 focus:ring-red-500" : "border-neutral-300 focus:border-neutral-300 focus:ring-neutral-400")}""
                    {(Disabled ? "disabled" : "")}
                    {(Required ? "required" : "")}
                    {(AspFor != null ? $@"aria-describedby=""{textareaId}-description"" " : "")}
                >{EscapeJavaScriptString(inputValue)}</textarea>
            </div>

            {(ShowCharacterCount && MaxLength.HasValue ? $@"
            <div class=""flex justify-between text-xs text-gray-500"">
                <span>{(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) ? EscapeJavaScriptString(HelpText) : "")}</span>
                <span>{inputValue.Length}/{MaxLength}</span>
            </div>
            " : "")}

            {(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) && !ShowCharacterCount ? $@"
            <p id=""{textareaId}-description"" class=""text-sm text-gray-500"">
                {EscapeJavaScriptString(HelpText)}
            </p>
            " : "")}

            {(!string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p id=""{textareaId}-description"" class=""text-sm text-red-600"">
                {EscapeJavaScriptString(ErrorMessage)}
            </p>
            " : "")}";

        output.Content.SetHtmlContent(textareaHtml);
    }

    private string[] GetTextareaClasses()
    {
        var baseClasses = new[]
        {
            "flex", "w-full", "px-3", "py-2", "text-sm", "bg-white", "border", "rounded-md",
            "peer", "ring-offset-background", "placeholder:text-neutral-400", "focus:outline-none",
            "focus:ring-2", "focus:ring-offset-2", "disabled:cursor-not-allowed", "disabled:opacity-50",
            "resize-y"
        };

        var sizeClasses = Size switch
        {
            "sm" => new[] { "text-xs", "p-2" },
            "lg" => new[] { "text-base", "p-4" },
            _ => new string[0] // md (uses base classes)
        };

        if (!AutoResize)
        {
            baseClasses = baseClasses.Concat(new[] { MinHeight }).ToArray();
        }

        return baseClasses.Concat(sizeClasses).ToArray();
    }
}