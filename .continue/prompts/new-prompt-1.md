---
name: Repo Overview (Hypricing / Hyprland toolchain)
---

# Repository Layout

- `src/Hyprland.Configuration/`
  - Owns Hyprland config parsing + semantic model + serialization.
  - This is the canonical place for Hyprland config logic.

- `src/Hypricing.Core/`
  - Application core/services.
  - Consumes `Hyprland.Configuration` semantic models.
  - Must not re-implement parsing/serialization logic.

- `src/Hypricing.Cli/`
  - CLI entrypoints and commands.
  - Calls Core services; no Hyprland semantics here.

- `src/Hypricing.View/`
  - UI layer.
  - Displays/edits semantic models via Core; no Hyprland semantics here.

- `native/hyprlangwrap/`
  - Native wrapper around hyprlang (C/C++ ↔ C# interop).
  - Syntax parsing/validation support only (no semantic behavior here).

- `doc/architecture/`
  - Human docs. Keep aligned with the rules in `.continue/rules/`.

# Goal

Build one “all-in-one” Hyprland tool:
- Parse Hyprland config files (syntax correctness).
- Implement Hyprland semantics in managed code using official Hyprland docs as source of truth.
- Represent each feature as a typed semantic model and serialize deterministically to Hyprland directives.
