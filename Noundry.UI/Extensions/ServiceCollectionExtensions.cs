using Microsoft.Extensions.DependencyInjection;

namespace Noundry.UI.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Noundry.UI TagHelpers services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNoundryUI(this IServiceCollection services)
    {
        // Register any services that TagHelpers might need
        // For now, this is mainly for future extensibility
        
        return services;
    }

    /// <summary>
    /// Adds Noundry.UI TagHelpers services with configuration to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="configure">An action to configure the Noundry.UI options.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddNoundryUI(this IServiceCollection services, Action<NoundryUIOptions> configure)
    {
        services.Configure(configure);
        return services.AddNoundryUI();
    }
}

public class NoundryUIOptions
{
    /// <summary>
    /// Default theme for components
    /// </summary>
    public string Theme { get; set; } = "default";

    /// <summary>
    /// Whether to include Alpine.js automatically
    /// </summary>
    public bool IncludeAlpineJS { get; set; } = false;

    /// <summary>
    /// Whether to include Tailwind CSS automatically
    /// </summary>
    public bool IncludeTailwindCSS { get; set; } = false;

    /// <summary>
    /// Alpine.js CDN URL
    /// </summary>
    public string AlpineJSCdnUrl { get; set; } = "https://unpkg.com/alpinejs@3.x.x/dist/cdn.min.js";

    /// <summary>
    /// Tailwind CSS CDN URL
    /// </summary>
    public string TailwindCSSCdnUrl { get; set; } = "https://cdn.tailwindcss.com";

    /// <summary>
    /// Custom CSS classes to add to all components
    /// </summary>
    public Dictionary<string, string> CustomClasses { get; set; } = new();

    /// <summary>
    /// Default component sizes
    /// </summary>
    public ComponentDefaults Defaults { get; set; } = new();
}

public class ComponentDefaults
{
    public string ButtonSize { get; set; } = "md";
    public string ButtonVariant { get; set; } = "primary";
    public string AlertType { get; set; } = "info";
    public string BadgeVariant { get; set; } = "default";
    public string ModalMaxWidth { get; set; } = "sm:max-w-lg";
    public string ToastPosition { get; set; } = "top-right";
    public int ToastDelay { get; set; } = 5000;
    public string SwitchSize { get; set; } = "md";
    public string SwitchColor { get; set; } = "blue";
}