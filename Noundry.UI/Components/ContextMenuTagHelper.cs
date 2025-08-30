using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-context-menu")]
public class ContextMenuTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Text to display in the trigger area
    /// </summary>
    public string TriggerText { get; set; } = "Right click here";

    /// <summary>
    /// Width of the trigger area
    /// </summary>
    public string TriggerWidth { get; set; } = "w-[300px]";

    /// <summary>
    /// Height of the trigger area
    /// </summary>
    public string TriggerHeight { get; set; } = "h-[150px]";

    /// <summary>
    /// Additional CSS classes for trigger area
    /// </summary>
    public string? TriggerCssClass { get; set; }

    /// <summary>
    /// Additional CSS classes for menu
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var contextMenuContext = new ContextMenuContext();
        context.Items[typeof(ContextMenuContext)] = contextMenuContext;

        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("contextMenuOpen", false)
            .AddMethod("contextMenuToggle(event) { this.contextMenuOpen = true; event.preventDefault(); this.$refs.contextmenu.classList.add('opacity-0'); const that = this; this.$nextTick(function() { that.calculateContextMenuPosition(event); that.calculateSubMenuPosition(event); that.$refs.contextmenu.classList.remove('opacity-0'); }); }")
            .AddMethod("calculateContextMenuPosition(clickEvent) { if(window.innerHeight < clickEvent.clientY + this.$refs.contextmenu.offsetHeight) { this.$refs.contextmenu.style.top = (window.innerHeight - this.$refs.contextmenu.offsetHeight) + 'px'; } else { this.$refs.contextmenu.style.top = clickEvent.clientY + 'px'; } if(window.innerWidth < clickEvent.clientX + this.$refs.contextmenu.offsetWidth) { this.$refs.contextmenu.style.left = (clickEvent.clientX - this.$refs.contextmenu.offsetWidth) + 'px'; } else { this.$refs.contextmenu.style.left = clickEvent.clientX + 'px'; } }")
            .AddMethod("calculateSubMenuPosition(clickEvent) { const submenus = document.querySelectorAll('[data-submenu]'); const contextMenuWidth = this.$refs.contextmenu.offsetWidth; for(let i = 0; i < submenus.length; i++) { if(window.innerWidth < (clickEvent.clientX + contextMenuWidth + submenus[i].offsetWidth)) { submenus[i].classList.add('left-0', '-translate-x-full'); submenus[i].classList.remove('right-0', 'translate-x-full'); } else { submenus[i].classList.remove('left-0', '-translate-x-full'); submenus[i].classList.add('right-0', 'translate-x-full'); } } }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "init", "$watch('contextMenuOpen', function(value) { if(value === true) { document.body.classList.add('overflow-hidden'); } else { document.body.classList.remove('overflow-hidden'); } }); window.addEventListener('resize', function(event) { contextMenuOpen = false; });");

        output.Attributes.SetAttribute("@contextmenu", "contextMenuToggle(event)");

        var triggerClasses = new List<string>
        {
            "relative", "z-50", "flex", "text-sm", "items-center", "justify-center", 
            "rounded-md", "border", "border-neutral-300", "border-dashed", "text-neutral-800",
            TriggerHeight, TriggerWidth
        };

        if (!string.IsNullOrEmpty(TriggerCssClass))
        {
            triggerClasses.Add(TriggerCssClass);
        }

        AddCssClasses(output, triggerClasses.ToArray());

        var menuClasses = new List<string>
        {
            "z-50", "min-w-[8rem]", "text-neutral-800", "rounded-md", "border", "border-neutral-200/70", 
            "bg-white", "text-sm", "fixed", "p-1", "shadow-md", "w-64"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            menuClasses.Add(CssClass);
        }

        var contextMenuHtml = $@"
            <span class=""cursor-default text-neutral-400"">{EscapeJavaScriptString(TriggerText)}</span>

            <template x-teleport=""body"">
                <div x-show=""contextMenuOpen"" @click.away=""contextMenuOpen=false"" x-ref=""contextmenu"" 
                     class=""{string.Join(" ", menuClasses)}"" x-cloak>
                    {content}
                </div>
            </template>";

        output.Content.SetHtmlContent(contextMenuHtml);
    }
}

