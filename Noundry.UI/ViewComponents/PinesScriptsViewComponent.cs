using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Noundry.UI.Extensions;

namespace Noundry.UI.ViewComponents;

public class NoundryScriptsViewComponent : ViewComponent
{
    private readonly NoundryUIOptions _options;

    public NoundryScriptsViewComponent(IOptions<NoundryUIOptions> options)
    {
        _options = options.Value;
    }

    public IViewComponentResult Invoke()
    {
        var scripts = new List<string>();

        if (_options.IncludeTailwindCSS)
        {
            scripts.Add($"""<script src="{_options.TailwindCSSCdnUrl}"></script>""");
        }

        if (_options.IncludeAlpineJS)
        {
            scripts.Add($"""<script defer src="{_options.AlpineJSCdnUrl}"></script>""");
        }

        // Add Alpine.js plugins that Noundry.UI components require
        var alpinePlugins = new[]
        {
            "https://unpkg.com/@alpinejs/collapse@3.x.x/dist/cdn.min.js", // for accordion
            "https://unpkg.com/@alpinejs/focus@3.x.x/dist/cdn.min.js" // for modals
        };

        if (_options.IncludeAlpineJS)
        {
            scripts.InsertRange(-1, alpinePlugins.Select(plugin => 
                $"""<script defer src="{plugin}"></script>"""));
        }

        var html = string.Join("\n", scripts);
        return new HtmlContentViewComponentResult(new HtmlString(html));
    }
}

public class HtmlContentViewComponentResult : IViewComponentResult
{
    private readonly IHtmlContent _content;

    public HtmlContentViewComponentResult(IHtmlContent content)
    {
        _content = content;
    }

    public void Execute(ViewComponentContext context)
    {
        context.Writer.Write(_content);
    }

    public Task ExecuteAsync(ViewComponentContext context)
    {
        Execute(context);
        return Task.CompletedTask;
    }
}