using System.Text.Json;

namespace Hyprland.Configuration;

public sealed class HyprlangValidationService
{
    public sealed record Diagnostic(string Message);

    public Diagnostic[] Validate(string configText)
    {
        IntPtr handle = HyprlangNative.HyprConfigParseText(configText, out var result);

        try
        {
            IntPtr diagPtr = HyprlangNative.HyprConfigGetDiagnosticsJson(handle);
            string? json = HyprlangNative.PtrToUtf8AndFree(diagPtr);

            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<Diagnostic>();

            // JSON shape: [{ "message": "..." }]
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return [new Diagnostic("Invalid diagnostics JSON from hyprlangwrap")];

            var list = new System.Collections.Generic.List<Diagnostic>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("message", out var msg))
                    list.Add(new Diagnostic(msg.GetString() ?? ""));
            }

            return list.ToArray();
        }
        finally
        {
            // Free error string if present
            HyprlangNative.PtrToUtf8AndFree(result.Error);
            HyprlangNative.HyprConfigDestroy(handle);
        }
    }

    public Diagnostic[] ValidateFile(string filePath)
    {
        IntPtr handle = HyprlangNative.HyprConfigParseFile(filePath, out var result);

        try
        {
            IntPtr diagPtr = HyprlangNative.HyprConfigGetDiagnosticsJson(handle);
            string? json = HyprlangNative.PtrToUtf8AndFree(diagPtr);

            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<Diagnostic>();

            // JSON shape: [{ "message": "..." }]
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return [new Diagnostic("Invalid diagnostics JSON from hyprlangwrap")];

            var list = new System.Collections.Generic.List<Diagnostic>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("message", out var msg))
                    list.Add(new Diagnostic(msg.GetString() ?? ""));
            }

            return list.ToArray();
        }
        finally
        {
            // Free error string if present
            HyprlangNative.PtrToUtf8AndFree(result.Error);
            HyprlangNative.HyprConfigDestroy(handle);
        }
    }
}