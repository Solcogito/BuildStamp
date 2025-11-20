// ============================================================================
// File:        GitResolver.cs
// Project:     Solcogito.BuildStamp.Core
// Description: Minimal git state resolver (safe for no-git repos)
// ============================================================================

using System;
using System.Diagnostics;
using System.IO;

namespace Solcogito.BuildStamp.Core
{
    public sealed class GitInfo
    {
        public bool Exists { get; init; }
        public string? ShortCommit { get; init; }
        public string? Branch { get; init; }
        public DateTime? CommitDateUtc { get; init; }
    }

    public static class GitResolver
    {
        public static GitInfo TryResolveGitInfo()
        {
            if (!Directory.Exists(".git"))
                return new GitInfo { Exists = false };

            // commit hash
            string? hash = RunGit("rev-parse --short HEAD");
            if (string.IsNullOrWhiteSpace(hash))
                return new GitInfo { Exists = false };

            // branch
            string? branch = RunGit("rev-parse --abbrev-ref HEAD");

            // commit date
            string? dateStr = RunGit("show -s --format=%cI HEAD");
            DateTime? date = null;
            if (DateTime.TryParse(dateStr, out var dt))
                date = dt.ToUniversalTime();

            return new GitInfo
            {
                Exists = true,
                ShortCommit = hash.Trim(),
                Branch = branch?.Trim(),
                CommitDateUtc = date
            };
        }

        private static string? RunGit(string args)
        {
            try
            {
                var psi = new ProcessStartInfo("git", args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                proc?.WaitForExit(500);

                return proc?.StandardOutput.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }
}
