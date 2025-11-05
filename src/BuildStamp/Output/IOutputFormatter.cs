// ============================================================================
// File:        IOutputFormatter.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Interface for output formatters.
// ============================================================================

namespace Solcogito.BuildStamp.Output;

/// <summary>
/// Defines a formatter for producing build metadata in a specific output format.
/// </summary>
public interface IOutputFormatter
{
    /// <summary>Gets the file extension used by this format.</summary>
    string Extension { get; }

    /// <summary>Converts BuildInfo into formatted text.</summary>
    string Format(BuildInfo info);
}
