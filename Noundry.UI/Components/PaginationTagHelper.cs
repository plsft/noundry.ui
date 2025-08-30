using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-pagination")]
public class PaginationTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; } = 1;

    /// <summary>
    /// Number of page links to show around current page
    /// </summary>
    public int PageRange { get; set; } = 2;

    /// <summary>
    /// Whether to show first/last page links
    /// </summary>
    public bool ShowFirstLast { get; set; } = true;

    /// <summary>
    /// Whether to show previous/next links
    /// </summary>
    public bool ShowPrevNext { get; set; } = true;

    /// <summary>
    /// Whether to show page info (e.g., "Page 1 of 10")
    /// </summary>
    public bool ShowInfo { get; set; } = true;

    /// <summary>
    /// Base URL for page links (page number will be appended)
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Query parameter name for page number
    /// </summary>
    public string PageParam { get; set; } = "page";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        output.TagName = "div";

        var classes = new List<string>
        {
            "flex", "items-center", "justify-between", "w-full", "h-16", "px-3", "border-t", "border-neutral-200"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var prevIcon = CreateIcon(Icons.ArrowLeft, "w-4 h-4");
        var nextIcon = CreateIcon(Icons.ArrowRight, "w-4 h-4");

        var paginationHtml = $@"
            {(ShowInfo ? $@"
            <div class=""text-sm text-gray-700"">
                Page <span class=""font-medium"">{CurrentPage}</span> of <span class=""font-medium"">{TotalPages}</span>
            </div>
            " : "")}
            
            <div class=""flex items-center space-x-1"">
                {(ShowFirstLast && CurrentPage > 1 ? $@"
                <a href=""{BuildPageUrl(1)}"" class=""inline-flex items-center px-2 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 hover:text-gray-700"">
                    First
                </a>
                " : "")}
                
                {(ShowPrevNext && CurrentPage > 1 ? $@"
                <a href=""{BuildPageUrl(CurrentPage - 1)}"" class=""inline-flex items-center px-2 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 hover:text-gray-700"">
                    {prevIcon}
                    <span class=""ml-1"">Previous</span>
                </a>
                " : "")}
                
                <div class=""flex items-center space-x-1"">
                    {BuildPageLinks()}
                </div>
                
                {(ShowPrevNext && CurrentPage < TotalPages ? $@"
                <a href=""{BuildPageUrl(CurrentPage + 1)}"" class=""inline-flex items-center px-2 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 hover:text-gray-700"">
                    <span class=""mr-1"">Next</span>
                    {nextIcon}
                </a>
                " : "")}
                
                {(ShowFirstLast && CurrentPage < TotalPages ? $@"
                <a href=""{BuildPageUrl(TotalPages)}"" class=""inline-flex items-center px-2 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 hover:text-gray-700"">
                    Last
                </a>
                " : "")}
            </div>";

        output.Content.SetHtmlContent(paginationHtml);
    }

    private string BuildPageUrl(int pageNumber)
    {
        if (!string.IsNullOrEmpty(BaseUrl))
        {
            var separator = BaseUrl.Contains('?') ? "&" : "?";
            return $"{BaseUrl}{separator}{PageParam}={pageNumber}";
        }
        
        return $"?{PageParam}={pageNumber}";
    }

    private string BuildPageLinks()
    {
        var links = new List<string>();
        
        var startPage = Math.Max(1, CurrentPage - PageRange);
        var endPage = Math.Min(TotalPages, CurrentPage + PageRange);

        // Show ellipsis if we're not showing the first page
        if (startPage > 1)
        {
            if (startPage > 2)
            {
                links.Add(@"<span class=""px-2 py-1 text-sm text-gray-500"">…</span>");
            }
        }

        for (int i = startPage; i <= endPage; i++)
        {
            if (i == CurrentPage)
            {
                links.Add($@"
                    <span class=""inline-flex items-center px-3 py-1 text-sm font-medium text-blue-600 bg-blue-50 border border-blue-300 rounded-md"">
                        {i}
                    </span>");
            }
            else
            {
                links.Add($@"
                    <a href=""{BuildPageUrl(i)}"" class=""inline-flex items-center px-3 py-1 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 hover:text-gray-700"">
                        {i}
                    </a>");
            }
        }

        // Show ellipsis if we're not showing the last page
        if (endPage < TotalPages)
        {
            if (endPage < TotalPages - 1)
            {
                links.Add(@"<span class=""px-2 py-1 text-sm text-gray-500"">…</span>");
            }
        }

        return string.Join("\n", links);
    }
}