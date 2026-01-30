//https://raw.githubusercontent.com/hyprwm/hyprland-wiki/refs/heads/main/content/Configuring/Variables.md
using System.Collections.Frozen;

namespace Hypricing.Core.Mods;

public static class ModKindRegistry
{
    public static readonly FrozenDictionary<string, ModKind> NameToKind =
        new Dictionary<string, ModKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["SHIFT"] = ModKind.Shift,
            ["CAPS"] = ModKind.Caps,
            ["CTRL"] = ModKind.Ctrl,
            ["CONTROL"] = ModKind.Ctrl,
            ["ALT"] = ModKind.Alt,
            ["MOD2"] = ModKind.Mod2,
            ["MOD3"] = ModKind.Mod3,
            ["SUPER"] = ModKind.Super,
            ["WIN"] = ModKind.Super,
            ["LOGO"] = ModKind.Super,
            ["MOD4"] = ModKind.Super,
            ["MOD5"] = ModKind.Mod5,
        }.ToFrozenDictionary();

}
