using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-card")]
public class CardTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Card title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Card description/subtitle
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Image URL for card header
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Image alt text
    /// </summary>
    public string? ImageAlt { get; set; }

    /// <summary>
    /// Card variant (default, bordered, shadow)
    /// </summary>
    public string Variant { get; set; } = "default";

    /// <summary>
    /// Whether card is clickable
    /// </summary>
    public bool Clickable { get; set; } = false;

    /// <summary>
    /// URL for clickable card
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = new CardContext();
        context.Items[typeof(CardContext)] = cardContext;

        var content = await output.GetChildContentAsync();

        var tagName = !string.IsNullOrEmpty(Href) ? "a" : "div";
        output.TagName = tagName;

        if (!string.IsNullOrEmpty(Href))
        {
            output.Attributes.SetAttribute("href", Href);
        }

        var classes = GetCardClasses();
        AddCssClasses(output, classes);

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        if (Clickable && string.IsNullOrEmpty(Href))
        {
            output.Attributes.SetAttribute("role", "button");
            output.Attributes.SetAttribute("tabindex", "0");
        }

        var cardHtml = BuildCardHtml(content, cardContext);
        output.Content.SetHtmlContent(cardHtml);
    }

    private string[] GetCardClasses()
    {
        var baseClasses = new List<string>
        {
            "rounded-lg", "overflow-hidden", "bg-white", "text-neutral-700"
        };

        var variantClasses = Variant switch
        {
            "bordered" => new[] { "border", "border-neutral-200" },
            "shadow" => new[] { "shadow-lg" },
            _ => new[] { "border", "border-neutral-200/60", "shadow-sm" }
        };

        if (Clickable)
        {
            baseClasses.AddRange(new[] { "cursor-pointer", "transition-transform", "hover:scale-[1.02]", "focus:outline-none", "focus:ring-2", "focus:ring-blue-500" });
        }

        return baseClasses.Concat(variantClasses).ToArray();
    }

    private string BuildCardHtml(Microsoft.AspNetCore.Html.IHtmlContent content, CardContext cardContext)
    {
        var sections = new List<string>();

        // Header image
        if (!string.IsNullOrEmpty(ImageUrl))
        {
            sections.Add($@"
                <div class=""aspect-w-16 aspect-h-9"">
                    <img src=""{ImageUrl}"" alt=""{EscapeJavaScriptString(ImageAlt ?? "")}"" class=""object-cover w-full h-48"">
                </div>");
        }

        // Header section
        if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Description) || cardContext.HasHeader)
        {
            sections.Add($@"
                <div class=""px-6 py-4"">
                    {(!string.IsNullOrEmpty(Title) ? $@"<h3 class=""text-lg font-semibold leading-tight"">{EscapeJavaScriptString(Title)}</h3>" : "")}
                    {(!string.IsNullOrEmpty(Description) ? $@"<p class=""mt-1 text-sm text-neutral-500"">{EscapeJavaScriptString(Description)}</p>" : "")}
                    {cardContext.HeaderContent}
                </div>");
        }

        // Body content
        if (!string.IsNullOrEmpty(content.GetContent()) || cardContext.HasBody)
        {
            sections.Add($@"
                <div class=""px-6 py-4"">
                    {content}
                    {cardContext.BodyContent}
                </div>");
        }

        // Footer
        if (cardContext.HasFooter)
        {
            sections.Add($@"
                <div class=""px-6 py-4 bg-gray-50 border-t border-neutral-200"">
                    {cardContext.FooterContent}
                </div>");
        }

        return string.Join("\n", sections);
    }
}

[HtmlTargetElement("noundry-card-header")]
public class CardHeaderTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext?)context.Items[typeof(CardContext)];
        var content = await output.GetChildContentAsync();

        if (cardContext != null)
        {
            cardContext.HasHeader = true;
            cardContext.HeaderContent = content.GetContent();
        }

        output.SuppressOutput();
    }
}

[HtmlTargetElement("noundry-card-body")]
public class CardBodyTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext?)context.Items[typeof(CardContext)];
        var content = await output.GetChildContentAsync();

        if (cardContext != null)
        {
            cardContext.HasBody = true;
            cardContext.BodyContent = content.GetContent();
        }

        output.SuppressOutput();
    }
}

[HtmlTargetElement("noundry-card-footer")]
public class CardFooterTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var cardContext = (CardContext?)context.Items[typeof(CardContext)];
        var content = await output.GetChildContentAsync();

        if (cardContext != null)
        {
            cardContext.HasFooter = true;
            cardContext.FooterContent = content.GetContent();
        }

        output.SuppressOutput();
    }
}

public class CardContext
{
    public bool HasHeader { get; set; }
    public bool HasBody { get; set; }
    public bool HasFooter { get; set; }
    public string HeaderContent { get; set; } = string.Empty;
    public string BodyContent { get; set; } = string.Empty;
    public string FooterContent { get; set; } = string.Empty;
}