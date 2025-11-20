// ============================================================================
// File:        BuildStampConfigMerger.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.7.0
// Description: Merges config from file(s), environment, and CLI args.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Solcogito.Common.ArgForge;

using Solcogito.BuildStamp.Core.Config;

namespace Solcogito.BuildStamp.Core.ConfigLayering
{
    public static class BuildStampConfigMerger
    {
        public static BuildStampConfig LoadAndMerge(string? cliConfigPath, ArgResult cliArgs)
        {
            // 1) Base config: buildstamp.json (if present)
            var cfg = new BuildStampConfig();
            var basePath = cliConfigPath ?? "buildstamp.json";
            if (File.Exists(basePath))
            {
                cfg = Deserialize(basePath) ?? cfg;
            }

            // 2) Global overlay: .buildstamp.config.json (if present)
            var globalPath = ".buildstamp.config.json";
            if (File.Exists(globalPath))
            {
                var overlay = Deserialize(globalPath);
                if (overlay != null) cfg = Merge(cfg, overlay);
            }

            // 3) Environment overrides (BUILDSTAMP_*)
            ApplyEnvironmentOverrides(cfg);

            // 4) CLI arguments (highest priority)
            if (cliArgs != null) ApplyCliOverrides(cfg, cliArgs);

            return cfg;
        }

        private static BuildStampConfig? Deserialize(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                return JsonSerializer.Deserialize<BuildStampConfig>(text, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch { return null; }
        }

        private static BuildStampConfig Merge(BuildStampConfig a, BuildStampConfig b)
        {
            // b overwrites non-null values onto a
            a.OutputPath = b.OutputPath ?? a.OutputPath;
            a.Format = b.Format ?? a.Format;
            a.Namespace = b.Namespace ?? a.Namespace;
            a.ClassName = b.ClassName ?? a.ClassName;
            a.EmitAssemblyAttributes = b.EmitAssemblyAttributes;
            a.IncludeGit = b.IncludeGit;
            a.IncludeTimestampUtc = b.IncludeTimestampUtc;
            a.Project = b.Project ?? a.Project;
            a.VersionOverride = b.VersionOverride ?? a.VersionOverride;

            if (b.Tags != null && b.Tags.Count > 0)
            {
                a.Tags ??= new List<string>();
                foreach (var t in b.Tags)
                {
                    if (!a.Tags.Contains(t)) a.Tags.Add(t);
                }
            }
            return a;
        }

        private static void ApplyEnvironmentOverrides(BuildStampConfig cfg)
        {
            string? v;

            v = Environment.GetEnvironmentVariable("BUILDSTAMP_OUT");
            if (!string.IsNullOrWhiteSpace(v)) cfg.OutputPath = v;

            v = Environment.GetEnvironmentVariable("BUILDSTAMP_FORMAT");
            if (!string.IsNullOrWhiteSpace(v)) cfg.Format = v;

            v = Environment.GetEnvironmentVariable("BUILDSTAMP_VERSION");
            if (!string.IsNullOrWhiteSpace(v)) cfg.VersionOverride = v;

            v = Environment.GetEnvironmentVariable("BUILDSTAMP_PROJECT");
            if (!string.IsNullOrWhiteSpace(v)) cfg.Project = v;

            v = Environment.GetEnvironmentVariable("BUILDSTAMP_TAGS");
            if (!string.IsNullOrWhiteSpace(v))
            {
                cfg.Tags ??= new List<string>();
                foreach (var tag in v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!cfg.Tags.Contains(tag)) cfg.Tags.Add(tag);
                }
            }
        }

        private static void ApplyCliOverrides(BuildStampConfig cfg, ArgResult args)
        {
            if (args.TryGetValue("out", out var outPath) && outPath != null)
                cfg.OutputPath = outPath;

            if (args.TryGetValue("format", out var fmt) && fmt != null)
                cfg.Format = fmt;

            if (args.TryGetValue("namespace", out var ns) && ns != null)
                cfg.Namespace = ns;

            if (args.TryGetValue("className", out var cn) && cn != null)
                cfg.ClassName = cn;

            if (args.TryGetValue("project", out var pj) && pj != null)
                cfg.Project = pj;

            if (args.TryGetValue("version", out var vo) && vo != null)
                cfg.VersionOverride = vo;

            // -------------------------
            // Tags (multiple: comma-separated)
            // -------------------------
            if (args.TryGetValue("tags", out var tagsCsv) && !string.IsNullOrWhiteSpace(tagsCsv))
            {
                cfg.Tags ??= new List<string>();

                foreach (var t in tagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!cfg.Tags.Contains(t))
                        cfg.Tags.Add(t);
                }
            }
        }
    }
}
