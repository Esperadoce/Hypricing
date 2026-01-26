using System.Reflection;
using Hyprland.Configuration;
using Hyprland.Configuration.IO;

namespace Hypricing.Cli
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                var first = args[0];
                if (first is "-h" or "--help")
                {
                    PrintHelp();
                    return 0;
                }

                if (first is "-v" or "--version")
                {
                    PrintVersion();
                    return 0;
                }
            }

            var inputPath = GetInputPath(args);
            var outputPath = GetOutputPath();

            try
            {
                var loader = new HyprConfigLoader();

                // Async from sync Main:
                var merged = loader.LoadMergedAsync(inputPath).GetAwaiter().GetResult();

                File.WriteAllText(outputPath, merged.Text);

                Console.WriteLine($"Input : {inputPath}");
                Console.WriteLine($"Output: {outputPath}");
                Console.WriteLine($"Lines : {merged.Text.Length}");

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: {ex.Message}");
                return 1;
            }
        }

        private static string GetDefaultHyprDir()
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrWhiteSpace(home))
                home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            return Path.Combine(home, ".config", "hypr");
        }

        private static string GetInputPath(string[] args)
        {
            if (args.Length >= 2 && args[0] == "--path")
                return args[1];

            if (args.Length >= 1 && args[0] is not "-h" and not "--help" and not "-v" and not "--version")
                return args[0];

            return Path.Combine(GetDefaultHyprDir(), "hyprland.conf");
        }

        private static string GetOutputPath()
        {
            return Path.Combine(GetDefaultHyprDir(), "hyprland.test.conf");
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Hypricing CLI");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  hypricing [--path <path>]");
            Console.WriteLine("  hypricing <path>");
            Console.WriteLine("  hypricing --help");
            Console.WriteLine("  hypricing --version");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --path <path>  Input path to hyprland.conf");
            Console.WriteLine("  -h, --help     Show help");
            Console.WriteLine("  -v, --version  Show version");
        }

        private static void PrintVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";
            Console.WriteLine(version);
        }
    }
}
