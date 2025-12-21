// ============================================================================
// File:        BuildStampResult.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description:
//     Result returned by BuildStampEngine.
// ============================================================================

namespace Solcogito.BuildStamp;

public sealed record BuildStampResult
(
    string Content,
    string FileExtension
);
