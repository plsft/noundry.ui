using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Noundry.UI.Core;

public abstract class NoundryFormTagHelperBase : NoundryTagHelperBase
{
    /// <summary>
    /// The model property to bind to
    /// </summary>
    public ModelExpression? AspFor { get; set; }

    /// <summary>
    /// Additional CSS classes to apply
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Whether the control is disabled
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Whether the control is required
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Placeholder text
    /// </summary>
    public string? Placeholder { get; set; }

    protected string GetInputName()
    {
        return AspFor?.Name ?? string.Empty;
    }

    protected string GetInputId()
    {
        return AspFor?.Name.Replace("[", "_").Replace("]", "_").Replace(".", "_") 
            ?? GenerateUniqueId("input");
    }

    protected object? GetInputValue()
    {
        return AspFor?.Model;
    }

    protected string GetDisplayName()
    {
        return AspFor?.Metadata.DisplayName ?? AspFor?.Name ?? string.Empty;
    }

    protected void ApplyValidationAttributes(TagBuilder tagBuilder)
    {
        if (AspFor?.Metadata.IsRequired == true || Required)
        {
            SetBooleanAttribute(tagBuilder, "required", true);
            SetAttribute(tagBuilder, "aria-required", "true");
        }

        if (Disabled)
        {
            SetBooleanAttribute(tagBuilder, "disabled", true);
            SetAttribute(tagBuilder, "aria-disabled", "true");
        }
    }
}