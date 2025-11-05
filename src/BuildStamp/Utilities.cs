// ============================================================================
// File:        Utilities.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Helper methods for Git detection and version retrieval.
// ============================================================================

using System.Diagnostics;
using System.Text.Json;

namespace Solcogito.BuildStamp;

/// <summary>
/// Provides helper functions for Git branch/commit detection and
/// reading the version from AutoVersion-compatible files.
/// </summary>
internal static class Utilities
{
    /// <summary>Detects the current Git branch.</summary>
    public static string DetectGitBranch()
    {
        try { return RunGitCommand("rev-parse --abbrev-ref HEAD"); }
        catch { return "unknown"; }
    }

    /// <summary>Detects the current short Git commit hash.</summary>
    public static string DetectGitCommit()
    {
        try { return RunGitCommand("rev-parse --short HEAD"); }
        catch { return "0000000"; }
    }

    /// <summary>Runs a Git command and returns the trimmed output.</summary>
    private static string RunGitCommand(string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var output = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        return string.IsNullOrEmpty(output) ? "unknown" : output;
    }

    /// <summary>Reads the project version from version.json if available.</summary>
    public static string DetectVersion()
    {
        const string path = "./version.json";
        if (!File.Exists(path)) return "0.0.0";

        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            if (doc.RootElement.TryGetProperty("version", out var ver))
                return ver.GetString() ?? "0.0.0";
        }
        catch
        {
            // ignored
        }
        return "0.0.0";
    }
}
