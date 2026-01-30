---
name: Pipeline (Parse → Bind → Serialize)
---

# Core Pipeline

1) Parse (syntax only): text → AST
2) Bind (semantics): AST subset → typed semantic model (records/classes)
3) Serialize: semantic model → Hyprland directives (deterministic output)
4) Validate: semantic invariants independent of parsing

# Hard Rules

- No stringly-typed editing in core logic.
- No feature logic implemented by direct string manipulation of config files.
- Parsing concerns must stay inside `src/Hyprland.Configuration/Parsing` (or equivalent).
- Semantic models must stay inside `src/Hyprland.Configuration` (Model/Features namespaces).
- Serialization must be deterministic: same model → same output.

# Ownership

- `Hyprland.Configuration` owns: AST, binders, models, serializers, semantic validation.
- `Hypricing.Core` orchestrates: load/parse → bind → modify model → serialize → write.
- `CLI/View` only call Core and display/edit models.
