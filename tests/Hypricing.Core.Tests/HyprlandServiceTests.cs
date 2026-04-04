using Hypricing.Core.Infrastructure;
using Hypricing.Core.Services;

namespace Hypricing.Core.Tests;

public class HyprlandServiceTests : IDisposable
{
    private readonly string _tempDir;

    public HyprlandServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"hypricing-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public async Task LoadAndReadDeclarations()
    {
        var path = WriteTempConfig("""
            $myvar = SUPER
            $terminal = kitty
            gaps_in = 5
            """);

        var service = CreateService();
        await service.LoadAsync(path);

        var declarations = service.GetDeclarations();

        Assert.Equal(2, declarations.Count);
        Assert.Equal("myvar", declarations[0].Name);
        Assert.Equal("SUPER", declarations[0].Value);
        Assert.Equal("terminal", declarations[1].Name);
        Assert.Equal("kitty", declarations[1].Value);
    }

    [Fact]
    public async Task ModifyAndSaveRoundTrip()
    {
        var path = WriteTempConfig("""
            $myvar = SUPER
            gaps_in = 5
            """);

        var service = CreateService();
        await service.LoadAsync(path);

        var declarations = service.GetDeclarations();
        declarations[0].Value = "ALT";

        await service.SaveAsync();

        // Re-read the file and verify
        var saved = await File.ReadAllTextAsync(path);
        Assert.Contains("$myvar = ALT", saved);
        Assert.Contains("gaps_in = 5", saved);
    }

    [Fact]
    public async Task SaveTriggersHyprctlReload()
    {
        var path = WriteTempConfig("$var = value\n");

        var spy = new SpyCliRunner();
        var service = new HyprlandService(spy);
        await service.LoadAsync(path);

        await service.SaveAsync();

        Assert.Single(spy.Invocations);
        Assert.Equal("hyprctl", spy.Invocations[0].Command);
        Assert.Equal("reload", spy.Invocations[0].Arguments);
    }

    [Fact]
    public async Task EmptyConfigReturnsNoDeclarations()
    {
        var path = WriteTempConfig("""
            general {
                gaps_in = 5
            }
            """);

        var service = CreateService();
        await service.LoadAsync(path);

        var declarations = service.GetDeclarations();
        Assert.Empty(declarations);
    }

    [Fact]
    public void GetDeclarationsBeforeLoadThrows()
    {
        var service = CreateService();
        Assert.Throws<InvalidOperationException>(() => service.GetDeclarations());
    }

    [Fact]
    public async Task SavePreservesUnmodifiedLines()
    {
        const string config = "$myvar = SUPER\n# this is a comment\ngaps_in = 5\n";
        var path = WriteTempConfig(config);

        var service = CreateService();
        await service.LoadAsync(path);

        // Save without modifications
        await service.SaveAsync();

        var saved = await File.ReadAllTextAsync(path);
        Assert.Equal(config, saved);
    }

    [Fact]
    public async Task DeclarationsRefreshAfterSave()
    {
        var path = WriteTempConfig("$myvar = SUPER\n");

        var service = CreateService();
        await service.LoadAsync(path);

        var declarations = service.GetDeclarations();
        declarations[0].Value = "ALT";
        await service.SaveAsync();

        // After save, re-parsed AST should reflect the new value
        var refreshed = service.GetDeclarations();
        Assert.Equal("ALT", refreshed[0].Value);
    }

    [Fact]
    public async Task LoadAndReadEnvironmentVariables()
    {
        var path = WriteTempConfig("""
            env = WAYLAND_DISPLAY,wayland-1
            env = GDK_BACKEND,wayland
            gaps_in = 5
            """);

        var service = CreateService();
        await service.LoadAsync(path);

        var envVars = service.GetEnvironmentVariables();

        Assert.Equal(2, envVars.Count);
        Assert.Equal("env", envVars[0].Keyword);
        Assert.Contains("WAYLAND_DISPLAY", envVars[0].Params);
        Assert.Contains("GDK_BACKEND", envVars[1].Params);
    }

    [Fact]
    public async Task ModifyEnvVarAndSaveRoundTrip()
    {
        var path = WriteTempConfig("env = GDK_BACKEND,wayland\n");

        var service = CreateService();
        await service.LoadAsync(path);

        var envVars = service.GetEnvironmentVariables();
        envVars[0].Params = "GDK_BACKEND,x11";

        await service.SaveAsync();

        var saved = await File.ReadAllTextAsync(path);
        Assert.Contains("env = GDK_BACKEND,x11", saved);
    }

    private string WriteTempConfig(string content)
    {
        var path = Path.Combine(_tempDir, "hyprland.conf");
        File.WriteAllText(path, content);
        return path;
    }

    private HyprlandService CreateService() => new(new SpyCliRunner());

    /// <summary>CliRunner that records invocations for verification.</summary>
    private sealed class SpyCliRunner : CliRunner
    {
        public List<(string Command, string Arguments)> Invocations { get; } = [];

        public override Task<string> RunAsync(string command, string arguments, CancellationToken ct = default)
        {
            Invocations.Add((command, arguments));
            return Task.FromResult(string.Empty);
        }
    }
}
