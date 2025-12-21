// ============================================================================
// File:        BuildStampRequest.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description:
//     Immutable request describing a build stamp operation.
// ============================================================================

namespace Solcogito.BuildStamp;

public sealed record BuildStampRequest
(
    string Project,
    string Version,
    string? Branch,
    string? Commit,
    System.DateTimeOffset Timestamp,
    BuildStampFormat Format
);
