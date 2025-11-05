// ============================================================================
// File:        MarkdownFormatter.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Markdown (.md) output formatter.
// ============================================================================

namespace Solcogito.BuildStamp.Output;

public class MarkdownFormatter : IOutputFormatter
{
    public string Extension => "md";

    public string Format(BuildInfo info)
    {
        return
$@"**Project:** {info.Project}  
**Version:** {info.Version}  
**Branch:** {info.Branch}  
**Commit:** `{info.Commit}`  
**Built:** {info.Timestamp}";
    }
}
