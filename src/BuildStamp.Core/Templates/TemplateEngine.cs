// ============================================================================
// File:        TemplateEngine.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.7.0
// Description: Lightweight template loader and token replacer.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solcogito.BuildStamp.Core.Templates
{
    public static class TemplateEngine
    {
        // Default search order for user templates:
        //  1) ./.buildstamp/template.<ext>.txt (repo root)
        //  2) ./template.<ext>.txt (cwd fallback)
        public static string? TryLoadUserTemplate(string format)
        {
            var candidates = new[]
            {
                Path.GetFullPath($".buildstamp/template.{format}.txt"),
                Path.GetFullPath($"template.{format}.txt")
            };

            foreach (var path in candidates)
            {
                if (File.Exists(path))
                    return File.ReadAllText(path);
            }
            return null;
        }

        // Renders a template by replacing tokens like {PROJECT}, {VERSION}, etc.
        public static string Render(string template, IReadOnlyDictionary<string, string?> tokens)
        {
            // Simple brace token replacement.
            var result = template;
            foreach (var kvp in tokens)
            {
                var token = "{" + kvp.Key + "}";
                result = result.Replace(token, kvp.Value ?? string.Empty, StringComparison.Ordinal);
            }
            return result;
        }

        // Helper for tags array to common representations
        public static string ToCommaList(IEnumerable<string>? tags)
            => tags == null ? string.Empty : string.Join(",", tags);

        public static string ToCSharpArray(IEnumerable<string>? tags)
        {
            if (tags == null) return "new string[0]";
            var escaped = tags.Select(t => $"\"{t.Replace("\"", "\\\"")}\"");
            return $"new string[] {{ {string.Join(", ", escaped)} }}";
        }

        public static string ToJsonArray(IEnumerable<string>? tags)
        {
            if (tags == null) return "[]";
            var escaped = tags.Select(t => $"\"{t.Replace("\"", "\\\"")}\"");
            return $"[{string.Join(", ", escaped)}]";
        }
    }
}
