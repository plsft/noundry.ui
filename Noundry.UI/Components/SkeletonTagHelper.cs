using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-skeleton")]
public class SkeletonTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Background color class
    /// </summary>
    public string BackgroundColor { get; set; } = "bg-gray-300";

    /// <summary>
    /// Animation class for skeleton effect
    /// </summary>
    public string AnimationClass { get; set; } = "animate-pulse";

    /// <summary>
    /// Height of the skeleton element
    /// </summary>
    public string Height { get; set; } = "h-4";

    /// <summary>
    /// Width of the skeleton element
    /// </summary>
    public string Width { get; set; } = "auto";

    /// <summary>
    /// Border radius class
    /// </summary>
    public string BorderRadius { get; set; } = "rounded";

    /// <summary>
    /// Skeleton variant (text, circle, rectangle, avatar)
    /// </summary>
    public string Variant { get; set; } = "text";

    /// <summary>
    /// Number of skeleton lines (for text variant)
    /// </summary>
    public int Lines { get; set; } = 1;

    /// <summary>
    /// Whether to show different line widths (for text variant)
    /// </summary>
    public bool VariableWidth { get; set; } = true;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        if (Variant == "text" && Lines > 1)
        {
            output.TagName = "div";
            AddCssClasses(output, "space-y-2");

            if (!string.IsNullOrEmpty(CssClass))
            {
                output.Attributes.AppendInClassValue(CssClass);
            }

            var textLines = new List<string>();
            for (int i = 0; i < Lines; i++)
            {
                var lineWidth = VariableWidth && i == Lines - 1 ? "w-3/4" : "w-full";
                var combinedClasses = GetCombinedClasses(lineWidth);
                textLines.Add($@"<div class=""{combinedClasses}"" style=""width: {(VariableWidth && i == Lines - 1 ? "75%" : Width)};""></div>");
            }

            output.Content.SetHtmlContent(string.Join("\n", textLines));
        }
        else
        {
            output.TagName = "div";
            
            var combinedClasses = GetCombinedClasses();
            var inlineStyle = GetInlineStyle();

            output.Attributes.SetAttribute("class", combinedClasses);
            if (!string.IsNullOrEmpty(inlineStyle))
            {
                output.Attributes.SetAttribute("style", inlineStyle);
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                output.Attributes.AppendInClassValue(CssClass);
            }
        }
    }

    private string GetCombinedClasses(string? overrideWidth = null)
    {
        var classes = new List<string>
        {
            AnimationClass,
            BackgroundColor,
            Height
        };

        // Add variant-specific classes
        switch (Variant)
        {
            case "circle":
                classes.AddRange(new[] { "rounded-full", "aspect-square" });
                break;
            case "avatar":
                classes.AddRange(new[] { "rounded-full", "w-12", "h-12" });
                break;
            case "rectangle":
                classes.Add(BorderRadius);
                break;
            default: // text
                classes.Add(BorderRadius);
                break;
        }

        if (!string.IsNullOrEmpty(overrideWidth))
        {
            classes.Add(overrideWidth);
        }

        return string.Join(" ", classes);
    }

    private string GetInlineStyle()
    {
        var styles = new List<string>();

        if (Variant != "avatar" && !Width.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
            styles.Add($"width: {Width}");
        }

        return string.Join("; ", styles);
    }
}

[HtmlTargetElement("noundry-skeleton-container")]
public class SkeletonContainerTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Container layout class
    /// </summary>
    public string ContainerClass { get; set; } = "flex flex-col space-y-3";

    /// <summary>
    /// Whether to show loading state
    /// </summary>
    public bool Loading { get; set; } = true;

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();

        output.TagName = "div";

        var classes = new List<string>();
        
        if (!string.IsNullOrEmpty(ContainerClass))
        {
            classes.AddRange(ContainerClass.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        if (classes.Any())
        {
            AddCssClasses(output, classes.ToArray());
        }

        if (Loading)
        {
            output.Content.SetHtmlContent(content);
        }
        else
        {
            // If not loading, suppress skeleton content
            output.Content.Clear();
        }
    }
}

