namespace Hypricing.Core.Keywords;

public enum KeywordKind
{
    Unknown = 0,

    // Keywords page
    Exec,
    Execr,
    ExecOnce,
    ExecrOnce,
    ExecShutdown,
    Source,
    Env,
    Envd,
    Device,

    // Monitors
    Monitor,
    MonitorV2,

    // Binds
    Bind,      // bind[flags] is still Bind
    Unbind,
    Submap,

    // Rules
    WindowRule,
    WindowRuleV2, // legacy
    LayerRule,
    WorkspaceRule, // "workspace = ..."

    // Gestures
    Gesture,
}
