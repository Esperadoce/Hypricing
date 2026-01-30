using System.Text.Json;
using System.Text.Json.Serialization;
using Hyprland.Configuration.Native;

namespace Hyprland.Configuration;

public sealed class HyprlangValidationService
{
    public sealed record Diagnostic([property: JsonPropertyName("message")] string Message);

    public Diagnostic[] Validate(string configText)
    {
        HyprlangWrapper.ParseText(configText);
        var context = HyprlangWrapper.CurrentParseContext;
        if (context.IsError)
        {
            try
            {
                var diagnostics = JsonSerializer.Deserialize<Diagnostic[]>(context.Message);
                if (diagnostics != null)
                {
                    return diagnostics;
                }
            }
            catch (JsonException)
            {
                // Ignore JSON deserialization errors
            }
        }   
        return []; 
    }
}