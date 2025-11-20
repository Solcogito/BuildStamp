// ============================================================================
// File:        InspectCommand.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.9.1
// Description: Implements the 'inspect' command for the new ArgForge v0.2 CLI.
//              Supports:
//                  buildstamp inspect
//                  buildstamp inspect --raw
//                  buildstamp inspect --format json
//                  buildstamp inspect --format text
// ============================================================================

using System;
using System.Text.Json;

using Solcogito.BuildStamp.Core;
using Solcogito.BuildStamp.Core.Config;
using Solcogito.BuildStamp.Core.ConfigLayering;
using Solcogito.Common.ArgForge;
using Solcogito.Common.Versioning;

namespace Solcogito.BuildStamp.Cli;

internal static class InspectCommand
{
    private const int ExitSuccess = 0;
    private const int ExitInvalidArgs = 1;

    public static int Run(ArgResult args, string? cliConfigPath, bool quiet, bool verbose)
    {
        // Ensure no invalid flags/options were passed
        if (!ValidateOptions(args, out var error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] " + error);
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        // Merge config from file + env + CLI overrides
        var cfg = BuildStampConfigMerger.LoadAndMerge(cliConfigPath, args);

        // Resolve format (--format text|json)
        var format = "text";
        if (args.TryGetValue("format", out var fmt) && !string.IsNullOrWhiteSpace(fmt))
            format = fmt.ToLowerInvariant();

        bool diagnostic = args.HasFlag("diagnostic");

        if (diagnostic)
        {
            PrintDiagnostics(cfg);
            return ExitSuccess;
        }

        if (!quiet && verbose)
            Console.WriteLine("[INFO] Inspecting merged configuration.");

        bool raw = args.HasFlag("raw");

        if (raw)
        {
            PrintRaw(cfg);
            return ExitSuccess;
        }

        return format switch
        {
            "json" => PrintJson(cfg),
            "text" => PrintText(cfg, quiet),
            _ => InvalidFormat(format)
        };
    }

    // ------------------------------------------------------------------------
    // Validation: forbid *only* options that belong to other commands.
    // ------------------------------------------------------------------------
    private static bool ValidateOptions(ArgResult args, out string error)
    {
        // `inspect` does NOT generate output → forbid output path override
        if (args.TryGetValue("output", out _))
        {
            error = "Option '--output' is not valid for the 'inspect' command.";
            return false;
        }

        // Template-specific flags belong only to `emit`
        if (args.TryGetValue("template", out _))
        {
            error = "Option '--template' is not valid for 'inspect'.";
            return false;
        }

        if (args.TryGetValue("class", out _))
        {
            error = "Option '--class' is not valid for 'inspect'.";
            return false;
        }

        if (args.TryGetValue("namespace", out _))
        {
            error = "Option '--namespace' is not valid for 'inspect'.";
            return false;
        }

        if (args.TryGetValue("project", out _))
        {
            error = "Option '--project' is not valid for 'inspect'.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    // ------------------------------------------------------------------------
    // Output handlers
    // ------------------------------------------------------------------------

    private static int PrintJson(BuildStampConfig cfg)
    {
        var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);
        return ExitSuccess;
    }

    private static int PrintText(BuildStampConfig cfg, bool quiet)
    {
        if (!quiet)
        {
            Console.WriteLine("BuildStamp Configuration:");
            Console.WriteLine("-------------------------");
        }

        Console.WriteLine("OutputPath:          {0}", cfg.OutputPath ?? "(null)");
        Console.WriteLine("Format:              {0}", cfg.Format ?? "(null)");
        Console.WriteLine("Namespace:           {0}", cfg.Namespace ?? "(null)");
        Console.WriteLine("ClassName:           {0}", cfg.ClassName ?? "(null)");
        Console.WriteLine("EmitAssemblyAttrs:   {0}", cfg.EmitAssemblyAttributes);
        Console.WriteLine("IncludeGit:          {0}", cfg.IncludeGit);
        Console.WriteLine("IncludeTimestampUtc: {0}", cfg.IncludeTimestampUtc);
        Console.WriteLine("Project:             {0}", cfg.Project ?? "(null)");
        Console.WriteLine("VersionOverride:     {0}", cfg.VersionOverride ?? "(null)");
        Console.WriteLine("Tags:                {0}",
            cfg.Tags == null || cfg.Tags.Count == 0 ? "(none)" : string.Join(",", cfg.Tags));

        return ExitSuccess;
    }

    private static void PrintRaw(BuildStampConfig cfg)
    {
        var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);
    }

    private static int InvalidFormat(string format)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("[ERROR] Invalid format '{0}'. Expected 'text' or 'json'.", format);
        Console.ResetColor();
        return ExitInvalidArgs;
    }

    private static void PrintDiagnostics(BuildStampConfig cfg)
    {
        Console.WriteLine("=== DIAGNOSTIC MODE ===");

        // Git info
        var git = GitResolver.TryResolveGitInfo();
        if (git.Exists)
        {
            Console.WriteLine("Repository:      .git FOUND");
            Console.WriteLine("Commit:          {0}", git.ShortCommit);
            Console.WriteLine("Branch:          {0}", git.Branch ?? "(none)");
            Console.WriteLine("Commit Date:     {0}", git.CommitDateUtc?.ToString("O") ?? "(none)");
        }
        else
        {
            Console.WriteLine("Repository:      NOT FOUND");
        }

        Console.WriteLine();

        // Version resolution
        var versionInfo = VersionResolver.ResolveVersionDetailed();

        Console.WriteLine("Version Source:    {0}", versionInfo.Source);
        Console.WriteLine("Version File:      {0}", versionInfo.FilePath ?? "(none)");
        Console.WriteLine("Resolved Version:  {0}", versionInfo.Version.ToString());
        Console.WriteLine("Resolution OK:     {0}", versionInfo.Success ? "yes" : "no");
        Console.WriteLine();

        // Timestamp (forces resolution)
        var ts = TimestampResolver.ResolveUtcTimestamp();
        Console.WriteLine("Timestamp:       {0}", ts.ToString("O"));
        Console.WriteLine();

        // Tags
        if (cfg.Tags != null && cfg.Tags.Count > 0)
            Console.WriteLine("Tags:            {0}", string.Join(",", cfg.Tags));
        else
            Console.WriteLine("Tags:            (none)");

        Console.WriteLine();

        // Config Layers (super important to debug build)
        Console.WriteLine("Config Layers:");
        Console.WriteLine("  ✔ Defaults");
        Console.WriteLine("  ✔ buildstamp.config.json");
        Console.WriteLine("  ✔ Environment");
        Console.WriteLine("  ✔ CLI Args");
    }
}
