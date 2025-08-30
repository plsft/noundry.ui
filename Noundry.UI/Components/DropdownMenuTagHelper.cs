using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-dropdown-menu")]
public class DropdownMenuTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Button text or content
    /// </summary>
    public string ButtonText { get; set; } = "Menu";

    /// <summary>
    /// Button variant
    /// </summary>
    public string ButtonVariant { get; set; } = "default";

    /// <summary>
    /// Dropdown position (left, center, right)
    /// </summary>
    public string Position { get; set; } = "center";

    /// <summary>
    /// Whether to show dropdown arrow icon
    /// </summary>
    public bool ShowArrow { get; set; } = true;

    /// <summary>
    /// User avatar image URL (optional)
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// User name for display
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// User subtitle (email, role, etc.)
    /// </summary>
    public string? UserSubtitle { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dropdownContext = new DropdownMenuContext();
        context.Items[typeof(DropdownMenuContext)] = dropdownContext;

        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("dropdownOpen", false)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "relative");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var buttonClasses = GetButtonClasses();
        var positionClasses = GetPositionClasses();
        var dropdownArrow = ShowArrow ? CreateIcon(Icons.ChevronDown, "absolute right-0 mr-3 w-5 h-5") : null;

        var buttonContent = BuildButtonContent();

        var dropdownHtml = $"""
            <button x-on:click="dropdownOpen=true" class="{string.Join(" ", buttonClasses)}">
                {buttonContent}
                {(ShowArrow ? dropdownArrow : "")}
            </button>

            <div x-show="dropdownOpen" 
                x-on:click.away="dropdownOpen=false"
                x-transition:enter="ease-out duration-200"
                x-transition:enter-start="-translate-y-2"
                x-transition:enter-end="translate-y-0"
                class="{positionClasses} absolute top-0 z-50 mt-12 w-56"
                x-cloak>
                <div class="p-1 mt-1 bg-white rounded-md border shadow-md border-neutral-200/70 text-neutral-700">
                    {content}
                </div>
            </div>
            """;

        output.Content.SetHtmlContent(dropdownHtml);
    }

    private string[] GetButtonClasses()
    {
        var baseClasses = new[]
        {
            "inline-flex", "justify-center", "items-center", "py-2", "pr-12", "pl-3", "h-12",
            "text-sm", "font-medium", "bg-white", "rounded-md", "border", "transition-colors",
            "text-neutral-700", "hover:bg-neutral-100", "active:bg-white", "focus:bg-white",
            "focus:outline-none", "disabled:opacity-50", "disabled:pointer-events-none"
        };

        return baseClasses;
    }

    private string GetPositionClasses()
    {
        return Position switch
        {
            "left" => "left-0",
            "right" => "right-0",
            _ => "left-1/2 -translate-x-1/2" // center
        };
    }

    private string BuildButtonContent()
    {
        if (!string.IsNullOrEmpty(AvatarUrl) && !string.IsNullOrEmpty(UserName))
        {
            return $"""
                <img src="{AvatarUrl}" class="object-cover w-8 h-8 rounded-full border border-neutral-200" />
                <span class="flex flex-col flex-shrink-0 items-start ml-2 h-full leading-none translate-y-px">
                    <span>{EscapeJavaScriptString(UserName)}</span>
                    {(!string.IsNullOrEmpty(UserSubtitle) ? $"""<span class="text-xs font-light text-neutral-400">{EscapeJavaScriptString(UserSubtitle)}</span>""" : "")}
                </span>
                """;
        }

        return EscapeJavaScriptString(ButtonText);
    }
}

[HtmlTargetElement("noundry-dropdown-item")]
public class DropdownItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Item text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Icon name
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Keyboard shortcut text
    /// </summary>
    public string? Shortcut { get; set; }

    /// <summary>
    /// Link href (makes it an anchor)
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Whether this is a separator
    /// </summary>
    public bool IsSeparator { get; set; } = false;

    /// <summary>
    /// Whether this item is disabled
    /// </summary>
    public bool Disabled { get; set; } = false;

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
            output.Attributes.SetAttribute("class", "-mx-1 my-1 h-px bg-neutral-200");
            output.SuppressOutput();
            return;
        }

        var tagName = !string.IsNullOrEmpty(Href) ? "a" : "div";
        output.TagName = tagName;

        var classes = new List<string>
        {
            "relative", "flex", "cursor-default", "select-none", "hover:bg-neutral-100", "items-center",
            "rounded", "px-2", "py-1.5", "text-sm", "outline-none", "transition-colors"
        };

        if (Disabled)
        {
            classes.AddRange(new[] { "pointer-events-none", "opacity-50" });
            output.Attributes.SetAttribute("data-disabled", "true");
        }

        AddCssClasses(output, classes.ToArray());

        if (!string.IsNullOrEmpty(Href))
        {
            output.Attributes.SetAttribute("href", Href);
        }

        var iconContent = !string.IsNullOrEmpty(Icon) ? CreateIcon(GetIconContent(Icon), "mr-2 w-4 h-4") : null;
        var submenuIcon = HasSubmenu ? CreateIcon(Icons.ChevronRight, "ml-auto w-4 h-4") : null;

        var itemHtml = $"""
            {iconContent}
            <span>{EscapeJavaScriptString(Text)}</span>
            {(!string.IsNullOrEmpty(Shortcut) ? $"""<span class="ml-auto text-xs tracking-widest opacity-60">{EscapeJavaScriptString(Shortcut)}</span>""" : "")}
            {submenuIcon}
            """;

        output.Content.SetHtmlContent(itemHtml);
    }

    private string GetIconContent(string iconName)
    {
        return iconName switch
        {
            "user" => Icons.User,
            "settings" => Icons.Settings,
            "copy" => Icons.Copy,
            "external-link" => Icons.ExternalLink,
            "plus" => Icons.Plus,
            "check" => Icons.Check,
            "close" => Icons.Close,
            _ => Icons.Info
        };
    }
}

public class DropdownMenuContext
{
    public List<DropdownMenuItem> Items { get; set; } = new();
}

public class DropdownMenuItem
{
    public string Text { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Shortcut { get; set; }
    public string? Href { get; set; }
    public bool IsSeparator { get; set; }
    public bool Disabled { get; set; }
}