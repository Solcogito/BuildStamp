// ============================================================================
// File:        EmitCommand.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// ============================================================================

using System;

using Solcogito.BuildStamp;
using Solcogito.Common.ArgForge;
using Solcogito.Common.Errors;
using Solcogito.Common.IOKit;
using Solcogito.Common.LogScribe;

namespace Solcogito.BuildStamp.Cli.Commands;

public static class EmitCommand
{
    public static int Execute(ArgResult args, Logger logger)
    {
        try
        {
            string project = args.Positionals[0];
            string version = args.Positionals[1];

            BuildStampFormat format = BuildStampFormat.Json;
            if (args.Options.TryGetValue("format", out string? fmt))
            {
                if (!Enum.TryParse(fmt, true, out format))
                {
                    throw new BuildStampException(
                        BuildStampErrorCode.UnsupportedFormat,
                        $"Unsupported format '{fmt}'.");
                }
            }

            bool quiet = args.Flags.ContainsKey("quiet");

            string outputPath =
                args.Options.TryGetValue("out", out string? outPath)
                    ? outPath!
                    : $"buildinfo.{GetExtension(format)}";

            var request = new BuildStampRequest(
                Project: project,
                Version: version,
                Branch: null,
                Commit: null,
                Timestamp: DateTime.UtcNow,
                Format: format);

            BuildStampResult result = BuildStampEngine.Run(request);

            SafeFile.SafeWriteAllText(outputPath, result.Content);

            if (!quiet)
                logger.Stdout(result.Content);

            return 0;
        }
        catch (BuildStampException ex)
        {
            throw new ErrorException(
                ErrorInfo.Unexpected(
                    new ErrorIdentifier(
                        ErrorCategory.System,
                        new ErrorInstitution("BuildStamp"),
                        "BUILDSTAMP_FAILURE"),
                    ex));
        }
    }

    private static string GetExtension(BuildStampFormat format) =>
        format switch
        {
            BuildStampFormat.Json => "json",
            BuildStampFormat.Text => "txt",
            BuildStampFormat.Markdown => "md",
            BuildStampFormat.CSharp => "cs",
            _ => "out"
        };
}
