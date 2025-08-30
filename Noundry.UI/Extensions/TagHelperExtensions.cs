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
}