// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.7.0
// Description: CLI entry point with config/env/CLI merge and templates.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Solcogito.BuildStamp.Core.Config;
using Solcogito.BuildStamp.Core.ConfigLayering;
using Solcogito.BuildStamp.Core.Metadata;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== BuildStamp CLI v0.7.0 ===");
        Console.ResetColor();

        try
        {
            var argMap = ParseArgs(args);
            argMap.TryGetValue("config", out var cliConfigPath);

            var cfg = BuildStampConfigMerger.LoadAndMerge(cliConfigPath, argMap);

            var emitter = new BuildInfoEmitter(
                cfg.OutputPath ?? "./Builds/BuildInfo.cs",
                cfg.Namespace ?? "BuildStamp.Output",
                cfg.ClassName ?? "BuildInfo",
                cfg.EmitAssemblyAttributes,
                cfg.IncludeGit,
                cfg.IncludeTimestampUtc,
                cfg.Format ?? "cs",
                cfg.Project,
                cfg.VersionOverride,
                cfg.Tags
            );

            var outPath = emitter.Generate();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] BuildInfo generated -> {outPath}");
            Console.ResetColor();
            return 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    private static Dictionary<string, string> ParseArgs(string[] args)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--"))
            {
                var key = args[i][2..];
                string? val = (i + 1 < args.Length && !args[i + 1].StartsWith("--")) ? args[++i] : "true";
                dict[key] = val;
            }
        }
        return dict;
    }
}
