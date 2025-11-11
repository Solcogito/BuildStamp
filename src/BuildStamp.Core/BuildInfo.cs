// ============================================================================
// File:        BuildInfo.cs
// Project:     Solcogito.BuildStamp.Core
// Version:     0.6.0
// Author:      Solcogito S.E.N.C.
// Description: Shared build metadata model for formatters and emitter.
// ============================================================================

using System.Text.Json.Serialization;

namespace Solcogito.BuildStamp.Core
{
    /// <summary>
    /// Represents the core metadata used by BuildStamp output formatters.
    /// </summary>
    public class BuildInfo
    {
        [JsonPropertyName("project")] public string Project { get; set; } = string.Empty;
        [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;
        [JsonPropertyName("branch")]  public string Branch  { get; set; } = string.Empty;
        [JsonPropertyName("commit")]  public string Commit  { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")] public string Timestamp { get; set; } = string.Empty;
        [JsonPropertyName("tag")] public string? Tag { get; set; } = null;
    }
}
