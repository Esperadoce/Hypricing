namespace Hypricing.Core.Statements;

public sealed record RawStatement
{
    public required string RawKeyword { get; init; }  // e.g. "bindd"
    public required string RawValue { get; init; }    // RHS
    public string? SourceFile { get; init; }
    public uint? Line { get; init; }
}
