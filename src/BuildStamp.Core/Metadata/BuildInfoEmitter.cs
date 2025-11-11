// ============================================================================
// File:        BuildInfoEmitter.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.6.0
// Author:      Solcogito S.E.N.C.
// Description: Generates build metadata using Solcogito.Common.Versioning.
// ============================================================================

using System;
using System.IO;
using Solcogito.Common.Versioning;

namespace Solcogito.BuildStamp.Core.Metadata
{
    /// <summary>
    /// Generates build metadata (JSON, CS, etc.) using shared versioning API.
    /// </summary>
    public sealed class BuildInfoEmitter
    {
        private readonly string _outputPath;
        private readonly string _namespace;
        private readonly string _className;
        private readonly bool _emitAssemblyAttributes;
        private readonly bool _includeGit;
        private readonly bool _includeTimestampUtc;

        public BuildInfoEmitter(
            string outputPath,
            string ns,
            string className,
            bool emitAssemblyAttributes = true,
            bool includeGit = true,
            bool includeTimestampUtc = true)
        {
            _outputPath = outputPath;
            _namespace = ns;
            _className = className;
            _emitAssemblyAttributes = emitAssemblyAttributes;
            _includeGit = includeGit;
            _includeTimestampUtc = includeTimestampUtc;
        }

        public string Generate()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[INFO] Using Solcogito.Common.Versioning...");
            Console.ResetColor();

            // 1. Resolve version file path and read version model
            var versionModel = VersionResolver.ResolveVersion();

            // 2. Gather optional metadata
            string? commit = _includeGit ? TryGit("rev-parse --short HEAD") : null;
            string? branch = _includeGit ? TryGit("rev-parse --abbrev-ref HEAD") : null;
            string? timestamp = _includeTimestampUtc ? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") : null;

            string versionStr = versionModel.ToString();

            // 3. Render BuildInfo.cs code
            string code = BuildInfoTemplate.Render(
                _namespace,
                _className,
                versionStr,
                versionModel.Major,
                versionModel.Minor,
                versionModel.Patch,
                versionModel.PreRelease,
                versionModel.Metadata,
                versionStr,
                commit,
                branch,
                timestamp,
                _emitAssemblyAttributes
            );

            // 4. Write file
            var fullPath = Path.GetFullPath(_outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, code);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] BuildInfo generated → {fullPath}");
            Console.ResetColor();

            return fullPath;
        }

        private static string? TryGit(string args)
        {
            try
            {
                var p = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = args,
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
            catch { return null; }
        }
    }
}
