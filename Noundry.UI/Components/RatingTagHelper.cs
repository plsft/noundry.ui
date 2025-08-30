using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-rating")]
public class RatingTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Maximum number of stars
    /// </summary>
    public int MaxStars { get; set; } = 5;

    /// <summary>
    /// Current rating value
    /// </summary>
    public int Value { get; set; } = 0;

    /// <summary>
    /// Whether the rating is read-only
    /// </summary>
    public bool ReadOnly { get; set; } = false;

    /// <summary>
    /// Star size (sm, md, lg)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Star color (yellow, blue, red, green)
    /// </summary>
    public string Color { get; set; } = "yellow";

    /// <summary>
    /// Whether to show reset button
    /// </summary>
    public bool ShowReset { get; set; } = true;

    /// <summary>
    /// Whether to show rating feedback text
    /// </summary>
    public bool ShowFeedback { get; set; } = true;

    /// <summary>
    /// Label for the rating
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var ratingId = GetInputId();
        var inputName = GetInputName();
        var currentValue = Value;

        if (AspFor?.Model is int modelValue)
        {
            currentValue = modelValue;
        }

        output.TagName = "div";

        var alpineData = new AlpineDataBuilder()
            .AddBooleanProperty("disabled", ReadOnly || Disabled)
            .AddProperty("max_stars", MaxStars)
            .AddProperty("stars", currentValue)
            .AddProperty("value", currentValue)
            .AddMethod("hoverStar(star) { if (this.disabled) return; this.stars = star; }")
            .AddMethod("mouseLeftStar() { if (this.disabled) return; this.stars = this.value; }")
            .AddMethod("rate(star) { if (this.disabled) return; this.stars = star; this.value = star; if (this.$refs.rated) { this.$refs.rated.classList.remove('opacity-0'); setTimeout(() => { this.$refs.rated.classList.add('opacity-0'); }, 2000); } }")
            .AddMethod("reset() { if (this.disabled) return; this.value = 0; this.stars = 0; }")
            .Build();

        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "init", "this.stars = this.value");

        var classes = new List<string> { "relative" };
        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var (starSize, starClasses) = GetStarClasses();
        var starColors = GetStarColors();

        var ratingHtml = $@"
            {(!string.IsNullOrEmpty(Label) ? $@"
            <label class=""block text-sm font-medium text-gray-700 mb-2"">
                {EscapeJavaScriptString(Label)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </label>
            " : "")}
            
            <div class=""flex flex-col items-center max-w-6xl mx-auto justify-center"">
                {(ShowFeedback ? @"
                <div x-ref=""rated"" class=""absolute -mt-2 text-xs font-medium text-gray-900 duration-300 ease-out -translate-y-full opacity-0"">
                    Rated <span x-text=""value""></span> Stars
                </div>
                " : "")}
                
                <ul class=""flex"">
                    <template x-for=""star in max_stars"" :key=""star"">
                        <li @mouseover=""hoverStar(star)"" @mouseleave=""mouseLeftStar"" @click=""rate(star)"" 
                            class=""px-1 cursor-pointer"" 
                            :class=""{{ 'text-gray-400 cursor-not-allowed': disabled }}"">
                            
                            <!-- Empty star -->
                            <svg x-show=""star > stars"" class=""{starSize} text-gray-900"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 256 256"">
                                <rect width=""256"" height=""256"" fill=""none""/>
                                <path d=""M128,189.09l54.72,33.65a8.4,8.4,0,0,0,12.52-9.17l-14.88-62.79,48.7-42A8.46,8.46,0,0,0,224.27,94L160.36,88.8,135.74,29.2a8.36,8.36,0,0,0-15.48,0L95.64,88.8,31.73,94a8.46,8.46,0,0,0-4.79,14.83l48.7,42L60.76,213.57a8.4,8.4,0,0,0,12.52,9.17Z"" 
                                      fill=""none"" stroke=""currentColor"" stroke-linecap=""round"" stroke-linejoin=""round"" stroke-width=""16""/>
                            </svg>
                            
                            <!-- Filled star -->
                            <svg x-show=""star <= stars"" class=""{starSize} {starColors.filled} fill-current"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 256 256"">
                                <rect width=""256"" height=""256"" fill=""none""/>
                                <path d=""M234.29,114.85l-45,38.83L203,211.75a16.4,16.4,0,0,1-24.5,17.82L128,198.49,77.47,229.57A16.4,16.4,0,0,1,53,211.75l13.76-58.07-45-38.83A16.46,16.46,0,0,1,31.08,86l59-4.76,22.76-55.08a16.36,16.36,0,0,1,30.27,0l22.75,55.08,59,4.76a16.46,16.46,0,0,1,9.37,28.86Z""/>
                            </svg>
                        </li>
                    </template>
                </ul>
                
                {(ShowReset && !ReadOnly ? @"
                <button @click=""reset"" class=""inline-flex items-center px-2 py-1 mt-3 text-xs text-white bg-gray-900 rounded-full hover:bg-black hover:text-white"">
                    <svg class=""w-3 h-3 mr-0.5"" xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 256 256"">
                        <rect width=""256"" height=""256"" fill=""none""/>
                        <polyline points=""24 56 24 104 72 104"" fill=""none"" stroke=""currentColor"" stroke-linecap=""round"" stroke-linejoin=""round"" stroke-width=""24""/>
                        <path d=""M67.59,192A88,88,0,1,0,65.77,65.77L24,104"" fill=""none"" stroke=""currentColor"" stroke-linecap=""round"" stroke-linejoin=""round"" stroke-width=""24""/>
                    </svg>
                    <span>Reset</span>
                </button>
                " : "")}
            </div>
            
            <input type=""hidden"" name=""{inputName}"" :value=""value"" />";

        output.Content.SetHtmlContent(ratingHtml);
    }

    private (string starSize, string[] starClasses) GetStarClasses()
    {
        var starSize = Size switch
        {
            "sm" => "w-4 h-4",
            "lg" => "w-8 h-8",
            _ => "w-6 h-6" // md
        };

        var starClasses = new[] { "transition-colors", "duration-150" };

        return (starSize, starClasses);
    }

    private dynamic GetStarColors()
    {
        return Color switch
        {
            "blue" => new { filled = "text-blue-500", empty = "text-gray-300" },
            "red" => new { filled = "text-red-500", empty = "text-gray-300" },
            "green" => new { filled = "text-green-500", empty = "text-gray-300" },
            _ => new { filled = "text-yellow-500", empty = "text-gray-300" } // yellow
        };
    }
}