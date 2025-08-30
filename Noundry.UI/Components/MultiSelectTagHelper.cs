using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-multi-select")]
public class MultiSelectTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Label text for the multi-select
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Whether to load options from API
    /// </summary>
    public bool UseApi { get; set; } = false;

    /// <summary>
    /// API endpoint URL for loading options
    /// </summary>
    public string? ApiUrl { get; set; }

    /// <summary>
    /// Property name for option ID in API response
    /// </summary>
    public string ApiIdProperty { get; set; } = "id";

    /// <summary>
    /// Property name for option name in API response
    /// </summary>
    public string ApiNameProperty { get; set; } = "name";

    /// <summary>
    /// Loading text
    /// </summary>
    public string LoadingText { get; set; } = "Loading options...";

    /// <summary>
    /// Error text when API fails
    /// </summary>
    public string ErrorText { get; set; } = "Failed to load options. Please try again.";

    /// <summary>
    /// Placeholder text when no options selected
    /// </summary>
    public string SelectPlaceholder { get; set; } = "Select options...";

    /// <summary>
    /// Maximum height for dropdown
    /// </summary>
    public string MaxHeight { get; set; } = "max-h-60";

    /// <summary>
    /// Whether to show remove buttons on selected items
    /// </summary>
    public bool ShowRemoveButtons { get; set; } = true;

    /// <summary>
    /// Selected items display style (tags, list, count)
    /// </summary>
    public string DisplayStyle { get; set; } = "tags";

    /// <summary>
    /// Maximum number of items to display before showing count
    /// </summary>
    public int MaxDisplayItems { get; set; } = 5;

    /// <summary>
    /// Color scheme for selected items (blue, green, red, gray)
    /// </summary>
    public string Color { get; set; } = "blue";

    /// <summary>
    /// Help text displayed below the multi-select
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public new string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var multiSelectContext = new MultiSelectContext 
        { 
            InputName = GetInputName(),
            UseApi = UseApi,
            ApiUrl = ApiUrl ?? "",
            ApiIdProperty = ApiIdProperty,
            ApiNameProperty = ApiNameProperty
        };
        context.Items[typeof(MultiSelectContext)] = multiSelectContext;

        var content = await output.GetChildContentAsync();
        
        var inputId = GetInputId();
        var labelText = Label ?? GetDisplayName();

        output.TagName = "div";
        AddCssClasses(output, "space-y-1");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var alpineData = BuildMultiSelectData(multiSelectContext);
        SetAlpineAttribute(output, "data", alpineData);
        
        if (UseApi)
        {
            SetAlpineAttribute(output, "init", "init()");
        }

        output.Attributes.SetAttribute("@keydown.escape", "close($refs.button)");
        output.Attributes.SetAttribute("@click.away", "close()");
        output.Attributes.SetAttribute("x-cloak", "");

        var hasError = !string.IsNullOrEmpty(ErrorMessage);
        var chevronIcon = CreateIcon(Icons.ChevronDown, "h-5 w-5 text-gray-400");
        var checkIcon = CreateIcon(Icons.Check, "h-5 w-5");
        var removeIcon = CreateIcon(Icons.Close, "h-2 w-2");

        var colorClasses = GetColorClasses();

        var multiSelectHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <label for=""{inputId}"" class=""block text-sm font-medium text-gray-700 mb-1"">
                {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </label>
            " : "")}

            <div class=""relative"">
                <!-- Multiselect button -->
                <button
                    x-ref=""button""
                    @click=""toggle()""
                    type=""button""
                    id=""{inputId}""
                    aria-haspopup=""listbox""
                    :aria-expanded=""open""
                    :disabled=""loading || {Disabled.ToString().ToLower()}""
                    class=""relative w-full bg-white border rounded-md shadow-sm pl-3 pr-10 py-2 text-left cursor-default focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm {(hasError ? "border-red-300" : "border-gray-300")}""
                >
                    <span class=""flex flex-wrap gap-2"">
                        {(UseApi ? $@"
                        <template x-if=""loading"">
                            <span class=""text-gray-500"">{EscapeJavaScriptString(LoadingText)}</span>
                        </template>
                        " : "")}
                        
                        <template x-if=""{(UseApi ? "!loading && " : "")}selected.length === 0"">
                            <span class=""text-gray-500"">{EscapeJavaScriptString(SelectPlaceholder)}</span>
                        </template>
                        
                        <template x-if=""selected.length > 0"">
                            <div class=""flex flex-wrap gap-2"">
                                <!-- Display style: tags -->
                                <template x-if=""'{DisplayStyle}' === 'tags' && selected.length <= {MaxDisplayItems}"">
                                    <template x-for=""option in selected"" :key=""option.id || option.value"">
                                        <span class=""inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium {colorClasses.bg} {colorClasses.text}"">
                                            <span x-text=""option.name || option.text""></span>
                                            {(ShowRemoveButtons ? $@"
                                            <button 
                                                @click.stop=""remove(option)"" 
                                                type=""button"" 
                                                class=""flex-shrink-0 ml-1 h-4 w-4 rounded-full inline-flex items-center justify-center {colorClasses.removeButton} focus:outline-none focus:bg-blue-500 focus:text-white""
                                            >
                                                <span class=""sr-only"">Remove</span>
                                                {removeIcon}
                                            </button>
                                            " : "")}
                                        </span>
                                    </template>
                                </template>
                                
                                <!-- Display style: count when too many items -->
                                <template x-if=""'{DisplayStyle}' === 'tags' && selected.length > {MaxDisplayItems}"">
                                    <span class=""inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium {colorClasses.bg} {colorClasses.text}"">
                                        <span x-text=""selected.length + ' items selected'""></span>
                                        {(ShowRemoveButtons ? @"
                                        <button 
                                            @click.stop=""clearAll()"" 
                                            type=""button"" 
                                            class=""flex-shrink-0 ml-1 h-4 w-4 rounded-full inline-flex items-center justify-center text-blue-400 hover:bg-blue-200 hover:text-blue-500 focus:outline-none focus:bg-blue-500 focus:text-white""
                                        >
                                            <span class=""sr-only"">Clear All</span>
                                            <svg class=""h-2 w-2"" stroke=""currentColor"" fill=""none"" viewBox=""0 0 8 8"">
                                                <path stroke-linecap=""round"" stroke-width=""1.5"" d=""M1 1l6 6m0-6L1 7"" />
                                            </svg>
                                        </button>
                                        " : "")}
                                    </span>
                                </template>
                                
                                <!-- Display style: count -->
                                <template x-if=""'{DisplayStyle}' === 'count'"">
                                    <span class=""text-sm text-gray-700"">
                                        <span x-text=""selected.length""></span> item<span x-show=""selected.length !== 1"">s</span> selected
                                    </span>
                                </template>
                            </div>
                        </template>
                    </span>
                    
                    <span class=""absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none"">
                        <div :class=""{{ 'rotate-180': open }}"" class=""transition-transform duration-200"">
                            {chevronIcon}
                        </div>
                    </span>
                </button>

                <!-- Options dropdown -->
                <div 
                    x-show=""open"" 
                    x-transition:enter=""transition ease-out duration-100"" 
                    x-transition:enter-start=""transform opacity-0 scale-95"" 
                    x-transition:enter-end=""transform opacity-100 scale-100"" 
                    x-transition:leave=""transition ease-in duration-75"" 
                    x-transition:leave-start=""transform opacity-100 scale-100"" 
                    x-transition:leave-end=""transform opacity-0 scale-95"" 
                    class=""absolute z-10 mt-1 w-full bg-white shadow-lg {MaxHeight} rounded-md py-1 text-base ring-1 ring-black ring-opacity-5 overflow-auto focus:outline-none sm:text-sm""
                    tabindex=""-1""
                    role=""listbox""
                    x-cloak
                >
                    <template x-if=""error"">
                        <div class=""text-red-500 px-3 py-2"" x-text=""error""></div>
                    </template>
                    <template x-if=""loading"">
                        <div class=""text-gray-500 px-3 py-2"">{EscapeJavaScriptString(LoadingText)}</div>
                    </template>
                    <template x-for=""(option, index) in options"" :key=""option.id || option.value"">
                        <div
                            :id=""'listbox-option-' + index""
                            @click=""select(option)""
                            role=""option""
                            :aria-selected=""isSelected(option)""
                            :class=""{{ 'text-white bg-blue-600': isSelected(option), 'text-gray-900 hover:bg-gray-100 hover:text-gray-900': !isSelected(option) }}""
                            class=""cursor-default select-none relative py-2 pl-3 pr-9""
                        >
                            <div class=""flex items-center"">
                                <span x-text=""option.name || option.text"" :class=""{{ 'font-semibold': isSelected(option), 'font-normal': !isSelected(option) }}"" class=""block truncate""></span>
                            </div>

                            <span 
                                x-show=""isSelected(option)"" 
                                class=""absolute inset-y-0 right-0 flex items-center pr-4""
                                :class=""{{ 'text-white': isSelected(option), 'text-blue-600': !isSelected(option) }}""
                            >
                                {checkIcon}
                            </span>
                        </div>
                    </template>
                </div>
                
                <!-- Hidden inputs for form submission -->
                <template x-for=""(option, index) in selected"" :key=""option.id || option.value"">
                    <input type=""hidden"" :name=""`{GetInputName()}[${{index}}]`"" :value=""option.id || option.value"" />
                </template>
            </div>

            {(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-gray-500"">
                {EscapeJavaScriptString(HelpText)}
            </p>
            " : "")}

            {(!string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-red-600"">
                {EscapeJavaScriptString(ErrorMessage)}
            </p>
            " : "")}";

        output.Content.SetHtmlContent(multiSelectHtml + content.GetContent());
    }

    private string BuildMultiSelectData(MultiSelectContext context)
    {
        var builder = new AlpineDataBuilder()
            .AddBooleanProperty("open", false)
            .AddProperty("options", new object[0])
            .AddProperty("selected", new object[0])
            .AddBooleanProperty("loading", UseApi)
            .AddProperty("error", "null");

        // Core methods
        builder.AddMethod("toggle() { this.open = !this.open; }");
        builder.AddMethod("isSelected(option) { return this.selected.some(item => (item.id || item.value) === (option.id || option.value)); }");
        builder.AddMethod("select(option) { if (this.isSelected(option)) { this.selected = this.selected.filter(item => (item.id || item.value) !== (option.id || option.value)); } else { this.selected.push(option); } }");
        builder.AddMethod("remove(option) { this.selected = this.selected.filter(item => (item.id || item.value) !== (option.id || option.value)); }");
        builder.AddMethod("clearAll() { this.selected = []; }");
        builder.AddMethod("close(focusAfter) { this.open = false; focusAfter && focusAfter.focus(); }");

        if (UseApi && !string.IsNullOrEmpty(ApiUrl))
        {
            builder.AddMethod($@"async init() {{ await this.fetchOptions(); }}");
            builder.AddMethod($@"async fetchOptions() {{ 
                try {{ 
                    const response = await fetch('{EscapeJavaScriptString(ApiUrl)}'); 
                    if (!response.ok) {{ 
                        throw new Error('Failed to fetch options'); 
                    }} 
                    const data = await response.json(); 
                    this.options = data.map(item => ({{ 
                        id: item.{ApiIdProperty}, 
                        name: item.{ApiNameProperty} 
                    }})); 
                    this.loading = false; 
                }} catch (error) {{ 
                    console.error('Error fetching options:', error); 
                    this.error = '{EscapeJavaScriptString(ErrorText)}'; 
                    this.loading = false; 
                }} 
            }}");
        }
        else
        {
            // For static options, populate from child content
            var optionsJs = string.Join(",", context.Options.Select(opt => 
                $"{{id: '{EscapeJavaScriptString(opt.Value)}', name: '{EscapeJavaScriptString(opt.Text)}', value: '{EscapeJavaScriptString(opt.Value)}', text: '{EscapeJavaScriptString(opt.Text)}'}}"));
            
            builder.AddProperty("options", $"[{optionsJs}]");
        }

        return builder.Build();
    }

    private dynamic GetColorClasses()
    {
        return Color switch
        {
            "green" => new 
            { 
                bg = "bg-green-100", 
                text = "text-green-800",
                removeButton = "text-green-400 hover:bg-green-200 hover:text-green-500"
            },
            "red" => new 
            { 
                bg = "bg-red-100", 
                text = "text-red-800",
                removeButton = "text-red-400 hover:bg-red-200 hover:text-red-500"
            },
            "gray" => new 
            { 
                bg = "bg-gray-100", 
                text = "text-gray-800",
                removeButton = "text-gray-400 hover:bg-gray-200 hover:text-gray-500"
            },
            _ => new // blue
            { 
                bg = "bg-blue-100", 
                text = "text-blue-800",
                removeButton = "text-blue-400 hover:bg-blue-200 hover:text-blue-500"
            }
        };
    }
}

[HtmlTargetElement("noundry-multi-select-option")]
public class MultiSelectOptionTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Option value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Option ID (for API compatibility)
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Whether this option is initially selected
    /// </summary>
    public bool Selected { get; set; } = false;

    /// <summary>
    /// Whether this option is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var multiSelectContext = (MultiSelectContext?)context.Items[typeof(MultiSelectContext)];
        var content = await output.GetChildContentAsync();

        if (multiSelectContext != null && !multiSelectContext.UseApi)
        {
            multiSelectContext.Options.Add(new MultiSelectOption
            {
                Id = Id ?? Value,
                Value = Value,
                Text = content.GetContent(),
                Selected = Selected,
                Disabled = Disabled
            });
        }

        output.SuppressOutput();
    }
}

public class MultiSelectContext
{
    public string InputName { get; set; } = string.Empty;
    public bool UseApi { get; set; }
    public string ApiUrl { get; set; } = string.Empty;
    public string ApiIdProperty { get; set; } = "id";
    public string ApiNameProperty { get; set; } = "name";
    public List<MultiSelectOption> Options { get; set; } = new();
}

public class MultiSelectOption
{
    public string Id { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
    public bool Disabled { get; set; }
}