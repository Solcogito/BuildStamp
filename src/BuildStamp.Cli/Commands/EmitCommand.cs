// ============================================================================
// File:        EmitCommand.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.9.0
// Author:      Benoit Desrosiers (Solcogito S.E.N.C.)
// ----------------------------------------------------------------------------
// Description:
//     Implements the 'emit' command. Supports quiet/verbose modes, strict
//     option validation, optional cleaning, and delegates template-based
//     output generation to BuildInfoEmitter.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;

using Solcogito.BuildStamp.Core.Config;
using Solcogito.BuildStamp.Core.ConfigLayering;
using Solcogito.BuildStamp.Core.Metadata;
using Solcogito.Common.ArgForge;

namespace Solcogito.BuildStamp.Cli;

internal static class EmitCommand
{
    private const int ExitSuccess = 0;
    private const int ExitInvalidArgs = 1;

    public static int Run(ArgResult args, string? cliConfigPath, bool quiet, bool verbose)
    {
        // --------------------------------------------------------------------
        // STEP 1: Strict validation of allowed/disallowed options
        // --------------------------------------------------------------------
        if (!ValidateOptions(args, out var error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] " + error);
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        // --------------------------------------------------------------------
        // STEP 2: Retrieve required option --format (template)
        // NOTE: In v0.9.0 "template" comes from cfg.Format or CLI override
        // --------------------------------------------------------------------
        if (!args.TryGetValue("format", out var templateName) ||
            string.IsNullOrWhiteSpace(templateName))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] Invalid command line arguments.");
            Console.Error.WriteLine("        Missing required option: format");
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        // Verbose validation info
        if (verbose && !quiet)
        {
            Console.WriteLine("[INFO] Validating emit command arguments...");
            Console.WriteLine("[INFO] Template: {0}", templateName);
        }

        // --------------------------------------------------------------------
        // STEP 3: Load and merge configuration from config, env, CLI
        // --------------------------------------------------------------------
        var cfg = BuildStampConfigMerger.LoadAndMerge(cliConfigPath, args);

        if (verbose && !quiet)
        {
            Console.WriteLine("[INFO] Merged configuration loaded.");
        }

        // --------------------------------------------------------------------
        // STEP 4: Emit-specific flag overrides
        // --------------------------------------------------------------------
        if (args.HasFlag("nogit"))
            cfg.IncludeGit = false;

        if (args.HasFlag("notimestamp"))
            cfg.IncludeTimestampUtc = false;

        // --------------------------------------------------------------------
        // STEP 5: Resolve core values with fallbacks
        // --------------------------------------------------------------------
        var outputPath = cfg.OutputPath ?? "./Builds/BuildInfo.cs";
        var format = string.IsNullOrWhiteSpace(cfg.Format) ? "cs" : cfg.Format.ToLowerInvariant();
        var ns = string.IsNullOrWhiteSpace(cfg.Namespace) ? "BuildStamp.Output" : cfg.Namespace;
        var className = string.IsNullOrWhiteSpace(cfg.ClassName) ? "BuildInfo" : cfg.ClassName;

        // --------------------------------------------------------------------
        // STEP 6: Semantic validation
        // --------------------------------------------------------------------
        if (!IsValidFormat(format))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] Invalid format '{0}'. Expected one of: cs, json, md, text.", format);
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] Output path cannot be empty.");
            Console.ResetColor();
            return ExitInvalidArgs;
        }

        // --------------------------------------------------------------------
        // STEP 7: Optional clean mode (remove existing file before generating)
        // --------------------------------------------------------------------
        if (args.HasFlag("clean"))
        {
            TryCleanOutput(outputPath, verbose);
        }

        // --------------------------------------------------------------------
        // STEP 8: Verbose-level diagnostics
        // --------------------------------------------------------------------
        if (verbose && !quiet)
        {
            Console.WriteLine("[INFO] Preparing to emit BuildInfo...");
            Console.WriteLine("[INFO] Output Path: {0}", Path.GetFullPath(outputPath));
            Console.WriteLine("[INFO] Format:      {0}", format);
            Console.WriteLine("[INFO] Namespace:   {0}", ns);
            Console.WriteLine("[INFO] Class:       {0}", className);
            Console.WriteLine("[INFO] Git Enabled: {0}", cfg.IncludeGit ? "yes" : "no");
            Console.WriteLine("[INFO] Timestamp:   {0}", cfg.IncludeTimestampUtc ? "yes" : "no");
        }

        // --------------------------------------------------------------------
        // STEP 9: Emit using BuildInfoEmitter
        // --------------------------------------------------------------------
        var emitter = new BuildInfoEmitter(
            outputPath,
            ns,
            className,
            cfg.EmitAssemblyAttributes,
            cfg.IncludeGit,
            cfg.IncludeTimestampUtc,
            format,
            cfg.Project,
            cfg.VersionOverride,
            cfg.Tags
        );

        var fullPath = emitter.Generate();

        // --------------------------------------------------------------------
        // STEP 10: OK output (quiet suppresses color and prefix)
        // --------------------------------------------------------------------
        if (quiet)
        {
            Console.WriteLine(fullPath);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK] BuildInfo generated -> {0}", fullPath);
            Console.ResetColor();
        }

        return ExitSuccess;
    }

    // =========================================================================
    // Option Validation
    // =========================================================================
    private static bool ValidateOptions(ArgResult args, out string error)
    {
        // Disallowed global-format options for emit
        if (args.TryGetValue("inspectformat", out _))
        {
            error = "Option '--inspect-format' is not valid for the 'emit' command.";
            return false;
        }

        if (args.TryGetValue("resolveformat", out _))
        {
            error = "Option '--resolve-format' is not valid for the 'emit' command.";
            return false;
        }

        if (args.TryGetValue("schemaformat", out _))
        {
            error = "Option '--schema-format' is not valid for the 'emit' command.";
            return false;
        }

        if (args.HasFlag("versiononly"))
        {
            error = "Flag '--version-only' is not valid for the 'emit' command.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    // =========================================================================
    // Helpers
    // =========================================================================
    private static bool IsValidFormat(string format)
    {
        return string.Equals(format, "cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(format, "json", StringComparison.OrdinalIgnoreCase)
            || string.Equals(format, "md", StringComparison.OrdinalIgnoreCase)
            || string.Equals(format, "text", StringComparison.OrdinalIgnoreCase);
    }

    private static void TryCleanOutput(string outputPath, bool verbose)
    {
        try
        {
            var full = Path.GetFullPath(outputPath);
            if (File.Exists(full))
            {
                File.Delete(full);
                if (verbose)
                {
                    Console.WriteLine("[INFO] Removed existing output file: {0}", full);
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine("[WARN] Failed to clean existing output: {0}", ex.Message);
            Console.ResetColor();
        }
    }
}
