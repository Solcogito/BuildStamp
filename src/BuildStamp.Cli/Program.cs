// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.9.0
// Description: CLI entry point using ArgForge with verb-based commands.
//              Verbs: emit, inspect, resolve, schema.
// ============================================================================

using System;
using Solcogito.BuildStamp.Cli;
using Solcogito.BuildStamp.Core.ConfigLayering;
using Solcogito.Common.Versioning;
using Solcogito.Common.ArgForge;

internal static class Program
{
    private const int ExitSuccess = 0;
    private const int ExitInvalidArgs = 1;
    private const int ExitRuntimeError = 2;

    private static int Main(string[] args)
    {
        var versionText = VersionResolver.ResolveVersionDetailed().Version.ToString();

        var schema = BuildSchema();
        var argMap = ArgParser.Parse(args, schema);

        // Help always wins if explicitly requested
        if (argMap.HasFlag("help"))
        {
            PrintBanner(versionText);
                PrintUsage(schema);
            return ExitSuccess;
        }

        // CLI version flag: show CLI version and exit 0
        if (argMap.HasFlag("cliversion"))
        {
            PrintVersion(versionText);
            return ExitSuccess;
        }

        // Argument parsing errors (unknown options, wrong arity, etc.)
        if (!argMap.IsValid)
        {
            PrintBanner(versionText);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] Invalid command line arguments.");
            if (!string.IsNullOrWhiteSpace(argMap.Error))
            {
                Console.Error.WriteLine("        " + argMap.Error);
            }
            Console.ResetColor();

            Console.WriteLine();
            PrintUsage(schema);
            return ExitInvalidArgs;
        }

        bool quiet = argMap.HasFlag("quiet");
        bool verbose = argMap.HasFlag("verbose");

        if (!quiet)
        {
            PrintBanner(versionText);
        }

        try
        {
            string? command = argMap.CommandName;

            if (string.IsNullOrWhiteSpace(command))
            {
                Console.Error.WriteLine("[ERROR] Missing command. Expected one of: emit, inspect, merge-config, schema.");
                Console.WriteLine();
                Console.WriteLine(schema.GetHelp("buildstamp"));
                return ExitInvalidArgs;
            }

            // Optional config override for commands that support it
            argMap.TryGetValue("config", out var cliConfigPath);

            switch (command)
            {
                case "emit":
                    return EmitCommand.Run(argMap, cliConfigPath, quiet, verbose);

                case "inspect":
                    return InspectCommand.Run(argMap, cliConfigPath, quiet, verbose);

                case "resolve":
                    return ResolveCommand.Run(argMap, cliConfigPath, quiet, verbose);

                case "schema":
                    return SchemaCommand.Run(argMap, cliConfigPath, quiet, verbose);

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine("[ERROR] Unknown command '{0}'. Expected: emit, inspect, resolve, schema.", command);
                    Console.ResetColor();
                    Console.WriteLine();
                    PrintUsage(schema);
                    return ExitInvalidArgs;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("[ERROR] " + ex.Message);
            Console.ResetColor();
            return ExitRuntimeError;
        }
    }

    // ------------------------------------------------------------------------
    // Banner / usage
    // ------------------------------------------------------------------------

    private static void PrintBanner(string versionText)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== BuildStamp CLI v{0} ===", versionText);
        Console.ForegroundColor = previous;
    }

    private static void PrintVersion(string versionText)
    {
        Console.WriteLine("BuildStamp CLI v{0}", versionText);
    }

    private static void PrintUsage(ArgSchema schema)
    {
        Console.WriteLine(HelpFormatter.FormatHelp(schema, "buildstamp"));
        Console.WriteLine();
    }

    // ------------------------------------------------------------------------
    // ArgForge schema
    // ------------------------------------------------------------------------

    static ArgSchema BuildSchema()
    {
        var schema = ArgSchema.Create("buildstamp", "Embed build metadata into source files");

        // ---------------------------------------------------------------------
        // GLOBAL ROOT FLAGS AND OPTIONS
        // ---------------------------------------------------------------------
        schema.Flag("help", "-h", "--help", "Show help information");
        schema.Flag("quiet", "-q", "--quiet", "Suppress normal output; only raw data or errors are printed.");
        schema.Flag("verbose", "-v", "--verbose", "Enable verbose diagnostics and internal logging.");
        schema.Option("config", "-c", "--config", "Path to BuildStamp config file", requiredFlag: false);
        schema.Option("output", "-o", "--output", "Override output directory", requiredFlag: false);

        // ---------------------------------------------------------------------
        // COMMAND: inspect
        // ---------------------------------------------------------------------
        schema.Command("inspect", "Display resolved build info", cmd =>
        {
            cmd.Flag("raw", "-r", "--raw", "Print raw resolved metadata");
            cmd.Flag("diagnostic", null, "--diagnostic", "Show full resolution details (git, env, config layers)");
            cmd.Option("format", "-f", "--format", "Output format (json|text)", requiredFlag: false)
                .WithChoices("json", "text")
                .WithDefault("text");
        });

        // ---------------------------------------------------------------------
        // COMMAND: schema
        // ---------------------------------------------------------------------
        schema.Command("schema", "Print CLI schema as JSON", cmd =>
        {
            cmd.Flag("pretty", "-p", "--pretty", "Pretty-print schema JSON");
        });

        // ---------------------------------------------------------------------
        // COMMAND: emit
        // ---------------------------------------------------------------------
        schema.Command("emit", "Generate build metadata files", cmd =>
        {
            cmd.Option("template", "-t", "--template", "Template name or path", requiredFlag: true);
            cmd.Option("project", "-p", "--project", "Optional project name override", requiredFlag: false);
            cmd.Option("class", null, "--class", "Class name to generate", requiredFlag: false);
            cmd.Option("namespace", null, "--namespace", "Namespace for generated class", requiredFlag: false);
            cmd.Flag("include-git", null, "--include-git", "Include git metadata");
            cmd.Flag("utc", null, "--utc", "Emit timestamp in UTC");
            cmd.Option("format", "-f", "--format", "Output format (cs|json|md|text)", requiredFlag: false)
                .WithChoices("cs", "json", "md", "text")
                .WithDefault("cs");
        });

        // ---------------------------------------------------------------------
        // COMMAND: merge-config
        // ---------------------------------------------------------------------
        schema.Command("merge-config", "Merge multiple config files", cmd =>
        {
            cmd.Option("input", "-i", "--input", "Input config file(s)", requiredFlag: true)
                    .Multiple();
            cmd.Option("output", "-o", "--output", "Merged output config file", requiredFlag: true);
            cmd.Flag("pretty", "-p", "--pretty", "Pretty-print merged JSON");
        });

        return schema;
    }
}
