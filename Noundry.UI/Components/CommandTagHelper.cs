using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-command")]
public class CommandTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Placeholder text for search input
    /// </summary>
    public string Placeholder { get; set; } = "Type a command or search...";

    /// <summary>
    /// Maximum height of results area
    /// </summary>
    public string MaxHeight { get; set; } = "max-h-[320px]";

    /// <summary>
    /// Whether to show search icon
    /// </summary>
    public bool ShowSearchIcon { get; set; } = true;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var commandContext = new CommandContext();
        context.Items[typeof(CommandContext)] = commandContext;

        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddProperty("commandItems", new Dictionary<string, object[]>())
            .AddProperty("commandItemsFiltered", new object[0])
            .AddProperty("commandItemActive", "null")
            .AddProperty("commandItemSelected", "null")
            .AddStringProperty("commandId", GenerateUniqueId("command"))
            .AddStringProperty("commandSearch", "")
            .AddMethod("commandSearchIsEmpty() { return this.commandSearch.length == 0; }")
            .AddMethod("commandItemIsActive(item) { return this.commandItemActive && this.commandItemActive.value == item.value; }")
            .AddMethod("commandItemActiveNext() { let index = this.commandItemsFiltered.indexOf(this.commandItemActive); if(index < this.commandItemsFiltered.length-1) { this.commandItemActive = this.commandItemsFiltered[index+1]; this.commandScrollToActiveItem(); } }")
            .AddMethod("commandItemActivePrevious() { let index = this.commandItemsFiltered.indexOf(this.commandItemActive); if(index > 0) { this.commandItemActive = this.commandItemsFiltered[index-1]; this.commandScrollToActiveItem(); } }")
            .AddMethod("commandScrollToActiveItem() { if(this.commandItemActive) { const activeElement = document.getElementById(this.commandItemActive.value + '-' + this.commandId); if(!activeElement) return; const newScrollPos = (activeElement.offsetTop + activeElement.offsetHeight) - this.$refs.commandItemsList.offsetHeight; if(newScrollPos > 0) { this.$refs.commandItemsList.scrollTop = newScrollPos; } else { this.$refs.commandItemsList.scrollTop = 0; } } }")
            .AddMethod("commandSearchItems() { if(!this.commandSearchIsEmpty()) { const searchTerm = this.commandSearch.replace(/\\*/g, '').toLowerCase(); this.commandItemsFiltered = this.commandItems.filter(item => item.title.toLowerCase().startsWith(searchTerm)); this.commandScrollToActiveItem(); } else { this.commandItemsFiltered = this.commandItems.filter(item => item.default); } this.commandItemActive = this.commandItemsFiltered[0]; }")
            .AddMethod("commandShowCategory(item, index) { if(index == 0) return true; if(typeof this.commandItems[index-1] === 'undefined') return false; return item.category != this.commandItems[index-1].category; }")
            .AddMethod("commandCategoryCapitalize(string) { return string.charAt(0).toUpperCase() + string.slice(1); }")
            .AddMethod("commandItemsReorganize() { const commandItemsOriginal = this.commandItems; const keys = Object.keys(this.commandItems); this.commandItems = []; keys.forEach((key, index) => { for(let i = 0; i < commandItemsOriginal[key].length; i++) { commandItemsOriginal[key][i].category = key; this.commandItems.push(commandItemsOriginal[key][i]); } }); }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "init", "commandItemsReorganize(); commandSearchItems(); $watch('commandSearch', value => commandSearchItems()); $watch('commandItemSelected', function(item) { if(item) { console.log('item:', item); } });");

        output.Attributes.SetAttribute("@keydown.down", "event.preventDefault(); commandItemActiveNext();");
        output.Attributes.SetAttribute("@keydown.up", "event.preventDefault(); commandItemActivePrevious();");
        output.Attributes.SetAttribute("@keydown.enter", "commandItemSelected=commandItemActive;");
        output.Attributes.SetAttribute("x-cloak", "");

        var classes = new List<string>
        {
            "flex", "min-h-[370px]", "justify-center", "w-full", "max-w-xl", "items-start"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var searchIcon = ShowSearchIcon ? CreateIcon(Icons.Search, "w-4 h-4 mr-0 text-neutral-400 shrink-0") : null;

        var commandHtml = $@"
            <div class=""flex flex-col w-full h-full overflow-hidden bg-white border rounded-lg shadow-md"">
                <div class=""flex items-center px-3 border-b"">
                    {(ShowSearchIcon ? searchIcon : "")}
                    <input type=""text"" x-ref=""commandInput"" x-model=""commandSearch"" 
                           class=""flex w-full px-2 py-3 text-sm bg-transparent border-0 rounded-md outline-none focus:outline-none focus:ring-0 focus:border-0 placeholder:text-neutral-400 h-11 disabled:cursor-not-allowed disabled:opacity-50"" 
                           placeholder=""{EscapeJavaScriptString(Placeholder)}"" 
                           autocomplete=""off"" autocorrect=""off"" spellcheck=""false"">
                </div>
                <div x-ref=""commandItemsList"" class=""{MaxHeight} overflow-y-auto overflow-x-hidden"">
                    <template x-for=""(item, index) in commandItemsFiltered"" :key=""'item-' + index"">
                        <div class=""pb-1 space-y-1"">
                            <template x-if=""commandShowCategory(item, index)"">
                                <div class=""px-1 overflow-hidden text-gray-700"">
                                    <div class=""px-2 py-1 my-1 text-xs font-medium text-neutral-500"" x-text=""commandCategoryCapitalize(item.category)""></div>
                                </div>
                            </template>
                            
                            <template x-if=""(item.default && commandSearchIsEmpty()) || !commandSearchIsEmpty()"">
                                <div class=""px-1"">
                                    <div :id=""item.value + '-' + commandId"" @click=""commandItemSelected=item"" @mousemove=""commandItemActive=item"" 
                                         :class=""{{ 'bg-neutral-100 text-gray-900' : commandItemIsActive(item) }}"" 
                                         class=""relative flex cursor-default select-none items-center rounded-sm px-2 py-1.5 text-sm outline-none data-[disabled]:pointer-events-none data-[disabled]:opacity-50"">
                                        <span x-html=""item.icon""></span>
                                        <span x-text=""item.title""></span>
                                        <template x-if=""item.right"">
                                            <span class=""ml-auto text-xs tracking-widest text-muted-foreground"" x-text=""item.right""></span>
                                        </template>
                                    </div>
                                </div>
                            </template>
                        </div>
                    </template>
                </div>
            </div>
            {content}";

        output.Content.SetHtmlContent(commandHtml);
    }
}

