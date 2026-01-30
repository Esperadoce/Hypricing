---
name: Hyprland All-in-One Tool (Parse → Semantic Model → Serialize)
---

# Project Summary

This repository is a single “all-in-one” tool for Hyprland / Wayland workflows.

Core approach:
- Parse Hyprland config files to validate syntactic correctness (syntax-only).
- Implement Hyprland semantics in this project using the official Hyprland documentation as the source of truth (not Hyprland internals).
- For each feature area (input/mouse, monitors, binds, exec/exec-once, window rules, colors, etc.):
  1) Define a typed semantic model (C# records/classes) representing Hyprland meaning
  2) Implement deterministic serialization that emits Hyprland directives preserving that meaning
  3) Add tests with representative snippets and edge cases

# Hard Rules (Must Follow)

## No stringly-typed editing
- Do not implement feature logic by editing raw config strings.
- Do not rely on whitespace, ordering, or formatting as semantics unless Hyprland docs explicitly define it.
- All core behavior must operate on typed semantic models.

## Separation of concerns
- Parsing = syntax only (AST / structured nodes).
- Binding = AST subset → semantic model.
- Serialization = semantic model → Hyprland directives.
- Validation = semantic invariants (independent of parsing).

## Deterministic output
- Serialization must be stable:
  - same semantic model => same output text
  - stable ordering rules (explicitly defined per feature)

## Doc-driven semantics
- When implementing or changing semantics, add:
  - a reference to the relevant Hyprland docs section (in code comment or test)
  - a test that locks the expected behavior
- If docs are ambiguous:
  - state the assumption explicitly
  - write tests that reflect the chosen behavior
  - keep the design flexible to revise later

# Architecture Constraints (Design for Scale)

## Feature modules
Each feature area must have:
- `Model` (typed records/classes)
- `Binder` (AST → Model)
- `Serializer` (Model → directives)
- `Tests` (snippets + edge cases + round-trip equivalence)

Feature modules must be independently testable.

## IPC-friendly APIs (future plugin support)
Even if plugins start in-process, APIs must be designed as if they cross a process boundary:
- no raw pointers
- no process-local handles
- prefer DTO-like models that are serializable and versionable

# Testing Requirements

Minimum per feature:
- Parse validity tests (syntax acceptance/rejection)
- Semantic binding tests (AST → model correctness)
- Serialization tests (model → directives correctness)
- Round-trip semantic equivalence tests:
  - input config → parse → bind → serialize
  - re-parse/re-bind should yield an equivalent semantic model
  - textual equality is not required; semantic equivalence is required

# What the Assistant Should Do In This Repo

When asked to implement/change something:
1) Identify the feature area (input/monitors/binds/exec/window-rules/colors…)
2) Update/extend the semantic model first
3) Update binder and serializer
4) Add/adjust tests for doc-defined behavior and edge cases
5) Keep output deterministic and avoid string hacks

When uncertain about semantics:
- request the relevant Hyprland documentation section (or point to where it is referenced in the repo)
- do not invent behavior silently
