using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Extensions;
using System.Text.Encodings.Web;

namespace Noundry.UI.Core;

public abstract class NoundryTagHelperBase : TagHelper
{
    protected const int DefaultOrder = 0;

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override int Order => DefaultOrder;

    protected static string GenerateUniqueId(string prefix = "noundry")
    {
        return $"{prefix}-{Guid.NewGuid():N}";
    }

    protected static void AddCssClass(TagBuilder tagBuilder, string cssClass)
    {
        if (!string.IsNullOrEmpty(cssClass))
        {
            tagBuilder.AddCssClass(cssClass);
        }
    }

    protected static void AddCssClasses(TagBuilder tagBuilder, params string[] cssClasses)
    {
        foreach (var cssClass in cssClasses.Where(c => !string.IsNullOrEmpty(c)))
        {
            tagBuilder.AddCssClass(cssClass);
        }
    }

    protected static void AddCssClasses(TagHelperOutput output, params string[] cssClasses)
    {
        foreach (var cssClass in cssClasses.Where(c => !string.IsNullOrEmpty(c)))
        {
            output.Attributes.AppendInClassValue(cssClass);
        }
    }

    protected static void SetAttribute(TagBuilder tagBuilder, string name, object? value)
    {
        if (value != null)
        {
            tagBuilder.Attributes[name] = value.ToString();
        }
    }

    protected static void SetBooleanAttribute(TagBuilder tagBuilder, string name, bool value)
    {
        if (value)
        {
            tagBuilder.Attributes[name] = name;
        }
    }

    protected static void SetDataAttribute(TagBuilder tagBuilder, string name, object? value)
    {
        if (value != null)
        {
            tagBuilder.Attributes[$"data-{name}"] = value.ToString();
        }
    }

    protected static void SetAlpineAttribute(TagBuilder tagBuilder, string directive, string expression)
    {
        if (!string.IsNullOrEmpty(expression))
        {
            tagBuilder.Attributes[$"x-{directive}"] = expression;
        }
    }

    protected static void SetAlpineAttribute(TagHelperOutput output, string directive, string expression)
    {
        if (!string.IsNullOrEmpty(expression))
        {
            output.Attributes.SetAttribute($"x-{directive}", expression);
        }
    }

    protected static string EscapeJavaScriptString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return JavaScriptEncoder.Default.Encode(input);
    }

    protected static IHtmlContent CreateIcon(string svgContent, string? cssClass = null)
    {
        var iconBuilder = new TagBuilder("svg");
        iconBuilder.Attributes["xmlns"] = "http://www.w3.org/2000/svg";
        iconBuilder.Attributes["fill"] = "none";
        iconBuilder.Attributes["viewBox"] = "0 0 24 24";
        iconBuilder.Attributes["stroke-width"] = "1.5";
        iconBuilder.Attributes["stroke"] = "currentColor";
        
        if (!string.IsNullOrEmpty(cssClass))
        {
            AddCssClass(iconBuilder, cssClass);
        }

        iconBuilder.InnerHtml.SetHtmlContent(svgContent);
        return iconBuilder;
    }
}