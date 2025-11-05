// ============================================================================
// File:        JsonFormatter.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: JSON output formatter.
// ============================================================================

using System.Text.Json;

namespace Solcogito.BuildStamp.Output;

public class JsonFormatter : IOutputFormatter
{
    public string Extension => "json";

    public string Format(BuildInfo info)
    {
        return JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
    }
}
