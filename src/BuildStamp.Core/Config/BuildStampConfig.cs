// ============================================================================
// File:        BuildStampConfig.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.7.0
// Description: Configuration model for BuildStamp (file/env/CLI merged).
// ============================================================================

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solcogito.BuildStamp.Core.Config
{
    public sealed class BuildStampConfig
    {
        // Output controls
        [JsonPropertyName("outputPath")] public string? OutputPath { get; set; } // e.g., ./Builds/BuildInfo.cs
        [JsonPropertyName("format")] public string? Format { get; set; }         // json, cs, md, text

        // C# codegen controls
        [JsonPropertyName("namespace")] public string? Namespace { get; set; }
        [JsonPropertyName("className")] public string? ClassName { get; set; } = "BuildInfo";
        [JsonPropertyName("emitAssemblyAttributes")] public bool EmitAssemblyAttributes { get; set; } = true;

        // Metadata toggles
        [JsonPropertyName("includeGit")] public bool IncludeGit { get; set; } = true;
        [JsonPropertyName("includeTimestampUtc")] public bool IncludeTimestampUtc { get; set; } = true;

        // Project identity
        [JsonPropertyName("project")] public string? Project { get; set; }

        // Multi-tag support
        [JsonPropertyName("tags")] public List<string>? Tags { get; set; }

        // Optional explicit version override
        [JsonPropertyName("versionOverride")] public string? VersionOverride { get; set; }
    }
}
