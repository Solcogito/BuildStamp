// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.6.0
// Author:      Solcogito S.E.N.C.
// Description: CLI entry point for BuildStamp v0.6.0 — Embedded Metadata API.
// ============================================================================

using System;
using System.IO;
using System.Text.Json;
using Solcogito.BuildStamp.Core.Metadata;

internal static class Program
{
    private const string DefaultConfig = "buildstamp.json";

    private static int Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== BuildStamp CLI v0.6.0 ===");
        Console.ResetColor();

        try
        {
            // Step 1: Load configuration
            var configPath = args.Length > 0 ? args[0] : DefaultConfig;
            if (!File.Exists(configPath))
            {
                Console.Error.WriteLine($"[ERROR] Config not found: {configPath}");
                return 2;
            }

            var json = File.ReadAllText(configPath);
            var cfg = JsonSerializer.Deserialize<BuildStampConfig>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new BuildStampConfig();

            // Step 2: Prepare emitter from config
            var emitter = new BuildInfoEmitter(
                cfg.OutputPath ?? "./Builds/BuildInfo.cs",
                cfg.Namespace ?? "BuildStamp.Output",
                cfg.ClassName ?? "BuildInfo",
                cfg.EmitAssemblyAttributes,
                cfg.IncludeGit,
                cfg.IncludeTimestampUtc
            );

            // Step 3: Generate file
            var outPath = emitter.Generate();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] BuildInfo generated → {outPath}");
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
}

/// <summary>
/// Mirrors the buildstamp.json configuration file.
/// </summary>
public sealed class BuildStampConfig
{
    public string? OutputPath { get; set; }
    public string? Namespace { get; set; }
    public string? ClassName { get; set; }
    public bool EmitAssemblyAttributes { get; set; } = true;
    public bool IncludeGit { get; set; } = true;
    public bool IncludeTimestampUtc { get; set; } = true;
}
