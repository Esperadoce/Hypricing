using System.Net.Http.Headers;
using Hyprland.Configuration.Ast;

namespace Hyprland.Configuration.Parsing;

public sealed class HyprParser
{
    private readonly HyprDocument  _document = new();
    private readonly Stack<HyprBlock>  _stack = new();
    public HyprDocument Parse(string text)
    {
        var reader = new StringReader(text);
        
        string? line;
        for (int lineNumber = 0; (line = reader.ReadLine()) != null; lineNumber++)
        {
            var original = line;
            var trimmed = line.Trim();
            
            //Empty Line
            if (trimmed.Length == 0)
            {   
                AddStatement(new HyprComment(string.Empty), original);
                continue;
            }

            if (trimmed[0] == '#')
            {
                AddStatement(new HyprComment(trimmed), original);
                continue;
            }

            if (trimmed[0] == '}')
            {
                if (_stack.Count == 0)
                    throw new FormatException($"Unmatched '}}' at line {lineNumber}");
                _stack.Pop();
                continue;
            }

            if (trimmed.EndsWith('{'))
            {
                var name = trimmed[..^1].Trim();
                var block =  new HyprBlock(name);
                AddStatement(block, original);
                _stack.Push(block);
                continue;
            }
            
            var index = line.IndexOf('=');
            if (index > 0)
            {
                var key = line[..index].Trim();
                var value = line[(index + 1)..].Trim();
                var assignment = new HyprAssignment(key, value);
                AddStatement(assignment, original);
                continue;
            }
            
            AddStatement(new HyprComment(original), original);
        }
        
        return _document;
    }

    private void AddStatement(HyprStatement statement, string originalLine)
    {
        
        //TODO : Check later.
        statement.LeadingTrivia = string.Empty;
        statement.TrailingTrivia = string.Empty;

        if (_stack.Count > 0)
        {
            _stack.Peek().Statements.Add(statement);
        }
        else
        {
            _document.Statements.Add(statement);
        }
        
    }
}