using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-tabs")]
public class TabsTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Tab style (default, pills)
    /// </summary>
    public string Style { get; set; } = "default";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Maximum width for tabs container
    /// </summary>
    public string MaxWidth { get; set; } = "max-w-sm";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tabsContext = new TabsContext { Style = Style };
        context.Items[typeof(TabsContext)] = tabsContext;

        var content = await output.GetChildContentAsync();
        var tabId = GenerateUniqueId("tabs");

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddProperty("tabSelected", 1)
            .AddStringProperty("tabId", tabId)
            .AddMethod("tabButtonClicked(tabButton) { this.tabSelected = tabButton.id.replace(this.tabId + '-', ''); this.tabRepositionMarker(tabButton); }")
            .AddMethod("tabRepositionMarker(tabButton) { this.$refs.tabMarker.style.width = tabButton.offsetWidth + 'px'; this.$refs.tabMarker.style.height = tabButton.offsetHeight + 'px'; this.$refs.tabMarker.style.left = tabButton.offsetLeft + 'px'; }")
            .AddMethod("tabContentActive(tabContent) { return this.tabSelected == tabContent.id.replace(this.tabId + '-content-', ''); }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "init", "tabRepositionMarker($refs.tabButtons.firstElementChild);");

        var classes = new List<string> { "relative", "w-full" };
        
        if (!string.IsNullOrEmpty(MaxWidth))
        {
            classes.Add(MaxWidth);
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        // Generate tab buttons and content sections
        var tabButtons = tabsContext.TabItems.Select((tab, index) => 
            $"""<button :id="$id(tabId)" @click="tabButtonClicked($el);" type="button" class="relative z-20 inline-flex items-center justify-center w-full h-8 px-3 text-sm font-medium transition-all rounded-md cursor-pointer whitespace-nowrap">{EscapeJavaScriptString(tab.Title)}</button>"""
        ).ToArray();

        var tabContents = tabsContext.TabItems.Select((tab, index) => 
            $"""
            <div :id="$id(tabId + '-content')" x-show="tabContentActive($el)" class="relative" {(index > 0 ? "x-cloak" : "")}>
                {tab.Content}
            </div>
            """
        ).ToArray();

        var gridCols = $"grid-cols-{tabsContext.TabItems.Count}";

        var tabsHtml = $"""
            <div x-ref="tabButtons" class="relative inline-grid items-center justify-center w-full h-10 {gridCols} p-1 text-gray-500 bg-gray-100 rounded-lg select-none">
                {string.Join("\n", tabButtons)}
                <div x-ref="tabMarker" class="absolute left-0 z-10 w-1/{tabsContext.TabItems.Count} h-full duration-300 ease-out" x-cloak>
                    <div class="w-full h-full bg-white rounded-md shadow-sm"></div>
                </div>
            </div>
            <div class="relative w-full mt-2 content">
                {string.Join("\n", tabContents)}
            </div>
            """;

        output.Content.SetHtmlContent(tabsHtml);
    }
}

[HtmlTargetElement("noundry-tab-item")]
public class TabItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Tab title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Whether this tab is active by default
    /// </summary>
    public bool Active { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var tabsContext = (TabsContext?)context.Items[typeof(TabsContext)];
        var content = await output.GetChildContentAsync();

        if (tabsContext != null)
        {
            tabsContext.TabItems.Add(new TabItem 
            { 
                Title = Title, 
                Content = content.GetContent(),
                IsActive = Active 
            });
        }

        output.SuppressOutput();
    }
}

public class TabsContext
{
    public string Style { get; set; } = "default";
    public List<TabItem> TabItems { get; set; } = new();
}

public class TabItem
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
}