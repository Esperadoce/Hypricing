---
name: Doc-driven Hyprland Semantics
---

# Source of Truth

Hyprland official documentation defines semantics.

# Hard Rules

- When implementing/changing semantics, add a reference to the relevant docs section:
  - in a unit test name/comment, or
  - in a code comment near the semantic rule.

- If docs are ambiguous:
  - state the assumption explicitly in code/tests
  - add a test that locks the chosen behavior
  - keep the design flexible to revise later

- Do not use Hyprland internals as the semantic source of truth.
- `native/hyprlangwrap` must not contain semantic behavior; syntax/interop only.
