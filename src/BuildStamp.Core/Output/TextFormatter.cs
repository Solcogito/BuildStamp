// ============================================================================
// File:        TextFormatter.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Plain text output formatter.
// ============================================================================

namespace Solcogito.BuildStamp.Output;
using Solcogito.BuildStamp.Core;

public class TextFormatter : IOutputFormatter
{
    public string Extension => "txt";

    public string Format(BuildInfo info)
    {
        return
$@"Project:   {info.Project}
Version:   {info.Version}
Branch:    {info.Branch}
Commit:    {info.Commit}
Timestamp: {info.Timestamp}";
    }
}
