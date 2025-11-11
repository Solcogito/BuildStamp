// ============================================================================
// File:        BuildInfoEmitter.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.7.0
// Author:      Solcogito S.E.N.C.
// Description: Generates build metadata using Solcogito.Common.Versioning,
//              template overrides, and multi-tag support.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using Solcogito.Common.Versioning;
using Solcogito.BuildStamp.Core;
using Solcogito.BuildStamp.Core.Templates;

namespace Solcogito.BuildStamp.Core.Metadata
{
    public sealed class BuildInfoEmitter
    {
        private readonly string _outputPath;
        private readonly string _namespace;
        private readonly string _className;
        private readonly bool _emitAssemblyAttributes;
        private readonly bool _includeGit;
        private readonly bool _includeTimestampUtc;
        private readonly string _format; // json, cs, md, text
        private readonly string? _project;
        private readonly string? _versionOverride;
        private readonly List<string>? _tags;

        public BuildInfoEmitter(
            string outputPath,
            string ns,
            string className,
            bool emitAssemblyAttributes,
            bool includeGit,
            bool includeTimestampUtc,
            string format,
            string? project,
            string? versionOverride,
            List<string>? tags)
        {
            _outputPath = outputPath;
            _namespace = ns;
            _className = className;
            _emitAssemblyAttributes = emitAssemblyAttributes;
            _includeGit = includeGit;
            _includeTimestampUtc = includeTimestampUtc;
            _format = format.ToLowerInvariant();
            _project = project;
            _versionOverride = versionOverride;
            _tags = tags;
        }

        public string Generate()
        {
            // 1) Resolve version with optional override
            var resolved = VersionResolver.ResolveVersion();
            var version = string.IsNullOrWhiteSpace(_versionOverride)
                ? resolved.ToString()
                : _versionOverride;

            // 2) Git + timestamp
            string? commit = _includeGit ? TryGit("rev-parse --short HEAD") : null;
            string? branch = _includeGit ? TryGit("rev-parse --abbrev-ref HEAD") : null;
            string timestamp = _includeTimestampUtc ? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") : string.Empty;

            // 3) Build metadata object
            var info = new BuildInfo
            {
                Project = _project ?? "BuildStamp",
                Version = version ?? "0.0.0",
                Branch = branch ?? string.Empty,
                Commit = commit ?? string.Empty,
                Timestamp = timestamp,
                Tags = _tags
            };

            // 4) Check for user templates
            var userTemplate = TemplateEngine.TryLoadUserTemplate(_format);
            string text;
            if (userTemplate != null)
            {
                var tokens = ToTokenMap(info, _namespace, _className, _emitAssemblyAttributes);
                text = TemplateEngine.Render(userTemplate, tokens);
            }
            else
            {
                text = BuiltInRender(info, _format, _namespace, _className, _emitAssemblyAttributes);
            }

            // 5) Write file
            var full = Path.GetFullPath(_outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            File.WriteAllText(full, text);
            return full;
        }

        private static string BuiltInRender(BuildInfo info, string format, string ns, string className, bool emitAttrs)
        {
            return format switch
            {
                "json" => System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }),
                "md" => RenderMd(info),
                "text" => RenderText(info),
                _ => BuildInfoTemplate.Render(ns, className, info.Version,
                                                   Major(info.Version), Minor(info.Version), Patch(info.Version),
                                                   pre: null, buildMeta: null,
                                                   informationalVersion: info.Version,
                                                   commitSha: info.Commit, branch: info.Branch, timestampUtcIso: info.Timestamp,
                                                   emitAssemblyAttributes: emitAttrs,
                                                   tagsCsArray: TemplateEngine.ToCSharpArray(info.Tags))
            };
        }

        private static string RenderMd(BuildInfo i) =>
$@"**Project:** {i.Project}
**Version:** {i.Version}
**Branch:** {i.Branch}
**Commit:** `{i.Commit}`
**Built:** {i.Timestamp}
**Tags:** {(i.Tags == null ? "" : string.Join(", ", i.Tags))}";

        private static string RenderText(BuildInfo i) =>
$@"Project:   {i.Project}
Version:   {i.Version}
Branch:    {i.Branch}
Commit:    {i.Commit}
Timestamp: {i.Timestamp}
Tags:      {(i.Tags == null ? "" : string.Join(",", i.Tags))}";

        private static int Major(string v) => ParsePart(v, 0);
        private static int Minor(string v) => ParsePart(v, 1);
        private static int Patch(string v) => ParsePart(v, 2);

        private static int ParsePart(string v, int idx)
        {
            var s = v.Split('.', '+', '-');
            return (idx < s.Length && int.TryParse(s[idx], out var n)) ? n : 0;
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

        private static IReadOnlyDictionary<string, string?> ToTokenMap(
            BuildInfo info, string ns, string className, bool emitAttrs)
        {
            return new Dictionary<string, string?>
            {
                ["PROJECT"] = info.Project,
                ["VERSION"] = info.Version,
                ["BRANCH"] = info.Branch,
                ["COMMIT"] = info.Commit,
                ["TIMESTAMP"] = info.Timestamp,
                ["TAGS"] = TemplateEngine.ToCommaList(info.Tags),
                ["TAGS_JSON"] = TemplateEngine.ToJsonArray(info.Tags),
                ["TAGS_CS"] = TemplateEngine.ToCSharpArray(info.Tags),
                ["NAMESPACE"] = ns,
                ["CLASS"] = className,
                ["ATTR_FLAG"] = emitAttrs ? "1" : "0"
            };
        }
    }
}
