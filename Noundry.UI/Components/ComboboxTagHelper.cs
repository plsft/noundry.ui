using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-combobox")]
public class ComboboxTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Whether to enable search/filtering
    /// </summary>
    public bool Searchable { get; set; } = true;

    /// <summary>
    /// Search placeholder text
    /// </summary>
    public string SearchPlaceholder { get; set; } = "Search...";

    /// <summary>
    /// No results text
    /// </summary>
    public string NoResultsText { get; set; } = "No results found";

    /// <summary>
    /// Label text
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Maximum height for dropdown
    /// </summary>
    public string MaxHeight { get; set; } = "max-h-60";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var comboboxContext = new ComboboxContext 
        { 
            Searchable = Searchable,
            InputName = GetInputName(),
            InputId = GetInputId()
        };
        context.Items[typeof(ComboboxContext)] = comboboxContext;

        var content = await output.GetChildContentAsync();
        
        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("comboboxOpen", false)
            .AddStringProperty("comboboxSearch", "")
            .AddStringProperty("comboboxSelected", "")
            .AddProperty("comboboxOptions", new object[0])
            .AddMethod("comboboxToggle() { this.comboboxOpen = !this.comboboxOpen; }")
            .AddMethod("comboboxClose() { this.comboboxOpen = false; this.comboboxSearch = ''; }")
            .AddMethod("comboboxSelectOption(option) { this.comboboxSelected = option.value; this.comboboxSearch = option.text; this.comboboxClose(); }")
            .AddMethod("comboboxGetDisplayText() { return this.comboboxSearch || '" + (Placeholder ?? "Select option") + "'; }")
            .AddMethod("comboboxFilteredOptions() { if(!this.comboboxSearch) return this.comboboxOptions; return this.comboboxOptions.filter(option => option.text.toLowerCase().includes(this.comboboxSearch.toLowerCase())); }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "relative", "w-full");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var labelText = Label ?? GetDisplayName();
        var chevronIcon = CreateIcon(Icons.ChevronDown, "w-5 h-5 text-gray-400");
        var searchIcon = CreateIcon(Icons.Search, "w-4 h-4 text-gray-400");

        // Process child options
        await output.GetChildContentAsync();
        
        var optionsJs = string.Join(",", comboboxContext.Options.Select(opt => 
            $"{{value: '{EscapeJavaScriptString(opt.Value)}', text: '{EscapeJavaScriptString(opt.Text)}', disabled: {opt.Disabled.ToString().ToLower()}}}"));
        
        var updatedAlpineData = alpineData.Replace("\"comboboxOptions\": []", $"\"comboboxOptions\": [{optionsJs}]");
        SetAlpineAttribute(output, "data", updatedAlpineData);

        var comboboxHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <label class=""block text-sm font-medium text-gray-700 mb-1"">
                {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </label>
            " : "")}
            
            <div class=""relative"">
                <div class=""relative min-h-[38px] flex items-center justify-between w-full py-2 pl-3 pr-10 text-left bg-white border rounded-md shadow-sm cursor-default border-gray-300 focus-within:ring-1 focus-within:ring-blue-500 focus-within:border-blue-500"">
                    <input type=""text"" 
                           x-model=""comboboxSearch"" 
                           @click=""comboboxToggle()"" 
                           @focus=""comboboxOpen = true""
                           class=""flex-1 bg-transparent border-0 focus:ring-0 focus:outline-none text-sm""
                           placeholder=""{EscapeJavaScriptString(Placeholder ?? "")}""
                           {(Disabled ? "disabled" : "")}
                           {(Required ? "required" : "")} />
                    
                    <button type=""button"" @click=""comboboxToggle()"" class=""absolute inset-y-0 right-0 flex items-center pr-2"">
                        <div :class=""{{ 'rotate-180': comboboxOpen }}"" class=""transition-transform duration-200"">
                            {chevronIcon}
                        </div>
                    </button>
                </div>

                <div x-show=""comboboxOpen"" @click.away=""comboboxClose()"" 
                     x-transition:enter=""transition ease-out duration-100""
                     x-transition:enter-start=""transform opacity-0 scale-95""
                     x-transition:enter-end=""transform opacity-100 scale-100""
                     x-transition:leave=""transition ease-in duration-75""
                     x-transition:leave-start=""transform opacity-100 scale-100""
                     x-transition:leave-end=""transform opacity-0 scale-95""
                     class=""absolute z-10 mt-1 w-full bg-white shadow-lg {MaxHeight} rounded-md py-1 text-base ring-1 ring-black ring-opacity-5 overflow-auto focus:outline-none sm:text-sm""
                     x-cloak>
                    
                    <div x-show=""comboboxFilteredOptions().length === 0"" class=""px-3 py-2 text-gray-500"">
                        {NoResultsText}
                    </div>

                    <template x-for=""option in comboboxFilteredOptions()"" :key=""option.value"">
                        <div @click=""comboboxSelectOption(option)"" 
                             class=""cursor-pointer select-none relative py-2 pl-3 pr-9 hover:bg-blue-50""
                             :class=""{{ 'bg-blue-50': comboboxSelected === option.value, 'opacity-50 cursor-not-allowed': option.disabled }}""
                             :aria-disabled=""option.disabled"">
                            <span class=""block truncate"" :class=""{{ 'font-semibold': comboboxSelected === option.value }}"" x-text=""option.text""></span>
                        </div>
                    </template>
                </div>
            </div>

            <input type=""hidden"" name=""{GetInputName()}"" :value=""comboboxSelected"" />";

        output.Content.SetHtmlContent(comboboxHtml);
    }
}

[HtmlTargetElement("noundry-combobox-option")]
public class ComboboxOptionTagHelper : NoundryTagHelperBase
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
        var comboboxContext = (ComboboxContext?)context.Items[typeof(ComboboxContext)];
        var content = await output.GetChildContentAsync();

        if (comboboxContext != null)
        {
            comboboxContext.Options.Add(new ComboboxOption
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

public class ComboboxContext
{
    public bool Searchable { get; set; }
    public string InputName { get; set; } = string.Empty;
    public string InputId { get; set; } = string.Empty;
    public List<ComboboxOption> Options { get; set; } = new();
}

public class ComboboxOption
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
    public bool Disabled { get; set; }
}