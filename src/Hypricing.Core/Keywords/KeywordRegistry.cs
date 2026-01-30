using System.Collections.Frozen;

namespace Hypricing.Core.Keywords;

public static class KeywordRegistry
{
    /// <summary>
    /// Maps raw keyword (as seen in config / hyprlang) to KeywordKind.
    /// This is the PRIMARY lookup.
    /// </summary>
    public static readonly FrozenDictionary<string, KeywordKind> NameToKind =
        new Dictionary<string, KeywordKind>(StringComparer.Ordinal)
        {
            // Execution / source / env
            ["exec"] = KeywordKind.Exec,
            ["execr"] = KeywordKind.Execr,
            ["exec-once"] = KeywordKind.ExecOnce,
            ["execr-once"] = KeywordKind.ExecrOnce,
            ["exec-shutdown"] = KeywordKind.ExecShutdown,
            ["source"] = KeywordKind.Source,
            ["env"] = KeywordKind.Env,
            ["envd"] = KeywordKind.Envd,
            ["device"] = KeywordKind.Device,

            // Monitors
            ["monitor"] = KeywordKind.Monitor,
            ["monitorv2"] = KeywordKind.MonitorV2,

            // Binds (base keyword only; flags handled separately)
            ["bind"] = KeywordKind.Bind,
            ["unbind"] = KeywordKind.Unbind,
            ["submap"] = KeywordKind.Submap,

            // Rules
            ["windowrule"] = KeywordKind.WindowRule,
            ["windowrulev2"] = KeywordKind.WindowRuleV2, // legacy
            ["layerrule"] = KeywordKind.LayerRule,
            ["workspace"] = KeywordKind.WorkspaceRule,

            // Gestures
            ["gesture"] = KeywordKind.Gesture,
        }.ToFrozenDictionary();

}
