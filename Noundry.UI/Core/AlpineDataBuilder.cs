using System.Text.Json;

namespace Noundry.UI.Core;

public class AlpineDataBuilder
{
    private readonly Dictionary<string, object> _data = new();
    private readonly List<string> _methods = new();

    public AlpineDataBuilder AddProperty(string name, object value)
    {
        _data[name] = value;
        return this;
    }

    public AlpineDataBuilder AddBooleanProperty(string name, bool value)
    {
        _data[name] = value;
        return this;
    }

    public AlpineDataBuilder AddStringProperty(string name, string? value)
    {
        _data[name] = value ?? string.Empty;
        return this;
    }

    public AlpineDataBuilder AddMethod(string methodDefinition)
    {
        if (!string.IsNullOrEmpty(methodDefinition))
        {
            _methods.Add(methodDefinition);
        }
        return this;
    }

    public AlpineDataBuilder AddToggleMethod(string propertyName)
    {
        return AddMethod($"{propertyName}Toggle() {{ this.{propertyName} = !this.{propertyName}; }}");
    }

    public AlpineDataBuilder AddSetterMethod(string propertyName, string methodName)
    {
        return AddMethod($"{methodName}(value) {{ this.{propertyName} = value; }}");
    }

    public string Build()
    {
        if (!_data.Any() && !_methods.Any())
        {
            return "{}";
        }

        var parts = new List<string>();

        // Add properties
        foreach (var kvp in _data)
        {
            var serializedValue = kvp.Value switch
            {
                string str => $"'{EscapeJavaScript(str)}'",
                bool b => b.ToString().ToLower(),
                null => "null",
                _ => JsonSerializer.Serialize(kvp.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };
            parts.Add($"{kvp.Key}: {serializedValue}");
        }

        // Add methods
        parts.AddRange(_methods);

        return $"{{ {string.Join(", ", parts)} }}";
    }

    private static string EscapeJavaScript(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t");
    }
}