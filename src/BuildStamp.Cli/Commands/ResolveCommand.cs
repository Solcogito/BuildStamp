// ============================================================================
// File:        ResolveCommand.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.9.0
// Description: Implements the 'resolve' command.
// ============================================================================

using System;
using System.Text.Json;

using Solcogito.BuildStamp.Core;
using Solcogito.BuildStamp.Core.Config;
using Solcogito.BuildStamp.Core.ConfigLayering;
using Solcogito.Common.ArgForge;
using Solcogito.Common.Versioning;

namespace Solcogito.BuildStamp.Cli;

internal static class ResolveCommand
{
    private const int ExitSuccess = 0;
    private const int ExitInvalidArgs = 1;

    public static int Run(ArgResult args, string? cliConfigPath, bool quiet, bool verbose)
    {
        // Strict verb-level validation
        if (!ValidateOptions(args, out var error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] " + error);
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        var cfg = BuildStampConfigMerger.LoadAndMerge(cliConfigPath, args);

        if (args.HasFlag("nogit"))
        {
            cfg.IncludeGit = false;
        }

        if (args.HasFlag("notimestamp"))
        {
            cfg.IncludeTimestampUtc = false;
        }

        var format = "text";
        if (args.TryGetValue("resolveformat", out var fmt) && !string.IsNullOrWhiteSpace(fmt))
        {
            format = fmt.ToLowerInvariant();
        }

        bool versionOnly = args.HasFlag("versiononly");

        var info = CreateBuildInfo(cfg);

        if (verbose && !quiet)
        {
            Console.WriteLine("[INFO] Resolved build metadata for project '{0}'", info.Project);
        }

        if (versionOnly)
        {
            Console.WriteLine(info.Version);
            return ExitSuccess;
        }

        if (string.Equals(format, "json", StringComparison.OrdinalIgnoreCase))
        {
            var json = JsonSerializer.Serialize(info, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
        }
        else if (string.Equals(format, "text", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Project:   {0}", info.Project);
            Console.WriteLine("Version:   {0}", info.Version);
            Console.WriteLine("Branch:    {0}", info.Branch);
            Console.WriteLine("Commit:    {0}", info.Commit);
            Console.WriteLine("Timestamp: {0}", info.Timestamp);
            Console.WriteLine("Tags:      {0}", info.Tags == null ? string.Empty : string.Join(",", info.Tags));
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] Invalid resolve format '{0}'. Expected 'text' or 'json'.", format);
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        return ExitSuccess;
    }

    private static bool ValidateOptions(ArgResult args, out string error)
    {
        // Options not valid for resolve
        if (args.TryGetValue("out", out _))
        {
            error = "Option '--out' is not valid for the 'resolve' command.";
            return false;
        }

        if (args.TryGetValue("format", out _))
        {
            error = "Option '--format' is not valid for the 'resolve' command. Use '--resolve-format' instead.";
            return false;
        }

        if (args.TryGetValue("namespace", out _))
        {
            error = "Option '--namespace' is not valid for the 'resolve' command.";
            return false;
        }

        if (args.TryGetValue("classname", out _))
        {
            error = "Option '--class-name' is not valid for the 'resolve' command.";
            return false;
        }

        if (args.TryGetValue("inspectformat", out _))
        {
            error = "Option '--inspect-format' is not valid for the 'resolve' command.";
            return false;
        }

        if (args.TryGetValue("schemaformat", out _))
        {
            error = "Option '--schema-format' is not valid for the 'resolve' command.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static BuildInfo CreateBuildInfo(BuildStampConfig cfg)
    {
        var resolved = VersionResolver.ResolveVersionDetailed();
        var version = string.IsNullOrWhiteSpace(cfg.VersionOverride)
            ? resolved.Version.ToString()
            : cfg.VersionOverride;

        string? commit = cfg.IncludeGit ? TryGit("rev-parse --short HEAD") : null;
        string? branch = cfg.IncludeGit ? TryGit("rev-parse --abbrev-ref HEAD") : null;
        string timestamp = cfg.IncludeTimestampUtc
            ? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            : string.Empty;

        return new BuildInfo
        {
            Project = cfg.Project ?? "BuildStamp",
            Version = version ?? "0.0.0",
            Branch = branch ?? string.Empty,
            Commit = commit ?? string.Empty,
            Timestamp = timestamp,
            Tags = cfg.Tags
        };
    }

    private static string? TryGit(string arguments)
    {
        try
        {
            var p = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            string output = p.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit();
            return string.IsNullOrWhiteSpace(output) ? null : output;
        }
        catch
        {
            return null;
        }
    }
}
