// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: CLI entry point for BuildStamp v0.4.0 (Multi-format output)
// ============================================================================

using Solcogito.BuildStamp.Output;
using System.Text.Json.Serialization;

namespace Solcogito.BuildStamp;

public class BuildInfo
{
    [JsonPropertyName("project")] public string Project { get; set; } = "BuildStamp";
    [JsonPropertyName("version")] public string Version { get; set; } = Utilities.DetectVersion();
    [JsonPropertyName("branch")]  public string Branch  { get; set; } = Utilities.DetectGitBranch();
    [JsonPropertyName("commit")]  public string Commit  { get; set; } = Utilities.DetectGitCommit();
    [JsonPropertyName("timestamp")] public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
}

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== BuildStamp CLI v0.4.0 ===");
        Console.ResetColor();

        string output = "./Builds/buildinfo.json";
        string format = "json";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--out" && i + 1 < args.Length)
                output = args[i + 1];
            else if (args[i] == "--format" && i + 1 < args.Length)
                format = args[i + 1];
            else if (args[i] is "--help" or "-h")
            {
                PrintHelp();
                return;
            }
        }

        IOutputFormatter formatter = format.ToLower() switch
        {
            "json" => new JsonFormatter(),
            "text" => new TextFormatter(),
            "md"   => new MarkdownFormatter(),
            "cs"   => new CsFormatter(),
            _ => throw new ArgumentException($"Unknown format: {format}")
        };

        var info = new BuildInfo();
        Directory.CreateDirectory(Path.GetDirectoryName(output) ?? ".");
        File.WriteAllText(output, formatter.Format(info));

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK] {format.ToUpper()} build info generated → {output}");
        Console.ResetColor();
    }

    private static void PrintHelp()
    {
        Console.WriteLine(@"
Usage:
  buildstamp [options]

Options:
  --format <type>   Output format: json | text | md | cs
  --out <path>      Output file path
  --help, -h        Show this help message
");
    }
}
