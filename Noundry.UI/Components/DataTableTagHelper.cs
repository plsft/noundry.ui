using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Noundry.UI.Core;
using Noundry.UI.Extensions;
using System.Collections;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-data-table")]
public class DataTableTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Table title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// API URL for data loading
    /// </summary>
    public string? ApiUrl { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PerPage { get; set; } = 10;

    /// <summary>
    /// Whether to use server-side pagination
    /// </summary>
    public bool ServerPagination { get; set; } = false;

    /// <summary>
    /// Whether to show search input
    /// </summary>
    public bool ShowSearch { get; set; } = true;

    /// <summary>
    /// Search placeholder text
    /// </summary>
    public string SearchPlaceholder { get; set; } = "Search...";

    /// <summary>
    /// Whether to show pagination controls
    /// </summary>
    public bool ShowPagination { get; set; } = true;

    /// <summary>
    /// Whether to show per-page selector
    /// </summary>
    public bool ShowPerPageSelector { get; set; } = true;

    /// <summary>
    /// Available per-page options (comma-separated)
    /// </summary>
    public string PerPageOptions { get; set; } = "5,10,25,50";

    /// <summary>
    /// No results message
    /// </summary>
    public string NoResultsMessage { get; set; } = "No results found";

    /// <summary>
    /// Loading message
    /// </summary>
    public string LoadingMessage { get; set; } = "Loading...";

    /// <summary>
    /// Error message prefix
    /// </summary>
    public string ErrorMessage { get; set; } = "Error loading data";

    /// <summary>
    /// Whether rows are hoverable
    /// </summary>
    public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Whether table is striped
    /// </summary>
    public bool Striped { get; set; } = false;

    /// <summary>
    /// Table size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Model expression for collection binding
    /// </summary>
    public ModelExpression? AspFor { get; set; }

    /// <summary>
    /// Template for rendering each item in the collection
    /// </summary>
    public string? ItemTemplate { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dataTableContext = new DataTableContext 
        { 
            ApiUrl = ApiUrl ?? "",
            PerPage = PerPage,
            ServerPagination = ServerPagination,
            ShowSearch = ShowSearch,
            ShowPagination = ShowPagination,
            Hoverable = Hoverable,
            Striped = Striped,
            Size = Size
        };
        context.Items[typeof(DataTableContext)] = dataTableContext;

        // Process child content to populate columns and static data
        var content = await output.GetChildContentAsync();

        // Handle model binding for collections
        if (AspFor != null && AspFor.Model is IEnumerable collection && collection.GetType() != typeof(string))
        {
            ProcessCollectionData(dataTableContext, collection);
        }

        output.TagName = "div";
        
        var alpineData = BuildDataTableData(dataTableContext);
        SetAlpineAttribute(output, "data", alpineData);
        
        if (!string.IsNullOrEmpty(ApiUrl))
        {
            SetAlpineAttribute(output, "init", "fetchData()");
        }
        else if (dataTableContext.StaticData.Any() || (AspFor?.Model is IEnumerable))
        {
            SetAlpineAttribute(output, "init", "initStaticData()");
        }

        var classes = new List<string>
        {
            "bg-white", "rounded-lg", "shadow-sm", "border", "border-gray-200"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var searchIcon = CreateIcon(Icons.Search, "absolute left-3 top-2.5 h-5 w-5 text-gray-400");
        var sortUpIcon = CreateIcon("""<path fill="currentColor" d="M7 14l5-5 5 5z"/>""", "w-3 h-3 text-gray-400");
        var sortDownIcon = CreateIcon("""<path fill="currentColor" d="M7 10l5 5 5-5z"/>""", "w-3 h-3 text-gray-400");
        var loadingSpinner = CreateLoadingSpinner();
        var paginationControls = BuildPaginationControls();

        var dataTableHtml = $@"
            {(ShowSearch || !string.IsNullOrEmpty(Title) ? $@"
            <!-- Table Header with Search -->
            <div class=""p-4 border-b border-gray-200"">
                <div class=""flex flex-col md:flex-row md:items-center md:justify-between gap-4"">
                    {(!string.IsNullOrEmpty(Title) ? $@"<h2 class=""text-lg font-semibold text-gray-800"">{EscapeJavaScriptString(Title)}</h2>" : "")}
                    {(ShowSearch ? $@"
                    <div class=""relative"">
                        <input
                            type=""text""
                            x-model=""searchTerm""
                            @input=""handleSearch""
                            placeholder=""{EscapeJavaScriptString(SearchPlaceholder)}""
                            class=""w-full md:w-64 pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500""
                        >
                        {searchIcon}
                    </div>
                    " : "")}
                </div>
            </div>
            " : "")}
            
            <!-- Table -->
            <div class=""overflow-x-auto"">
                <table class=""min-w-full divide-y divide-gray-200"">
                    <thead class=""bg-gray-50"">
                        <tr>
                            <template x-for=""column in columns"" :key=""column.key"">
                                <th 
                                    @click=""column.sortable ? sort(column.key) : null""
                                    :class=""{{
                                        'cursor-pointer hover:bg-gray-100': column.sortable,
                                        'bg-gray-100': sortColumn === column.key
                                    }}""
                                    class=""px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider""
                                >
                                    <div class=""flex items-center space-x-1"">
                                        <span x-text=""column.label""></span>
                                        <template x-if=""column.sortable"">
                                            <span class=""inline-flex flex-col"">
                                                <svg 
                                                    :class=""{{ 'text-primary-600': sortColumn === column.key && sortDirection === 'asc' }}""
                                                    class=""w-3 h-3 text-gray-400"" 
                                                    viewBox=""0 0 24 24""
                                                >
                                                    <path fill=""currentColor"" d=""M7 14l5-5 5 5z""/>
                                                </svg>
                                                <svg 
                                                    :class=""{{ 'text-primary-600': sortColumn === column.key && sortDirection === 'desc' }}""
                                                    class=""w-3 h-3 text-gray-400"" 
                                                    viewBox=""0 0 24 24""
                                                >
                                                    <path fill=""currentColor"" d=""M7 10l5 5 5-5z""/>
                                                </svg>
                                            </span>
                                        </template>
                                    </div>
                                </th>
                            </template>
                        </tr>
                    </thead>
                    <tbody class=""bg-white divide-y divide-gray-200"">
                        <template x-if=""loading"">
                            <tr>
                                <td :colspan=""columns.length"" class=""px-6 py-12 text-center"">
                                    <div class=""flex justify-center"">
                                        {loadingSpinner}
                                    </div>
                                </td>
                            </tr>
                        </template>
                        <template x-if=""!loading && error"">
                            <tr>
                                <td :colspan=""columns.length"" class=""px-6 py-8 text-center text-red-500"">
                                    <span x-text=""error""></span>
                                </td>
                            </tr>
                        </template>
                        <template x-if=""!loading && !error && filteredData.length === 0"">
                            <tr>
                                <td :colspan=""columns.length"" class=""px-6 py-8 text-center text-gray-500"">
                                    {EscapeJavaScriptString(NoResultsMessage)}
                                </td>
                            </tr>
                        </template>
                        <template x-for=""(item, index) in paginatedData"" :key=""index"">
                            <tr class=""{(Hoverable ? "hover:bg-gray-50" : "")} {(Striped ? "even:bg-gray-50" : "")}"">
                                <template x-for=""column in columns"" :key=""column.key"">
                                    <td class=""px-6 py-4 whitespace-nowrap text-sm text-gray-800"">
                                        <template x-if=""column.href"">
                                            <a 
                                                :href=""formatHref(column.href, item)"" 
                                                class=""text-primary-600 hover:text-primary-800 hover:underline""
                                                x-text=""column.hrefText ? formatHref(column.hrefText, item) : getNestedValue(item, column.key)""
                                            ></a>
                                        </template>
                                        <template x-if=""!column.href"">
                                            <span x-text=""getNestedValue(item, column.key)""></span>
                                        </template>
                                    </td>
                                </template>
                            </tr>
                        </template>
                    </tbody>
                </table>
            </div>
            
            {(ShowPagination ? $@"
            <!-- Pagination -->
            <div class=""px-4 py-3 border-t border-gray-200 sm:px-6"">
                <div class=""flex flex-col md:flex-row md:items-center md:justify-between"">
                    <div class=""flex items-center text-sm text-gray-500"">
                        <span>
                            Showing <span class=""font-medium"" x-text=""(currentPage - 1) * perPage + 1""></span> to 
                            <span class=""font-medium"" x-text=""Math.min(currentPage * perPage, filteredData.length)""></span> of 
                            <span class=""font-medium"" x-text=""filteredData.length""></span> results
                        </span>
                    </div>
                    <div class=""mt-4 md:mt-0 flex items-center justify-between"">
                        {(ShowPerPageSelector ? $@"
                        <div class=""flex items-center"">
                            <label for=""perPage"" class=""mr-2 text-sm text-gray-500"">Per page:</label>
                            <select 
                                id=""perPage"" 
                                x-model.number=""perPage"" 
                                @change=""currentPage = 1; serverPagination ? fetchData() : null""
                                class=""border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500""
                            >
                                {BuildPerPageOptions()}
                            </select>
                        </div>
                        " : "")}
                        {paginationControls}
                    </div>
                </div>
            </div>
            " : "")}";

        output.Content.SetHtmlContent(dataTableHtml);
    }

    private void ProcessCollectionData(DataTableContext context, IEnumerable collection)
    {
        foreach (var item in collection)
        {
            if (item == null) continue;

            var row = new DataTableRow();
            var itemType = item.GetType();

            // Use reflection to get property values for each column
            foreach (var column in context.Columns)
            {
                var value = GetPropertyValue(item, column.Key);
                row.Data[column.Key] = value?.ToString() ?? "";
            }

            context.StaticData.Add(row);
        }
    }

    private object? GetPropertyValue(object obj, string propertyPath)
    {
        if (obj == null) return null;

        var properties = propertyPath.Split('.');
        var currentObj = obj;

        foreach (var propertyName in properties)
        {
            if (currentObj == null) return null;

            var property = currentObj.GetType().GetProperty(propertyName);
            if (property == null) return null;

            currentObj = property.GetValue(currentObj);
        }

        return currentObj;
    }

    private string BuildDataTableData(DataTableContext context)
    {
        // Build columns array from context
        var columnsJs = string.Join(",", context.Columns.Select(col => 
            $"{{key: '{EscapeJavaScriptString(col.Key)}', label: '{EscapeJavaScriptString(col.Label)}', sortable: {col.Sortable.ToString().ToLower()}, href: {(string.IsNullOrEmpty(col.Href) ? "null" : $"'{EscapeJavaScriptString(col.Href)}'")}, hrefText: {(string.IsNullOrEmpty(col.HrefText) ? "null" : $"'{EscapeJavaScriptString(col.HrefText)}'")}, width: {(string.IsNullOrEmpty(col.Width) ? "null" : $"'{EscapeJavaScriptString(col.Width)}'")}, align: '{EscapeJavaScriptString(col.Align)}', hidden: {col.Hidden.ToString().ToLower()}}}"));

        // Build static data if available
        var staticDataJs = "";
        if (context.StaticData.Any())
        {
            var dataItems = context.StaticData.Select(row => 
            {
                var properties = row.Data.Select(kvp => $"\"{kvp.Key}\": \"{EscapeJavaScriptString(kvp.Value)}\"");
                return $"{{{string.Join(", ", properties)}}}";
            });
            staticDataJs = $"[{string.Join(", ", dataItems)}]";
        }

        var builder = new AlpineDataBuilder()
            .AddStringProperty("apiUrl", context.ApiUrl)
            .AddProperty("columns", $"[{columnsJs}]")
            .AddProperty("perPage", PerPage)
            .AddProperty("data", staticDataJs.Length > 0 ? staticDataJs : "[]")
            .AddProperty("filteredData", new object[0])
            .AddBooleanProperty("loading", !string.IsNullOrEmpty(ApiUrl) && context.StaticData.Count == 0)
            .AddProperty("error", "null")
            .AddProperty("currentPage", 1)
            .AddStringProperty("searchTerm", "")
            .AddProperty("sortColumn", "null")
            .AddStringProperty("sortDirection", "asc")
            .AddProperty("totalRecords", 0)
            .AddBooleanProperty("serverPagination", ServerPagination);

        // Computed properties
        builder.AddMethod("get totalPages() { return this.serverPagination ? Math.ceil(this.totalRecords / this.perPage) : Math.ceil(this.filteredData.length / this.perPage); }");
        
        builder.AddMethod(@"get pageNumbers() { 
            const pages = []; 
            const maxVisiblePages = 5; 
            if (this.totalPages <= maxVisiblePages) { 
                for (let i = 1; i <= this.totalPages; i++) { 
                    pages.push(i); 
                } 
            } else { 
                pages.push(1); 
                let start = Math.max(2, this.currentPage - 1); 
                let end = Math.min(this.totalPages - 1, this.currentPage + 1); 
                if (this.currentPage <= 2) { 
                    end = 4; 
                } else if (this.currentPage >= this.totalPages - 1) { 
                    start = this.totalPages - 3; 
                } 
                if (start > 2) { 
                    pages.push('...'); 
                } 
                for (let i = start; i <= end; i++) { 
                    pages.push(i); 
                } 
                if (end < this.totalPages - 1) { 
                    pages.push('...'); 
                } 
                pages.push(this.totalPages); 
            } 
            return pages; 
        }");

        builder.AddMethod("get paginatedData() { if (this.serverPagination) { return this.data; } else { const start = (this.currentPage - 1) * this.perPage; const end = start + this.perPage; return this.filteredData.slice(start, end); } }");

        // API and data methods
        builder.AddMethod($@"async fetchData() {{ 
            this.loading = true; 
            this.error = null; 
            try {{ 
                const url = new URL(this.apiUrl, window.location.origin); 
                url.searchParams.set('_page', this.currentPage.toString()); 
                url.searchParams.set('_limit', this.perPage.toString()); 
                if (this.sortColumn) {{ 
                    url.searchParams.set('_sort', this.sortColumn); 
                    url.searchParams.set('_order', this.sortDirection); 
                }} 
                if (this.searchTerm.trim() !== '') {{ 
                    url.searchParams.set('q', this.searchTerm.trim()); 
                }} 
                const response = await fetch(url.toString()); 
                if (!response.ok) {{ 
                    throw new Error(`HTTP error! Status: ${{response.status}}`); 
                }} 
                const totalCount = response.headers.get('X-Total-Count'); 
                if (totalCount) {{ 
                    this.totalRecords = parseInt(totalCount, 10); 
                    this.serverPagination = true; 
                }} 
                const data = await response.json(); 
                this.data = Array.isArray(data) ? data : []; 
                if (!this.serverPagination) {{ 
                    this.applyFiltersAndSort(); 
                }} else {{ 
                    this.filteredData = this.data; 
                }} 
            }} catch (error) {{ 
                console.error('Error fetching data:', error); 
                this.error = error.message; 
                this.data = []; 
                this.filteredData = []; 
            }} finally {{ 
                this.loading = false; 
            }} 
        }}");

        builder.AddMethod("handleSearch() { this.currentPage = 1; if (this.serverPagination) { this.fetchData(); } else { this.applyFiltersAndSort(); } }");

        builder.AddMethod(@"sort(column) { 
            if (this.sortColumn === column) { 
                this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc'; 
            } else { 
                this.sortColumn = column; 
                this.sortDirection = 'asc'; 
            } 
            if (this.serverPagination) { 
                this.fetchData(); 
            } else { 
                this.applyFiltersAndSort(); 
            } 
        }");

        builder.AddMethod(@"applyFiltersAndSort() { 
            let filtered = this.data; 
            if (this.searchTerm.trim() !== '') { 
                const term = this.searchTerm.toLowerCase(); 
                filtered = filtered.filter(item => { 
                    return this.columns.some(column => { 
                        const value = this.getNestedValue(item, column.key); 
                        return value && String(value).toLowerCase().includes(term); 
                    }); 
                }); 
            } 
            if (this.sortColumn) { 
                filtered = [...filtered].sort((a, b) => { 
                    const aValue = this.getNestedValue(a, this.sortColumn); 
                    const bValue = this.getNestedValue(b, this.sortColumn); 
                    if (typeof aValue === 'string' && typeof bValue === 'string') { 
                        return this.sortDirection === 'asc' ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue); 
                    } 
                    return this.sortDirection === 'asc' ? aValue - bValue : bValue - aValue; 
                }); 
            } 
            this.filteredData = filtered; 
        }");

        // Static data initialization
        builder.AddMethod("initStaticData() { this.filteredData = [...this.data]; this.applyFiltersAndSort(); this.loading = false; }");

        // Utility methods
        builder.AddMethod("getNestedValue(obj, path) { return path.split('.').reduce((prev, curr) => { return prev && prev[curr] !== undefined ? prev[curr] : null; }, obj); }");
        builder.AddMethod("formatHref(template, item) { return template.replace(/{([^}]+)}/g, (match, key) => { return this.getNestedValue(item, key) || ''; }); }");

        return builder.Build();
    }

    private string CreateLoadingSpinner()
    {
        return @"<svg class=""animate-spin h-8 w-8 text-primary-500"" xmlns=""http://www.w3.org/2000/svg"" fill=""none"" viewBox=""0 0 24 24"">
                    <circle class=""opacity-25"" cx=""12"" cy=""12"" r=""10"" stroke=""currentColor"" stroke-width=""4""></circle>
                    <path class=""opacity-75"" fill=""currentColor"" d=""M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z""></path>
                </svg>";
    }

    private string BuildPerPageOptions()
    {
        var options = PerPageOptions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return string.Join("\n", options.Select(option => 
            $@"<option value=""{option.Trim()}"">{option.Trim()}</option>"));
    }

    private string BuildPaginationControls()
    {
        var firstIcon = CreateIcon("""<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 19l-7-7 7-7m8 14l-7-7 7-7" />""", "h-4 w-4");
        var prevIcon = CreateIcon(Icons.ArrowLeft, "h-4 w-4");
        var nextIcon = CreateIcon(Icons.ArrowRight, "h-4 w-4");
        var lastIcon = CreateIcon("""<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 5l7 7-7 7M5 5l7 7-7 7" />""", "h-4 w-4");

        return $@"
            <div class=""ml-4 flex items-center space-x-1"">
                <button 
                    @click=""currentPage = 1; serverPagination ? fetchData() : null"" 
                    :disabled=""currentPage === 1""
                    :class=""{{ 'opacity-50 cursor-not-allowed': currentPage === 1 }}""
                    class=""px-2 py-1 text-sm rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-50""
                >
                    <span class=""sr-only"">First</span>
                    {firstIcon}
                </button>
                <button 
                    @click=""currentPage--; serverPagination ? fetchData() : null"" 
                    :disabled=""currentPage === 1""
                    :class=""{{ 'opacity-50 cursor-not-allowed': currentPage === 1 }}""
                    class=""px-2 py-1 text-sm rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-50""
                >
                    <span class=""sr-only"">Previous</span>
                    {prevIcon}
                </button>
                
                <template x-for=""page in pageNumbers"" :key=""page"">
                    <button 
                        @click=""typeof page === 'number' ? (currentPage = page, serverPagination ? fetchData() : null) : null"" 
                        :class=""{{ 'bg-primary-500 text-white border-primary-500': currentPage === page, 'cursor-default': page === '...' }}""
                        :disabled=""page === '...'""
                        class=""px-3 py-1 text-sm rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-50""
                    >
                        <span x-text=""page""></span>
                    </button>
                </template>
                
                <button 
                    @click=""currentPage++; serverPagination ? fetchData() : null"" 
                    :disabled=""currentPage === totalPages""
                    :class=""{{ 'opacity-50 cursor-not-allowed': currentPage === totalPages }}""
                    class=""px-2 py-1 text-sm rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-50""
                >
                    <span class=""sr-only"">Next</span>
                    {nextIcon}
                </button>
                <button 
                    @click=""currentPage = totalPages; serverPagination ? fetchData() : null"" 
                    :disabled=""currentPage === totalPages""
                    :class=""{{ 'opacity-50 cursor-not-allowed': currentPage === totalPages }}""
                    class=""px-2 py-1 text-sm rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-50""
                >
                    <span class=""sr-only"">Last</span>
                    {lastIcon}
                </button>
            </div>";
    }
}

[HtmlTargetElement("noundry-data-table-column")]
public class DataTableColumnTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Column key/property name
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Column display label
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Whether this column is sortable
    /// </summary>
    public bool Sortable { get; set; } = false;

    /// <summary>
    /// URL template for links (use {property} placeholders)
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Text template for link display (use {property} placeholders)
    /// </summary>
    public string? HrefText { get; set; }

    /// <summary>
    /// Column width class
    /// </summary>
    public string? Width { get; set; }

    /// <summary>
    /// Text alignment (left, center, right)
    /// </summary>
    public string Align { get; set; } = "left";

    /// <summary>
    /// Whether this column is initially hidden
    /// </summary>
    public bool Hidden { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dataTableContext = (DataTableContext?)context.Items[typeof(DataTableContext)];
        await output.GetChildContentAsync();

        if (dataTableContext != null)
        {
            dataTableContext.Columns.Add(new DataTableColumn
            {
                Key = Key,
                Label = Label,
                Sortable = Sortable,
                Href = Href,
                HrefText = HrefText,
                Width = Width,
                Align = Align,
                Hidden = Hidden
            });
        }

        output.SuppressOutput();
    }
}

[HtmlTargetElement("noundry-data-table-row")]
public class DataTableRowTagHelper : NoundryTagHelperBase
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dataTableContext = (DataTableContext?)context.Items[typeof(DataTableContext)];
        var rowContext = new DataTableRowContext();
        context.Items[typeof(DataTableRowContext)] = rowContext;

        await output.GetChildContentAsync();

        if (dataTableContext != null)
        {
            dataTableContext.StaticData.Add(new DataTableRow
            {
                Data = rowContext.CellData
            });
        }

        output.SuppressOutput();
    }
}

[HtmlTargetElement("noundry-data-table-cell")]
public class DataTableCellTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Column key this cell belongs to
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var rowContext = (DataTableRowContext?)context.Items[typeof(DataTableRowContext)];
        var content = await output.GetChildContentAsync();

        if (rowContext != null && !string.IsNullOrEmpty(Key))
        {
            rowContext.CellData[Key] = content.GetContent();
        }

        output.SuppressOutput();
    }
}

public class DataTableRowContext
{
    public Dictionary<string, string> CellData { get; set; } = new();
}

public class DataTableRow
{
    public Dictionary<string, string> Data { get; set; } = new();
}

public class DataTableContext
{
    public string ApiUrl { get; set; } = string.Empty;
    public int PerPage { get; set; } = 10;
    public bool ServerPagination { get; set; }
    public bool ShowSearch { get; set; }
    public bool ShowPagination { get; set; }
    public bool Hoverable { get; set; }
    public bool Striped { get; set; }
    public string Size { get; set; } = "md";
    public List<DataTableColumn> Columns { get; set; } = new();
    public List<DataTableRow> StaticData { get; set; } = new();
}

public class DataTableColumn
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public string? Href { get; set; }
    public string? HrefText { get; set; }
    public string? Width { get; set; }
    public string Align { get; set; } = "left";
    public bool Hidden { get; set; }
}