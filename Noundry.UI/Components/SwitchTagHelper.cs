using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-switch")]
public class SwitchTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Label text for the switch
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Switch size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Color theme (blue, green, red, gray)
    /// </summary>
    public string Color { get; set; } = "blue";

    /// <summary>
    /// Whether the switch is initially checked
    /// </summary>
    public bool Checked { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var switchId = GetInputId();
        var inputName = GetInputName();
        var isChecked = Checked || (AspFor?.Model as bool? ?? false);

        output.TagName = "div";
        
        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("switchOn", isChecked)
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        AddCssClasses(output, "flex", "items-center", "justify-center", "space-x-2");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var (switchClasses, spanClasses, dimensions) = GetSizeClasses();
        var (bgClasses, labelColorClasses) = GetColorClasses();

        var switchHtml = $"""
            <input id="{switchId}" type="checkbox" name="{inputName}" class="hidden" :checked="switchOn" 
                   {(Disabled ? "disabled" : "")}
                   {(Required ? "required" : "")}/>
            
            <button 
                x-ref="switchButton"
                type="button" 
                @click="switchOn = ! switchOn"
                :class="switchOn ? '{bgClasses.active}' : '{bgClasses.inactive}'" 
                class="{string.Join(" ", switchClasses)}"
                {(Disabled ? "disabled" : "")}
                x-cloak>
                <span :class="switchOn ? '{dimensions.activeTranslate}' : '{dimensions.inactiveTranslate}'" class="{string.Join(" ", spanClasses)}"></span>
            </button>

            {(!string.IsNullOrEmpty(Label) ? $@"
            <label @click=""$refs.switchButton.click(); $refs.switchButton.focus()"" 
                   :class=""{{ '{labelColorClasses.active}': switchOn, '{labelColorClasses.inactive}': ! switchOn }}""
                   class=""text-sm select-none cursor-pointer""
                   x-cloak>
                {EscapeJavaScriptString(Label)}
            </label>
            " : "")}
            """;

        output.Content.SetHtmlContent(switchHtml);
    }

    private (string[] switchClasses, string[] spanClasses, dynamic dimensions) GetSizeClasses()
    {
        return Size switch
        {
            "sm" => (
                new[] { "relative", "inline-flex", "py-0.5", "focus:outline-none", "rounded-full", "w-8", "h-4" },
                new[] { "w-3", "h-3", "duration-200", "ease-in-out", "bg-white", "rounded-full", "shadow-md" },
                new { activeTranslate = "translate-x-4", inactiveTranslate = "translate-x-0.5" }
            ),
            "lg" => (
                new[] { "relative", "inline-flex", "py-1", "focus:outline-none", "rounded-full", "w-14", "h-8" },
                new[] { "w-6", "h-6", "duration-200", "ease-in-out", "bg-white", "rounded-full", "shadow-md" },
                new { activeTranslate = "translate-x-6", inactiveTranslate = "translate-x-1" }
            ),
            _ => ( // md
                new[] { "relative", "inline-flex", "h-6", "py-0.5", "ml-4", "focus:outline-none", "rounded-full", "w-10" },
                new[] { "w-5", "h-5", "duration-200", "ease-in-out", "bg-white", "rounded-full", "shadow-md" },
                new { activeTranslate = "translate-x-[18px]", inactiveTranslate = "translate-x-0.5" }
            )
        };
    }

    private (dynamic bgClasses, dynamic labelColorClasses) GetColorClasses()
    {
        return Color switch
        {
            "green" => (
                new { active = "bg-green-600", inactive = "bg-neutral-200" },
                new { active = "text-green-600", inactive = "text-gray-400" }
            ),
            "red" => (
                new { active = "bg-red-600", inactive = "bg-neutral-200" },
                new { active = "text-red-600", inactive = "text-gray-400" }
            ),
            "gray" => (
                new { active = "bg-gray-600", inactive = "bg-neutral-200" },
                new { active = "text-gray-600", inactive = "text-gray-400" }
            ),
            _ => ( // blue
                new { active = "bg-blue-600", inactive = "bg-neutral-200" },
                new { active = "text-blue-600", inactive = "text-gray-400" }
            )
        };
    }
}