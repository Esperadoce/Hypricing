namespace Hypricing.Core.Dispatchers;

public sealed record Dispatcher
{
    public DispatcherKind Kind { get; init; } = DispatcherKind.Undefined;

    // Raw string from config (preserve exactly for round-trip / diagnostics)
    public string RawName { get; init; } = "";

    // Canonical name for known dispatchers; null when unknown
    public string? CanonicalName { get; init; }

    public IReadOnlyList<string> Args { get; init; } = [];
    public bool IsKnown => Kind != DispatcherKind.Undefined;

    public static Dispatcher FromName(string name, IReadOnlyList<string>? args = null)
    {
        name ??= string.Empty;

        var kind = DispatcherRegistry.NameToKind.TryGetValue(name, out var k)
            ? k
            : DispatcherKind.Undefined;

        var canonical = (kind != DispatcherKind.Undefined &&
                         DispatcherRegistry.KindToName.TryGetValue(kind, out var cn))
            ? cn
            : null;

        return new Dispatcher
        {
            RawName = name,
            Kind = kind,
            CanonicalName = canonical,
            Args = args ?? []
        };
    }

    public static Dispatcher FromKind(DispatcherKind kind, IReadOnlyList<string>? args = null)
    {
        var canonical = DispatcherRegistry.KindToName.TryGetValue(kind, out var cn)
            ? cn
            : null;

        return new Dispatcher
        {
            RawName = canonical ?? kind.ToString(),
            Kind = canonical is null ? DispatcherKind.Undefined : kind,
            CanonicalName = canonical,
            Args = args ?? []
        };
    }
}