[HtmlTargetElement("noundry-skeleton-text")]
public class SkeletonTextTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Number of text lines
    /// </summary>
    public int Lines { get; set; } = 3;

    /// <summary>
    /// Height of each line
    /// </summary>
    public string LineHeight { get; set; } = "h-4";

    /// <summary>
    /// Whether to vary line widths
    /// </summary>
    public bool VariableWidth { get; set; } = true;

    /// <summary>
    /// Animation class
    /// </summary>
    public string AnimationClass { get; set; } = "animate-pulse";

    /// <summary>
    /// Background color
    /// </summary>
    public string BackgroundColor { get; set; } = "bg-gray-300";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        output.TagName = "div";
        AddCssClasses(output, "space-y-2");

        var textLines = new List<string>();
        for (int i = 0; i < Lines; i++)
        {
            var isLastLine = i == Lines - 1;
            var widthClass = VariableWidth && isLastLine ? "w-3/4" : "w-full";
            
            textLines.Add($@"
                <div class=""{AnimationClass} {BackgroundColor} {LineHeight} {widthClass} rounded""></div>");
        }

        output.Content.SetHtmlContent(string.Join("\n", textLines));
    }
}

[HtmlTargetElement("noundry-skeleton-avatar")]
public class SkeletonAvatarTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Avatar size (sm, md, lg, xl)
    /// </summary>
    public string Size { get; set; } = "md";

    /// <summary>
    /// Animation class
    /// </summary>
    public string AnimationClass { get; set; } = "animate-pulse";

    /// <summary>
    /// Background color
    /// </summary>
    public string BackgroundColor { get; set; } = "bg-gray-300";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        output.TagName = "div";

        var sizeClasses = GetAvatarSizeClasses();
        var classes = new List<string>
        {
            AnimationClass,
            BackgroundColor,
            "rounded-full"
        };

        classes.AddRange(sizeClasses);

        AddCssClasses(output, classes.ToArray());
    }

    private string[] GetAvatarSizeClasses()
    {
        return Size switch
        {
            "sm" => new[] { "w-8", "h-8" },
            "lg" => new[] { "w-16", "h-16" },
            "xl" => new[] { "w-20", "h-20" },
            _ => new[] { "w-12", "h-12" } // md
        };
    }
}

[HtmlTargetElement("noundry-skeleton-card")]
public class SkeletonCardTagHelper : NoundryTagHelperBase
{
    /// <summary>
    /// Whether to show image skeleton
    /// </summary>
    public bool ShowImage { get; set; } = true;

    /// <summary>
    /// Whether to show avatar skeleton
    /// </summary>
    public bool ShowAvatar { get; set; } = true;

    /// <summary>
    /// Number of text lines
    /// </summary>
    public int TextLines { get; set; } = 3;

    /// <summary>
    /// Animation class
    /// </summary>
    public string AnimationClass { get; set; } = "animate-pulse";

    /// <summary>
    /// Background color
    /// </summary>
    public string BackgroundColor { get; set; } = "bg-gray-300";

    /// <summary>
    /// Additional CSS classes
    /// </summary>
    public string? CssClass { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        output.TagName = "div";

        var classes = new List<string>
        {
            "border", "border-gray-200", "rounded-lg", "p-4", "space-y-4"
        };

        if (!string.IsNullOrEmpty(CssClass))
        {
            classes.Add(CssClass);
        }

        AddCssClasses(output, classes.ToArray());

        var cardContent = new List<string>();

        // Image skeleton
        if (ShowImage)
        {
            cardContent.Add($@"
                <div class=""{AnimationClass} {BackgroundColor} h-48 w-full rounded""></div>");
        }

        // Header with avatar and text
        if (ShowAvatar)
        {
            cardContent.Add($@"
                <div class=""flex items-center space-x-3"">
                    <div class=""{AnimationClass} {BackgroundColor} w-10 h-10 rounded-full""></div>
                    <div class=""flex-1 space-y-2"">
                        <div class=""{AnimationClass} {BackgroundColor} h-4 w-3/4 rounded""></div>
                        <div class=""{AnimationClass} {BackgroundColor} h-3 w-1/2 rounded""></div>
                    </div>
                </div>");
        }

        // Text lines
        if (TextLines > 0)
        {
            var textSkeletons = new List<string>();
            for (int i = 0; i < TextLines; i++)
            {
                var isLastLine = i == TextLines - 1;
                var widthClass = isLastLine ? "w-2/3" : "w-full";
                textSkeletons.Add($@"<div class=""{AnimationClass} {BackgroundColor} h-4 {widthClass} rounded""></div>");
            }
            
            cardContent.Add($@"
                <div class=""space-y-2"">
                    {string.Join("\n", textSkeletons)}
                </div>");
        }

        output.Content.SetHtmlContent(string.Join("\n", cardContent));
    }
}