// ============================================================================
// File:        Program.cs
// Project:     Solcogito.BuildStamp.Cli
// Author:      Solcogito S.E.N.C.
// ============================================================================

using System;

using Solcogito.BuildStamp;
using Solcogito.BuildStamp.Output;

namespace Solcogito.BuildStamp.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            // Minimal, explicit CLI parsing (v0.1.0)
            // buildstamp <project> <version> [format]

            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            string project = args[0];
            string version = args[1];
            string formatArg = args.Length >= 3 ? args[2] : "text";

            if (!TryParseFormat(formatArg, out var format))
            {
                Console.Error.WriteLine($"Unknown format: {formatArg}");
                return 1;
            }

            var request = new BuildStampRequest(
                Project: project,
                Version: version,
                Branch: ResolveGitBranch(),
                Commit: ResolveGitCommit(),
                Timestamp: DateTimeOffset.UtcNow,
                Format: format
            );

            BuildStampResult result = BuildStampEngine.Run(request);

            Console.WriteLine(result.Content);
            return 0;
        }
        catch (BuildStampException ex)
        {
            Console.Error.WriteLine($"Error [{ex.Code}]: {ex.Message}");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            return 2;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine(
@"Usage:
  buildstamp <project> <version> [format]

Formats:
  text
  json
  markdown
  csharp");
    }

    private static bool TryParseFormat(string value, out BuildStampFormat format)
    {
        return Enum.TryParse(value, ignoreCase: true, out format);
    }

    // --------------------------------------------------------------------
    // Temporary resolvers (CLI responsibility)
    // --------------------------------------------------------------------

    private static string? ResolveGitBranch()
    {
        return Environment.GetEnvironmentVariable("GIT_BRANCH");
    }

    private static string? ResolveGitCommit()
    {
        return Environment.GetEnvironmentVariable("GIT_COMMIT");
    }
}
