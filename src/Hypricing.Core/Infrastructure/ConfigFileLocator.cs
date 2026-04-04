namespace Hypricing.Core.Infrastructure;

/// <summary>
/// Resolves the path to hyprland.conf using XDG conventions.
/// </summary>
public static class ConfigFileLocator
{
    private const string DefaultRelativePath = ".config/hypr/hyprland.conf";

    /// <summary>
    /// Returns the path to hyprland.conf, or null if the file does not exist.
    /// Resolution order: $HYPRLAND_CONFIG → $XDG_CONFIG_HOME/hypr/hyprland.conf → ~/.config/hypr/hyprland.conf.
    /// </summary>
    public static string? Resolve()
    {
        var envPath = Environment.GetEnvironmentVariable("HYPRLAND_CONFIG");
        if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
            return Path.GetFullPath(envPath);

        var xdgConfig = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (!string.IsNullOrEmpty(xdgConfig))
        {
            var xdgPath = Path.Combine(xdgConfig, "hypr", "hyprland.conf");
            if (File.Exists(xdgPath))
                return Path.GetFullPath(xdgPath);
        }

        var homePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            DefaultRelativePath);

        return File.Exists(homePath) ? Path.GetFullPath(homePath) : null;
    }
}
