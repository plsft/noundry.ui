using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-table")]
public class TableTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Whether the table is striped
    /// </summary>
    public bool Striped { get; set; } = false;

    /// <summary>
    /// Whether to show borders
    /// </summary>
    public bool Bordered { get; set; } = true;

    /// <summary>
    /// Whether to enable hover effects
    /// </summary>
    public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Table size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Whether table is responsive
    /// </summary>
    public bool Responsive { get; set; } = true;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tableContext = new TableContext 
        { 
            Striped = Striped, 
            Hoverable = Hoverable, 
            Size = Size 
        };
        context.Items[typeof(TableContext)] = tableContext;

        var content = await output.GetChildContentAsync();

        if (Responsive)
        {
            output.TagName = "div";
            AddCssClasses(output, "overflow-x-auto");

            if (!string.IsNullOrEmpty(CssClass))
            {
                output.Attributes.AppendInClassValue(CssClass);
            }

            var tableClasses = GetTableClasses();
            var tableHtml = $@"<table class=""{string.Join(" ", tableClasses)}"">{content}</table>";
            output.Content.SetHtmlContent(tableHtml);
        }
        else
        {
            output.TagName = "table";
            var classes = GetTableClasses();
            AddCssClasses(output, classes);

            if (!string.IsNullOrEmpty(CssClass))
            {
                output.Attributes.AppendInClassValue(CssClass);
            }

            output.Content.SetHtmlContent(content);
        }
    }

    private string[] GetTableClasses()
    {
        var baseClasses = new List<string>
        {
            "min-w-full", "divide-y", "divide-neutral-200"
        };

        if (Bordered)
        {
            baseClasses.AddRange(new[] { "border", "border-neutral-200" });
        }

        var sizeClasses = Size switch
        {
            "sm" => new[] { "text-xs" },
            "lg" => new[] { "text-base" },
            _ => new[] { "text-sm" } // md
        };

        return baseClasses.Concat(sizeClasses).ToArray();
    }
}

[HtmlTargetElement("noundry-table-head")]
public class TableHeadTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        output.TagName = "thead";
        AddCssClasses(output, "bg-gray-50");
        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("noundry-table-body")]
public class TableBodyTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tableContext = (TableContext?)context.Items[typeof(TableContext)];
        var content = await output.GetChildContentAsync();

        output.TagName = "tbody";
        
        var classes = new List<string> { "bg-white", "divide-y", "divide-gray-200" };
        
        if (tableContext?.Striped == true)
        {
            classes.Add("divide-y-0");
        }

        AddCssClasses(output, classes.ToArray());
        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("noundry-table-row")]
public class TableRowTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Whether this row is selected/highlighted
    /// </summary>
    public bool Selected { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tableContext = (TableContext?)context.Items[typeof(TableContext)];
        var content = await output.GetChildContentAsync();

        output.TagName = "tr";

        var classes = new List<string>();

        if (tableContext?.Striped == true)
        {
            classes.Add("even:bg-gray-50");
        }

        if (tableContext?.Hoverable == true)
        {
            classes.Add("hover:bg-gray-50");
        }

        if (Selected)
        {
            classes.Add("bg-blue-50");
        }

        if (classes.Any())
        {
            AddCssClasses(output, classes.ToArray());
        }

        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("noundry-table-header")]
public class TableHeaderTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Whether this column is sortable
    /// </summary>
    public bool Sortable { get; set; } = false;

    /// <summary>
    /// Sort direction (asc, desc, none)
    /// </summary>
    public string SortDirection { get; set; } = "none";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tableContext = (TableContext?)context.Items[typeof(TableContext)];
        var content = await output.GetChildContentAsync();

        output.TagName = "th";
        output.Attributes.SetAttribute("scope", "col");

        var sizeClasses = tableContext?.Size switch
        {
            "sm" => new[] { "px-3", "py-2" },
            "lg" => new[] { "px-8", "py-4" },
            _ => new[] { "px-6", "py-3" } // md
        };

        var classes = new List<string>
        {
            "text-left", "font-medium", "text-gray-500", "uppercase", "tracking-wider"
        };

        classes.AddRange(sizeClasses);

        if (Sortable)
        {
            classes.AddRange(new[] { "cursor-pointer", "hover:bg-gray-100", "select-none" });
            
            var sortIcon = SortDirection switch
            {
                "asc" => CreateIcon(Icons.ChevronUp, "w-4 h-4 ml-1"),
                "desc" => CreateIcon(Icons.ChevronDown, "w-4 h-4 ml-1"),
                _ => CreateIcon(Icons.ChevronDown, "w-4 h-4 ml-1 opacity-0 group-hover:opacity-100")
            };

            var sortHtml = $@"
                <div class=""flex items-center group"">
                    {content}
                    {sortIcon}
                </div>";
            output.Content.SetHtmlContent(sortHtml);
            return;
        }

        AddCssClasses(output, classes.ToArray());
        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("noundry-table-cell")]
public class TableCellTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Whether this is a header cell
    /// </summary>
    public bool IsHeader { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tableContext = (TableContext?)context.Items[typeof(TableContext)];
        var content = await output.GetChildContentAsync();

        output.TagName = IsHeader ? "th" : "td";

        var sizeClasses = tableContext?.Size switch
        {
            "sm" => new[] { "px-3", "py-2" },
            "lg" => new[] { "px-8", "py-4" },
            _ => new[] { "px-6", "py-4" } // md
        };

        var baseClasses = IsHeader
            ? new[] { "text-left", "font-medium", "text-gray-900" }
            : new[] { "text-gray-900" };

        AddCssClasses(output, baseClasses.Concat(sizeClasses).ToArray());
        output.Content.SetHtmlContent(content);
    }
}

public class TableContext
{
    public bool Striped { get; set; }
    public bool Hoverable { get; set; }
    public string Size { get; set; } = "md";
}