[HtmlTargetElement("noundry-context-menu-item")]
public class ContextMenuItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Item text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Icon name or HTML
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Keyboard shortcut text
    /// </summary>
    public string? Shortcut { get; set; }

    /// <summary>
    /// Whether this item is a separator
    /// </summary>
    public bool IsSeparator { get; set; } = false;

    /// <summary>
    /// Whether this item is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// Whether this item is checkable
    /// </summary>
    public bool Checkable { get; set; } = false;

    /// <summary>
    /// Whether this item is checked (for checkable items)
    /// </summary>
    public bool Checked { get; set; } = false;

    /// <summary>
    /// Radio group name (for radio-style items)
    /// </summary>
    public string? RadioGroup { get; set; }

    /// <summary>
    /// Whether this has a submenu
    /// </summary>
    public bool HasSubmenu { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        if (IsSeparator)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "h-px my-1 -mx-1 bg-neutral-200");
            output.Content.Clear();
            return;
        }

        output.TagName = "div";

        var classes = new List<string>
        {
            "relative", "flex", "cursor-default", "select-none", "items-center", "rounded", 
            "px-2", "py-1.5", "hover:bg-neutral-100", "outline-none"
        };

        if (Disabled)
        {
            classes.AddRange(new[] { "opacity-50", "pointer-events-none" });
            output.Attributes.SetAttribute("data-disabled", "true");
        }
        else
        {
            output.Attributes.SetAttribute("@click", "contextMenuOpen=false");
        }

        if (HasSubmenu)
        {
            classes.Add("group");
        }

        if (Checkable || !string.IsNullOrEmpty(RadioGroup))
        {
            classes.Add("pl-8");
        }

        AddCssClasses(output, classes.ToArray());

        var iconContent = !string.IsNullOrEmpty(Icon) ? GetIconHtml(Icon) : "";
        var checkIcon = (Checkable && Checked) || (!string.IsNullOrEmpty(RadioGroup) && Checked) 
            ? CreateIcon(Icons.Check, "absolute left-2 flex h-3.5 w-3.5 items-center justify-center w-4 h-4") 
            : null;
        var submenuIcon = HasSubmenu ? CreateIcon(Icons.ChevronRight, "w-4 h-4 ml-auto") : null;

        var itemHtml = $@"
            {checkIcon}
            {iconContent}
            <span>{EscapeJavaScriptString(Text)}</span>
            {(!string.IsNullOrEmpty(Shortcut) ? $@"<span class=""ml-auto text-xs tracking-widest text-neutral-400 group-hover:text-neutral-600"">{EscapeJavaScriptString(Shortcut)}</span>" : "")}
            {submenuIcon}
            {(HasSubmenu ? $@"
            <div data-submenu class=""absolute top-0 right-0 invisible mr-1 duration-200 ease-out translate-x-full opacity-0 group-hover:mr-0 group-hover:visible group-hover:opacity-100"">
                <div class=""z-50 min-w-[8rem] overflow-hidden rounded-md border bg-white p-1 shadow-md animate-in slide-in-from-left-1 w-48"">
                    {content}
                </div>
            </div>
            " : "")}";

        output.Content.SetHtmlContent(itemHtml);
    }

    private string GetIconHtml(string icon)
    {
        // If it's just an icon name, convert to icon
        var iconContent = icon switch
        {
            "back" => Icons.ArrowLeft,
            "forward" => Icons.ArrowRight,
            "reload" => """<path stroke-linecap="round" stroke-linejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99" />""",
            _ => icon
        };

        // If it looks like HTML, return as-is, otherwise create icon
        return icon.Contains('<') ? icon : CreateIcon(iconContent, "w-4 h-4 mr-2").ToString() ?? "";
    }
}

public class ContextMenuContext
{
    public List<ContextMenuItem> Items { get; set; } = new();
}

public class ContextMenuItem
{
    public string Text { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Shortcut { get; set; }
    public bool IsSeparator { get; set; }
    public bool Disabled { get; set; }
    public bool Checkable { get; set; }
    public bool Checked { get; set; }
    public string? RadioGroup { get; set; }
    public bool HasSubmenu { get; set; }
    public List<ContextMenuItem> SubmenuItems { get; set; } = new();
}