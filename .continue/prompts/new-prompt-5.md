---
name: Native interop boundaries (hyprlangwrap)
---

# Native boundary

`native/hyprlangwrap` exists to integrate hyprlang parsing with C# safely.

# Hard Rules

- C ABI only (`extern "C"`), stable exported symbols.
- Any native strings passed to C# must have explicit ownership and lifetime rules.
- C# must copy native-provided UTF-8 buffers immediately if lifetime is callback-scoped.
- Do not leak C++ types across the boundary.
- Keep interop minimal; prefer moving behavior to managed code.

# Responsibility split

- Native: parse/validate + return structured parse results / diagnostics.
- Managed: semantic model + interpretation + serialization.
