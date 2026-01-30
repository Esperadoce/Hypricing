# Hypricing: Hyprland Configuration Platform

Hypricing is an all-in-one tool for managing Hyprland and Wayland environments through a unified, extensible platform. It provides robust configuration parsing, semantic modeling, and deterministic formatting with a focus on correctness and extensibility.

## Vision

Build a central management tool for Hyprland that:

- Accurately parses and validates Hyprland configuration syntax
- Implements Hyprland semantics using official documentation as the source of truth
- Provides a stable platform for extending functionality through plugins
- Ensures deterministic output and round-trip semantic equivalence

## Project Structure

### Core Libraries

- **`src/Hyprland.Configuration/`** - The canonical library for Hyprland config processing:
  - AST parsing and syntax validation
  - Semantic models for Hyprland features (input, monitors, binds, etc.)
  - Deterministic serialization to Hyprland directives
  - Semantic validation and binding

- **`src/Hypricing.Core/`** - Application core and services:
  - Orchestrates configuration lifecycle (load → parse → bind → modify → serialize → write)
  - Plugin host and extension management
  - IPC-friendly APIs designed for future out-of-process plugins
  - Consumes semantic models from `Hyprland.Configuration`

### Application Layers

- **`src/Hypricing.Cli/`** - Command-line interface:
  - Entry points and commands for configuration management
  - Calls Core services; no Hyprland semantics implementation
  - Formatting, validation, and migration utilities

- **`src/Hypricing.View/`** - User interface layer:
  - Visual configuration editing and management
  - Displays/edits semantic models via Core services
  - No direct Hyprland semantics implementation

### Native Integration

- **`native/hyprlangwrap/`** - Native wrapper around hyprlang:
  - C/C++ ↔ C# interop for parsing and syntax validation
  - No semantic behavior - only syntax parsing and diagnostics
  - Stable C ABI with explicit ownership and lifetime management

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Hyprland installation (for reference documentation and testing)

## Plugin Architecture

Hypricing is designed as an extensible platform with a plugin system that evolves through two phases:

### Phase 1: In-Process Plugins

- Simple, high-performance direct API calls
- Rapid development and experimentation
- Limited security isolation

### Phase 2: Out-of-Process Plugins (Future)

- Strong security boundaries via OS-level sandboxing
- IPC-based communication
- Permission-based access control

See [Plugin Architecture](./doc/architecture/plugin-system.md) for details.

## Testing Requirements

Every feature must have:

- Parse validity tests (syntax acceptance/rejection)
- Semantic binding tests (AST → model correctness)
- Serialization tests (model → directives correctness)
- Round-trip semantic equivalence tests

Tests should reference Hyprland documentation sections and handle edge cases explicitly.

## License

MIT License. See [LICENSE](./LICENSE) for details.

## Contributing

1. Identify the feature area and update the semantic model first
2. Implement binder and serializer
3. Add tests for doc-defined behavior and edge cases
4. Ensure deterministic output without string manipulation hacks
5. Reference Hyprland documentation sections in code or tests

When uncertain about semantics, document assumptions and keep designs flexible for future revision.
