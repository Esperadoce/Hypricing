// https://raw.githubusercontent.com/hyprwm/hyprland-wiki/refs/heads/main/content/Configuring/Dispatchers.md
 using System.Collections.Frozen;

namespace Hypricing.Core.Dispatchers;

public static class DispatcherRegistry
{
    public static readonly FrozenDictionary<string, DispatcherKind> NameToKind =
        new Dictionary<string, DispatcherKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["exec"] = DispatcherKind.Exec,
            ["execr"] = DispatcherKind.Execr,
            ["pass"] = DispatcherKind.Pass,
            ["sendshortcut"] = DispatcherKind.SendShortcut,
            ["sendkeystate"] = DispatcherKind.SendKeyState,
            ["killactive"] = DispatcherKind.KillActive,
            ["forcekillactive"] = DispatcherKind.ForceKillActive,
            ["closewindow"] = DispatcherKind.CloseWindow,
            ["killwindow"] = DispatcherKind.KillWindow,
            ["signal"] = DispatcherKind.Signal,
            ["signalwindow"] = DispatcherKind.SignalWindow,
            ["workspace"] = DispatcherKind.Workspace,
            ["movetoworkspace"] = DispatcherKind.MoveToWorkspace,
            ["movetoworkspacesilent"] = DispatcherKind.MoveToWorkspaceSilent,
            ["togglefloating"] = DispatcherKind.ToggleFloating,
            ["setfloating"] = DispatcherKind.SetFloating,
            ["settiled"] = DispatcherKind.SetTiled,
            ["fullscreen"] = DispatcherKind.Fullscreen,
            ["fullscreenstate"] = DispatcherKind.FullscreenState,
            ["dpms"] = DispatcherKind.Dpms,
            ["forceidle"] = DispatcherKind.ForceIdle,
            ["pin"] = DispatcherKind.Pin,
            ["movefocus"] = DispatcherKind.MoveFocus,
            ["movewindow"] = DispatcherKind.MoveWindow,
            ["swapwindow"] = DispatcherKind.SwapWindow,
            ["centerwindow"] = DispatcherKind.CenterWindow,
            ["resizeactive"] = DispatcherKind.ResizeActive,
            ["moveactive"] = DispatcherKind.MoveActive,
            ["resizewindowpixel"] = DispatcherKind.ResizeWindowPixel,
            ["movewindowpixel"] = DispatcherKind.MoveWindowPixel,
            ["cyclenext"] = DispatcherKind.CycleNext,
            ["swapnext"] = DispatcherKind.SwapNext,
            ["tagwindow"] = DispatcherKind.TagWindow,
            ["focuswindow"] = DispatcherKind.FocusWindow,
            ["focusmonitor"] = DispatcherKind.FocusMonitor,
            ["splitratio"] = DispatcherKind.SplitRatio,
            ["movecursortocorner"] = DispatcherKind.MoveCursorToCorner,
            ["movecursor"] = DispatcherKind.MoveCursor,
            ["renameworkspace"] = DispatcherKind.RenameWorkspace,
            ["exit"] = DispatcherKind.Exit,
            ["forcerendererreload"] = DispatcherKind.ForceRendererReload,
            ["movecurrentworkspacetomonitor"] = DispatcherKind.MoveCurrentWorkspaceToMonitor,
            ["focusworkspaceoncurrentmonitor"] = DispatcherKind.FocusWorkspaceOnCurrentMonitor,
            ["moveworkspacetomonitor"] = DispatcherKind.MoveWorkspaceToMonitor,
            ["swapactiveworkspaces"] = DispatcherKind.SwapActiveWorkspaces,
            ["alterzorder"] = DispatcherKind.AlterZOrder,
            ["togglespecialworkspace"] = DispatcherKind.ToggleSpecialWorkspace,
            ["focusurgentorlast"] = DispatcherKind.FocusUrgentOrLast,
            ["togglegroup"] = DispatcherKind.ToggleGroup,
            ["changegroupactive"] = DispatcherKind.ChangeGroupActive,
            ["focuscurrentorlast"] = DispatcherKind.FocusCurrentOrLast,
            ["lockgroups"] = DispatcherKind.LockGroups,
            ["lockactivegroup"] = DispatcherKind.LockActiveGroup,
            ["moveintogroup"] = DispatcherKind.MoveIntoGroup,
            ["moveoutofgroup"] = DispatcherKind.MoveOutOfGroup,
            ["movewindoworgroup"] = DispatcherKind.MoveWindowOrGroup,
            ["movegroupwindow"] = DispatcherKind.MoveGroupWindow,
            ["denywindowfromgroup"] = DispatcherKind.DenyWindowFromGroup,
            ["setignoregrouplock"] = DispatcherKind.SetIgnoreGroupLock,
            ["global"] = DispatcherKind.Global,
            ["submap"] = DispatcherKind.Submap,
            ["event"] = DispatcherKind.Event,
            ["setprop"] = DispatcherKind.SetProp,
            ["toggleswallow"] = DispatcherKind.ToggleSwallow,
        }.ToFrozenDictionary();
    
    public static readonly FrozenDictionary<DispatcherKind, string> KindToName =
        NameToKind
            .GroupBy(kv => kv.Value)
            .ToFrozenDictionary(
                g => g.Key,
                g => g.First().Key
            );
}
