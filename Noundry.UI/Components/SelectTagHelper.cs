using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-select")]
public class SelectTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Whether to enable search/filtering
    /// </summary>
    public bool Searchable { get; set; } = true;

    /// <summary>
    /// Whether multiple selections are allowed
    /// </summary>
    public bool Multiple { get; set; } = false;

    /// <summary>
    /// Search placeholder text
    /// </summary>
    public string SearchPlaceholder { get; set; } = "Search...";

    /// <summary>
    /// No results text
    /// </summary>
    public string NoResultsText { get; set; } = "No results found";

    /// <summary>
    /// Select all text (for multiple)
    /// </summary>
    public string SelectAllText { get; set; } = "Select All";

    /// <summary>
    /// Maximum height for dropdown
    /// </summary>
    public string MaxHeight { get; set; } = "max-h-60";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var selectContext = new SelectContext 
        { 
            Multiple = Multiple, 
            Searchable = Searchable,
            InputName = GetInputName(),
            InputId = GetInputId()
        };
        context.Items[typeof(SelectContext)] = selectContext;

        var content = await output.GetChildContentAsync();
        
        output.TagName = "div";

        var alpineData = BuildAlpineData();
        SetAlpineAttribute(output, "data", alpineData);

        AddCssClasses(output, "relative", "w-full");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        // Initialize options from child content
        await output.GetChildContentAsync();
        
        // Update Alpine data with options
        var optionsJs = string.Join(",", selectContext.Options.Select(opt => 
            $"{{value: '{EscapeJavaScriptString(opt.Value)}', text: '{EscapeJavaScriptString(opt.Text)}', disabled: {opt.Disabled.ToString().ToLower()}}}"));
        
        var updatedAlpineData = alpineData.Replace("\"selectOptions\": []", $"\"selectOptions\": [{optionsJs}]");
        SetAlpineAttribute(output, "data", updatedAlpineData);

        var selectHtml = BuildSelectHtml(selectContext);
        output.Content.SetHtmlContent(selectHtml);
    }

    private string BuildAlpineData()
    {
        var builder = new AlpineDataBuilder()
            .AddBooleanProperty("selectOpen", false)
            .AddStringProperty("selectSearch", "")
            .AddProperty("selectSelected", Multiple ? new string[0] : "")
            .AddProperty("selectOptions", new object[0])
            .AddMethod("selectToggle() { this.selectOpen = !this.selectOpen; }")
            .AddMethod("selectClose() { this.selectOpen = false; this.selectSearch = ''; }")
            .AddMethod("selectOption(option) { " + (Multiple ? 
                "if(this.selectSelected.includes(option.value)) { this.selectSelected = this.selectSelected.filter(item => item !== option.value); } else { this.selectSelected.push(option.value); }" :
                "this.selectSelected = option.value; this.selectClose();") + " }")
            .AddMethod("isSelected(option) { " + (Multiple ? "return this.selectSelected.includes(option.value);" : "return this.selectSelected === option.value;") + " }")
            .AddMethod("getDisplayText() { " + (Multiple ?
                "if(this.selectSelected.length === 0) return '" + (Placeholder ?? "Select options") + "'; if(this.selectSelected.length === 1) { const option = this.selectOptions.find(opt => opt.value === this.selectSelected[0]); return option ? option.text : this.selectSelected[0]; } return this.selectSelected.length + ' selected';" :
                "if(!this.selectSelected) return '" + (Placeholder ?? "Select option") + "'; const option = this.selectOptions.find(opt => opt.value === this.selectSelected); return option ? option.text : this.selectSelected;") + " }")
            .AddMethod("filteredOptions() { if(!this.selectSearch) return this.selectOptions; return this.selectOptions.filter(option => option.text.toLowerCase().includes(this.selectSearch.toLowerCase())); }");

        return builder.Build();
    }

    private string BuildSelectHtml(SelectContext context)
    {
        var chevronIcon = CreateIcon(Icons.ChevronDown, "w-5 h-5 text-gray-400");
        var searchIcon = CreateIcon(Icons.Search, "w-4 h-4 text-gray-400");
        var checkIcon = CreateIcon(Icons.Check, "w-4 h-4 text-white");

        return $@"
            <div class=""relative"">
                <button @click=""selectToggle()"" type=""button"" 
                    class=""relative w-full bg-white border border-gray-300 rounded-md shadow-sm pl-3 pr-10 py-2 text-left cursor-default focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm""
                    :class="""" 'ring-1 ring-blue-500 border-blue-500': selectOpen """">
                    <span class=""block truncate"" x-text=""getDisplayText()""></span>
                    <span class=""absolute inset-y-0 right-0 flex items-center pr-2 pointer-events-none"">
                        <div :class="""" 'rotate-180': selectOpen """" class=""transition-transform duration-200"">
                            {chevronIcon}
                        </div>
                    </span>
                </button>

                <div x-show=""selectOpen"" @click.away=""selectClose()"" 
                    x-transition:enter=""transition ease-out duration-100""
                    x-transition:enter-start=""transform opacity-0 scale-95""
                    x-transition:enter-end=""transform opacity-100 scale-100""
                    x-transition:leave=""transition ease-in duration-75""
                    x-transition:leave-start=""transform opacity-100 scale-100""
                    x-transition:leave-end=""transform opacity-0 scale-95""
                    class=""absolute z-10 mt-1 w-full bg-white shadow-lg {MaxHeight} rounded-md py-1 text-base ring-1 ring-black ring-opacity-5 overflow-auto focus:outline-none sm:text-sm""
                    x-cloak>
                    
                    {(Searchable ? $@"
                    <div class=""px-2 py-2 border-b border-gray-200"">
                        <div class=""relative"">
                            <div class=""absolute inset-y-0 left-0 pl-2 flex items-center pointer-events-none"">
                                {searchIcon}
                            </div>
                            <input type=""text"" x-model=""selectSearch"" 
                                class=""block w-full pl-8 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm""
                                placeholder=""{SearchPlaceholder}"">
                        </div>
                    </div>
                    " : "")}

                    <div x-show=""filteredOptions().length === 0"" class=""px-3 py-2 text-gray-500"">
                        {NoResultsText}
                    </div>

                    <template x-for=""option in filteredOptions()"" :key=""option.value"">
                        <div @click=""selectOption(option)"" 
                            class=""cursor-pointer select-none relative py-2 pl-3 pr-9 hover:bg-blue-50""
                            :class="""" 'bg-blue-50': isSelected(option) """">
                            <span class=""block truncate"" :class="""" 'font-semibold': isSelected(option) """" x-text=""option.text""></span>
                            <span x-show=""isSelected(option)"" class=""absolute inset-y-0 right-0 flex items-center pr-4 text-blue-600"">
                                {checkIcon}
                            </span>
                        </div>
                    </template>
                </div>
            </div>

            {(Multiple ? 
                @"<input type=""hidden"" :name=""selectSelected.map((val, index) => `" + context.InputName + @"[${index}]`).join(',')"" :value=""selectSelected.join(',')"" x-show=""selectSelected.length > 0"">" :
                $@"<input type=""hidden"" name=""{context.InputName}"" :value=""selectSelected"">")}
            ";
    }
}

[HtmlTargetElement("noundry-option")]
public class OptionTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Option value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Whether this option is selected
    /// </summary>
    public bool Selected { get; set; } = false;

    /// <summary>
    /// Whether this option is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var selectContext = (SelectContext?)context.Items[typeof(SelectContext)];
        var content = await output.GetChildContentAsync();

        if (selectContext != null)
        {
            selectContext.Options.Add(new SelectOption
            {
                Value = Value,
                Text = content.GetContent(),
                Selected = Selected,
                Disabled = Disabled
            });
        }

        output.SuppressOutput();
    }
}

public class SelectContext
{
    public bool Multiple { get; set; }
    public bool Searchable { get; set; }
    public string InputName { get; set; } = string.Empty;
    public string InputId { get; set; } = string.Empty;
    public List<SelectOption> Options { get; set; } = new();
}

public class SelectOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
    public bool Disabled { get; set; }
}