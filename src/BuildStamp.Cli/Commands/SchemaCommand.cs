// ============================================================================
// File:        SchemaCommand.cs
// Project:     Solcogito.BuildStamp.Cli
// Version:     0.9.1
// ----------------------------------------------------------------------------
// Description:
//     Prints the BuildStamp template token schema.
//     Supports --pretty, quiet, and verbose.
// ============================================================================

using System;
using System.Text.Json;

using Solcogito.Common.ArgForge;

namespace Solcogito.BuildStamp.Cli;

internal static class SchemaCommand
{
    public static int Run(ArgResult args, string? _configPath, bool quiet, bool verbose)
    {
        bool pretty = args.HasFlag("pretty");

        var tokens = new[]
        {
            new {Name="PROJECT", Description="Project name (string)."},
            new {Name="VERSION", Description="Version string (SemVer or override)."},
            new {Name="BRANCH", Description="Current git branch name."},
            new {Name="COMMIT", Description="Current git commit short SHA."},
            new {Name="TIMESTAMP", Description="Build timestamp (UTC, ISO 8601)."},
            new {Name="TAGS", Description="Comma-separated tags."},
            new {Name="TAGS_JSON", Description="JSON array of tags."},
            new {Name="TAGS_CS", Description="C# string[] initializer of tags."},
            new {Name="NAMESPACE", Description="Target namespace for C# output."},
            new {Name="CLASS", Description="Target class name for C# output."},
            new {Name="ATTR_FLAG", Description="\"1\" if assembly attributes are emitted, else \"0\"."}
        };

        // Verbose mode header
        if (!quiet && verbose)
        {
            Console.WriteLine("[INFO] Printing BuildStamp template token schema.");
            Console.WriteLine();
        }

        // Quiet mode → only raw schema, no banners
        if (quiet)
        {
            if (pretty)
            {
                var obj = new { tokens };
                string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                foreach (var t in tokens)
                    Console.WriteLine($"{t.Name} {t.Description}");
            }
            return 0;
        }

        // Normal mode and pretty mode
        if (pretty)
        {
            var obj = new { tokens };
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
        }
        else
        {
            Console.WriteLine("Token       Description");
            Console.WriteLine("----------- ------------------------------------------------");
            foreach (var t in tokens)
                Console.WriteLine($"{t.Name,-12} {t.Description}");
        }

        // Verbose footer
        if (verbose)
        {
            Console.WriteLine();
            Console.WriteLine("[INFO] Done.");
        }

        return 0;
    }
}
