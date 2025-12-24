// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// ============================================================================

using System;

using Solcogito.BuildStamp.Cli.Commands;
using Solcogito.Common.ArgForge;
using Solcogito.Common.Errors;
using Solcogito.Common.LogScribe;

namespace Solcogito.BuildStamp.Cli;

public static class Program
{
    public static int Run(string[] args)
    {
        Logger logger = CreateLogger();
        ArgSchema schema = BuildSchema();

        if (args.Length == 0 || HasHelpFlag(args))
        {
            logger.Stdout(schema.GetHelp());
            return 0;
        }

        ArgResult parsed = new ArgParser().Parse(schema, args);
        if (!parsed.IsValid)
        {
            logger.Error(parsed.Error!);
            logger.Stdout(schema.GetHelp());
            return 1;
        }

        try
        {
            return EmitCommand.Execute(parsed, logger);
        }
        catch (ErrorException ex)
        {
            logger.Error(ex.Error);
            return 2;
        }
    }

    private static int Main(string[] args) => Run(args);

    // --------------------------------------------------------------------

    private static Logger CreateLogger()
    {
        return new Logger()
            .WithMinimumLevel(LogLevel.Info)
            .WithSink(new ConsoleSink(ConsoleSinkRole.Stdout))
            .WithSink(new ConsoleSink(ConsoleSinkRole.Stderr));
    }

    private static bool HasHelpFlag(string[] args)
    {
        foreach (string arg in args)
        {
            if (arg is "-h" or "--help")
                return true;
        }
        return false;
    }

    private static ArgSchema BuildSchema()
    {
        var schema = ArgSchema.Create(
            "buildstamp",
            "Embed build metadata into your artifacts.");

        schema.Positional(
            "project",
            index: 0,
            description: "Project name",
            required: true);

        schema.Positional(
            "version",
            index: 1,
            description: "Version string",
            required: true);

        schema.Option(
            "format",
            "-f",
            "--format",
            "Output format: json, text, markdown, csharp",
            requiredFlag: false);

        schema.Option(
            "out",
            "-o",
            "--out",
            "Output file path",
            requiredFlag: false);

        schema.Flag(
            "quiet",
            "-q",
            "--quiet",
            "Suppress standard output");

        return schema;
    }
}
