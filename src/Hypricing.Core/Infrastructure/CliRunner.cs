using System.Diagnostics;

namespace Hypricing.Core.Infrastructure;

/// <summary>
/// Thin wrapper around Process.Start for executing CLI tools.
/// Virtual methods allow subclassing for tests without reflection.
/// </summary>
public class CliRunner
{
    /// <summary>
    /// Runs a command and returns its stdout content.
    /// Throws <see cref="InvalidOperationException"/> on non-zero exit code.
    /// </summary>
    public virtual async Task<string> RunAsync(string command, string arguments, CancellationToken ct = default)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();

        // Read both streams concurrently to avoid deadlock when a pipe buffer fills
        var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = process.StandardError.ReadToEndAsync(ct);
        await Task.WhenAll(stdoutTask, stderrTask);
        await process.WaitForExitAsync(ct);

        var stdout = stdoutTask.Result;
        var stderr = stderrTask.Result;

        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"'{command} {arguments}' exited with code {process.ExitCode}: {stderr.Trim()}");

        return stdout;
    }
}
