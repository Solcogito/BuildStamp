// ============================================================================
// File:        BuildStampException.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description:
//     Base exception type for BuildStamp Core.
// ============================================================================

using System;

namespace Solcogito.BuildStamp;

public sealed class BuildStampException : Exception
{
    public BuildStampErrorCode Code { get; }

    public BuildStampException(
        BuildStampErrorCode code,
        string message)
        : base(message)
    {
        Code = code;
    }
}
