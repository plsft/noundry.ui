using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-breadcrumbs")]
public class BreadcrumbsTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Separator character or HTML
    /// </summary>
    public string Separator { get; set; } = "/";

    /// <summary>
    /// Whether to show home icon
    /// </summary>
    public bool ShowHome { get; set; } = true;

    /// <summary>
    /// Home URL
    /// </summary>
    public string HomeUrl { get; set; } = "/";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var breadcrumbContext = new BreadcrumbContext();
        context.Items[typeof(BreadcrumbContext)] = breadcrumbContext;

        var content = await output.GetChildContentAsync();

        output.TagName = "nav";
        output.Attributes.SetAttribute("aria-label", "breadcrumb");

        var classes = new List<string>
        {
            "flex", "justify-between", "px-3.5", "py-1", "border", "border-neutral-200/60", "rounded-md"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var homeIcon = CreateIcon("""<path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12l8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" />""", "w-4 h-4");

        var breadcrumbItems = new List<string>();

        if (ShowHome)
        {
            breadcrumbItems.Add($@"
                <li class=""flex items-center"">
                    <a href=""{HomeUrl}"" class=""flex items-center text-sm text-neutral-500 hover:text-neutral-700"">
                        {homeIcon}
                        <span class=""sr-only"">Home</span>
                    </a>
                </li>");
        }

        foreach (var item in breadcrumbContext.Items)
        {
            if (breadcrumbItems.Count > 0)
            {
                breadcrumbItems.Add($@"
                    <li class=""flex items-center"">
                        <span class=""mx-2 text-neutral-400"">{Separator}</span>
                    </li>");
            }

            if (!string.IsNullOrEmpty(item.Href))
            {
                breadcrumbItems.Add($@"
                    <li class=""flex items-center"">
                        <a href=""{item.Href}"" class=""text-sm text-neutral-500 hover:text-neutral-700"">{EscapeJavaScriptString(item.Text)}</a>
                    </li>");
            }
            else
            {
                breadcrumbItems.Add($@"
                    <li class=""flex items-center"">
                        <span class=""text-sm text-neutral-900"" aria-current=""page"">{EscapeJavaScriptString(item.Text)}</span>
                    </li>");
            }
        }

        var breadcrumbHtml = $@"
            <ol class=""flex items-center space-x-1"">
                {string.Join("\n", breadcrumbItems)}
            </ol>";

        output.Content.SetHtmlContent(breadcrumbHtml);
    }
}

[HtmlTargetElement("noundry-breadcrumb-item")]
public class BreadcrumbItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Item text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Item URL (optional, makes it a link)
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Whether this is the current/active item
    /// </summary>
    public bool Current { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var breadcrumbContext = (BreadcrumbContext?)context.Items[typeof(BreadcrumbContext)];
        var content = await output.GetChildContentAsync();

        var itemText = !content.IsEmptyOrWhiteSpace ? content.GetContent() : Text;

        if (breadcrumbContext != null)
        {
            breadcrumbContext.Items.Add(new BreadcrumbItem 
            { 
                Text = itemText,
                Href = Current ? null : Href
            });
        }

        output.SuppressOutput();
    }
}

public class BreadcrumbContext
{
    public List<BreadcrumbItem> Items { get; set; } = new();
}

public class BreadcrumbItem
{
    public string Text { get; set; } = string.Empty;
    public string? Href { get; set; }
}