[HtmlTargetElement("noundry-command-group")]
public class CommandGroupTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Group name/category
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var commandContext = (CommandContext?)context.Items[typeof(CommandContext)];
        var groupContext = new CommandGroupContext { Name = Name };
        context.Items[typeof(CommandGroupContext)] = groupContext;

        var content = await output.GetChildContentAsync();

        if (commandContext != null)
        {
            commandContext.Groups[Name] = groupContext.Items.ToArray();
        }

        output.SuppressOutput();
    }
}

[HtmlTargetElement("noundry-command-item")]
public class CommandItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Item title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Item value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Icon HTML or name
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Keyboard shortcut text
    /// </summary>
    public string? Shortcut { get; set; }

    /// <summary>
    /// Whether item is shown by default
    /// </summary>
    public bool Default { get; set; } = true;

    /// <summary>
    /// Whether item is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var groupContext = (CommandGroupContext?)context.Items[typeof(CommandGroupContext)];
        var content = await output.GetChildContentAsync();

        var item = new CommandItem
        {
            Title = Title,
            Value = Value,
            Icon = Icon ?? "",
            Shortcut = Shortcut,
            Default = Default,
            Disabled = Disabled
        };

        if (groupContext != null)
        {
            groupContext.Items.Add(item);
        }

        output.SuppressOutput();
    }
}

public class CommandContext
{
    public Dictionary<string, object[]> Groups { get; set; } = new();
}

public class CommandGroupContext
{
    public string Name { get; set; } = string.Empty;
    public List<CommandItem> Items { get; set; } = new();
}

public class CommandItem
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Shortcut { get; set; }
    public bool Default { get; set; } = true;
    public bool Disabled { get; set; } = false;
}