// ============================================================================
// File:        BuildInfo.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.7.0
// Description: Shared build metadata model for formatters and templates.
// ============================================================================

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solcogito.BuildStamp.Core
{
    public class BuildInfo
    {
        [JsonPropertyName("project")] public string Project { get; set; } = string.Empty;
        [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
        [JsonPropertyName("branch")] public string Branch { get; set; } = string.Empty;
        [JsonPropertyName("commit")] public string Commit { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")] public string Timestamp { get; set; } = string.Empty;
        [JsonPropertyName("tags")] public List<string>? Tags { get; set; } // new
    }
}
