# Hypricing

Hypricing is a small set of .NET libraries and a CLI for parsing and formatting
Hyprland configuration files.

## Overview

Hypricing provides a simple AST and a line-oriented parser for Hyprland config
syntax, plus a writer that can round-trip a file into a normalized output. The
CLI loads a config file and writes the formatted result to a test output path.

## Projects

- `Hyprland.Configuration`: Parser, AST nodes, and writer for Hyprland config.
- `Hypricing.Cli`: Command-line tool that reads a config and writes it back.
- `Hypricing.Core`: Shared library (currently minimal).

## Usage

Build:

```bash
dotnet build src/Hypricing.Cli/Hypricing.Cli.csproj
```

Run:

```bash
dotnet run --project src/Hypricing.Cli -- --path /path/to/hyprland.conf
```

Defaults:

- Input: `~/.config/hypr/hyprland.conf`
- Output: `~/.config/hypr/hyprland.test.conf`

Options:

- `--path <path>`: Input path to `hyprland.conf`
- `-h`, `--help`: Show help
- `-v`, `--version`: Show version

## Notes

- Blocks are parsed from `name { ... }` and assignments from `key = value`.
- Empty lines and non-matching lines are preserved as comments.
- Output uses 4-space indentation.

## License

MIT. See `LICENSE`.
