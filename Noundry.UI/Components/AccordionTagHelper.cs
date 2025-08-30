using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-accordion")]
public class AccordionTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Whether multiple items can be open at once
    /// </summary>
    public bool Multiple { get; set; } = false;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var accordionContext = new AccordionContext();
        context.Items[typeof(AccordionContext)] = accordionContext;

        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddStringProperty("activeAccordion", "")
            .AddMethod("setActiveAccordion(id) { this.activeAccordion = (this.activeAccordion == id) ? '' : id }")
            .Build();

        if (Multiple)
        {
            // For multiple open items, use array instead
            alpineData = new AlpineDataBuilder()
                .AddProperty("activeAccordions", new string[0])
                .AddMethod("toggleAccordion(id) { if(this.activeAccordions.includes(id)) { this.activeAccordions = this.activeAccordions.filter(item => item !== id) } else { this.activeAccordions.push(id) } }")
                .AddMethod("isAccordionActive(id) { return this.activeAccordions.includes(id) }")
                .Build();
        }

        SetAlpineAttribute(output, "data", alpineData);

        var classes = new List<string>
        {
            "relative", "w-full", "mx-auto", "overflow-hidden", "text-sm", "font-normal", 
            "bg-white", "border", "border-gray-200", "divide-y", "divide-gray-200", "rounded-md"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());
        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("noundry-accordion-item")]
public class AccordionItemTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Title of the accordion item
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Whether this item is initially open
    /// </summary>
    public bool Open { get; set; } = false;

    /// <summary>
    /// Additional CSS classes for the item
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var accordionContext = (AccordionContext?)context.Items[typeof(AccordionContext)];
        var isMultiple = accordionContext?.IsMultiple ?? false;

        var content = await output.GetChildContentAsync();
        var itemId = GenerateUniqueId("accordion");

        output.TagName = "div";
        
        var alpineData = new AlpineDataBuilder()
            .AddStringProperty("id", itemId)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "cursor-pointer", "group");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var chevronIcon = CreateIcon(Icons.ChevronDown, "w-4 h-4 duration-200 ease-out");

        var clickHandler = isMultiple ? "toggleAccordion(id)" : "setActiveAccordion(id)";
        var activeCondition = isMultiple ? "isAccordionActive(id)" : "activeAccordion==id";

        var itemHtml = $@"
            <button @click=""{clickHandler}"" class=""flex items-center justify-between w-full p-4 text-left select-none group-hover:underline"">
                <span>{EscapeJavaScriptString(Title)}</span>
                <div :class=""{{ 'rotate-180': {activeCondition} }}"">{chevronIcon}</div>
            </button>
            <div x-show=""{activeCondition}"" x-collapse x-cloak>
                <div class=""p-4 pt-0 opacity-70"">
                    {content}
                </div>
            </div>
            ";

        output.Content.SetHtmlContent(itemHtml);
    }
}

public class AccordionContext
{
    public bool IsMultiple { get; set; } = false;
}