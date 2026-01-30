using Hypricing.Core.Keywords;

namespace Hypricing.Core.Statements;

public sealed record ParsedStatement
{
    public required KeywordKind Kind { get; init; }
    public required string RawKeyword { get; init; }          // e.g. "bindrl"
    public required string? BindFlags { get; init; }          // e.g. "rl"
    public required string RawValue { get; init; }            // whatever hyprlang gives (often already “value”)
    public required uint Line { get; init; }                  // from hyprlang
    public required string SourceFile { get; init; }          // from hyprlang (if available)
}
