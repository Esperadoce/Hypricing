---
name: Feature Module Contract (scale to many features)
---

# Feature Areas

Examples: input/mouse, monitors, binds, exec/exec-once, window rules, colors.

Each feature must be implemented as an independent module with:

- Model: typed record/class (source of truth)
- Binder: AST → Model
- Serializer: Model → directives
- Tests: snippets + edge cases + doc-driven expectations

# Hard Rules

- Do not add a feature without tests.
- Do not couple one feature’s serializer to another feature’s internal types.
- Prefer small, composable serializers over one mega-serializer.
- Round-trip expectation is semantic equivalence (textual equality not required).

# Determinism

- Define ordering rules explicitly per feature (stable output).
- Avoid emitting directives in "input order" unless Hyprland semantics require it.
