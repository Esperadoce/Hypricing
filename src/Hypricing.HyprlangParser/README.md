# HyprlangParser

Standalone library that parses `hyprland.conf` (Hyprlang syntax) into an in-memory AST, allows modification, and serializes back to text. No I/O, no external dependencies.

```csharp
// Parse
var config = HyprlangParser.Parse(File.ReadAllText("hyprland.conf"));

// Read
var section = config.Children.OfType<SectionNode>().First(s => s.Name == "general");
var gaps = section.Children.OfType<AssignmentNode>().First(a => a.Key == "gaps_in");
Console.WriteLine(gaps.Value); // "5"

// Modify
gaps.Value = "8";

// Write back — only the modified line changes, everything else is byte-for-byte identical
File.WriteAllText("hyprland.conf", HyprlangWriter.Write(config));
```

## Supported Syntax

| Node | Example |
|---|---|
| Declaration | `$myvar = SUPER` |
| Assignment | `gaps_in = 5` |
| Keyword | `bind = SUPER,Q,killactive` |
| Section | `general { ... }` / `device:kb { ... }` |
| Exec | `exec-once = [workspace 1 silent] kitty` |
| Source | `source = ~/.config/hypr/keybinds.conf` |
| Comment | `# comment` / `gaps_in = 5 # inline` |
| Raw | anything unrecognized — preserved verbatim |

## Guarantees

- **Round-trip fidelity**: `Write(Parse(text)) == text` for unmodified ASTs
- **No data loss**: unrecognized content becomes a `RawNode` and is written back as-is
- **No I/O**: the caller is responsible for reading and writing files
