# Plugin Architecture and Sandboxing Strategy

## Context

This project is an open-source application designed as a central management tool for Hyprland and modern Wayland-based environments. The long-term vision is to build a flexible platform where functionality can be extended through external plugins developed by third parties or the community.

To support this vision, the core program must act as a hosting platform for extensions while ensuring stable interaction with Hyprland and Wayland components.

## Goals

* Provide a modular core capable of hosting external plugins.
* Allow plugins to interact with Hyprland and Wayland through well-defined APIs.
* Enable future extensibility without requiring modifications to the core.
* Prepare the architecture for secure execution of third-party code.

## Non-goals (initial versions)

* Full operating-system-level sandboxing in the first release.
* Allowing unrestricted direct system access from plugins.
* Guaranteeing complete isolation for in-process extensions.

## Proposed Architecture

The application is composed of two major parts:

### Core Host

* The core provides the main application logic.
* It exposes a stable Plugin API and SDK.
* It acts as the single controlled interface to Hyprland and Wayland.
* It manages plugin loading, lifecycle, and communication.

### Plugins

* Plugins extend the core functionalities.
* They consume only the public Plugin API.
* They must rely on the core for all Hyprland and Wayland interactions.
* They are treated as untrusted or semi-trusted third-party code.

This separation ensures that Hyprland-specific logic remains centralized, preventing plugins from directly manipulating compositor state without mediation.

## Plugin Interaction Model

Two execution models are considered:

### Phase 1 — In-process Plugins

* Plugins are loaded in the same process as the core.
* Communication occurs through direct API calls.
* This model is simple and performant.
* However, it does not provide true security isolation.

### Phase 2 — Out-of-process Plugins

* Plugins run as separate processes.
* Communication occurs through IPC (Inter-Process Communication).
* The core acts as a privileged broker.
* This model enables real sandboxing using OS-level mechanisms.

Phase 1 enables rapid development and experimentation. Phase 2 introduces strong security boundaries once the plugin ecosystem matures.

## Sandboxing and Security Considerations

A key design objective is limiting the potential impact of malicious or faulty plugins.

### Problem Statement

If plugins execute in the same process and under the same OS privileges as the core, they inherit full access to:

* File system resources.
* Network access.
* Hyprland and Wayland control channels.

In this scenario, restricting filesystem access or system calls cannot be reliably enforced at the language runtime level alone.

### Long-term Sandboxing Strategy

True confinement requires:

* Running plugins out-of-process.
* Applying OS-level sandboxing tools.
* Using restricted IPC channels.
* Defining explicit permission policies for each plugin.

Possible future sandbox technologies include:

* Linux namespaces.
* seccomp filters.
* Flatpak portals.
* Bubblewrap-based isolation.

This introduces additional complexity but provides strong guarantees of stability and security.

## Architectural Tradeoffs

| Aspect             | In-process Plugins | Out-of-process Plugins |
| ------------------ | ------------------ | ---------------------- |
| Complexity         | Low                | High                   |
| Performance        | High               | Medium                 |
| Security Isolation | Weak               | Strong                 |
| Development Speed  | Fast               | Slower                 |
| Future Scalability | Limited            | Excellent              |

The initial implementation will favor in-process plugins for simplicity, while keeping the architecture compatible with future migration to out-of-process execution.

## Decision

The core will be designed as a modular host platform with a stable Plugin API.
Early versions will load plugins in-process.
The API will be designed with future IPC compatibility in mind to allow seamless transition to real sandboxed plugins later.

## Open Questions

* Plugin permission manifest format.
* Plugin signing and distribution.
* IPC protocol definition.
* Standardization of Hyprland interaction endpoints.
* User-facing permission prompts.

---

This document records the initial architectural direction for the plugin system and sandboxing strategy.
