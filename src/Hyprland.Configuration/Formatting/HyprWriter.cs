using System.Text;
using Hyprland.Configuration.Ast;

namespace Hyprland.Configuration.Formatting;

public sealed class HyprWriter
{
    public string Write(HyprDocument hyprDocument)
    {
        var stringBuilder = new StringBuilder();
        foreach (var statement in hyprDocument.Statements)
            WriteStatement(statement, stringBuilder,0);
        return stringBuilder.ToString();
    }

    private void WriteStatement(HyprStatement hyprStatement, StringBuilder stringBuilder, int indent)
    {
        var indentationString = new string (' ', indent * 4);

        switch (hyprStatement)
        {
            case HyprComment c:
                stringBuilder.AppendLine(c.Text);
                break;
            case HyprAssignment a:
                stringBuilder.AppendLine($"{indentationString + a.Key} = {a.RawValue}");
                break;
            case HyprBlock b:
                stringBuilder.AppendLine($"{indentationString}{b.Name} {{");
                foreach (var statement in b.Statements)
                    WriteStatement(statement, stringBuilder, indent + 1);
                stringBuilder.AppendLine($"{indentationString}}}{b.CloseBraceTrailingComment}");
                break;
            default:
                throw new NotSupportedException($"Unknown statement type: {hyprStatement.GetType().FullName}");
        }
    }
}