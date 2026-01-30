namespace Hypricing.Core.Mods;

[Flags]
public enum ModKind
{
    None = 0,

    Shift = 1 << 0,
    Caps = 1 << 1,
    Ctrl = 1 << 2,
    Alt = 1 << 3,

    Mod2 = 1 << 4,
    Mod3 = 1 << 5,
    Super = 1 << 6,
    Mod5 = 1 << 7,
}
