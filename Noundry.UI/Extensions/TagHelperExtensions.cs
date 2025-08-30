using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Noundry.UI.Extensions;

public static class TagHelperExtensions
{
    /// <summary>
    /// Appends a CSS class value to the existing class attribute
    /// </summary>
    public static void AppendInClassValue(this TagHelperAttributeList attributes, string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (attributes.TryGetAttribute("class", out var existingAttribute))
        {
            var existingValue = existingAttribute.Value?.ToString() ?? string.Empty;
            var newValue = string.IsNullOrEmpty(existingValue) 
                ? value 
                : $"{existingValue} {value}";
            attributes.SetAttribute("class", newValue);
        }
        else
        {
            attributes.SetAttribute("class", value);
        }
    }

    /// <summary>
    /// Checks if IHtmlContent is empty or whitespace
    /// </summary>
    public static bool IsEmptyOrWhiteSpace(this IHtmlContent content)
    {
        if (content == null) return true;
        var contentString = content.GetContent();
        return string.IsNullOrWhiteSpace(contentString);
    }

    /// <summary>
    /// Gets the content string from IHtmlContent
    /// </summary>
    public static string GetContent(this IHtmlContent content)
    {
        if (content == null) return string.Empty;
        
        using var writer = new StringWriter();
        content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
        return writer.ToString();
    }
}