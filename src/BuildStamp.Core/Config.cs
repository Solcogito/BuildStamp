// ============================================================================
// File:        Config.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Configuration model and loader for BuildStamp.
// Version:     0.5.0
// ============================================================================

using System.Text.Json;

namespace Solcogito.BuildStamp;

/// <summary>
/// Represents user-defined settings loaded from buildstamp.config.json.
/// These values are merged with CLI arguments.
/// </summary>
internal class Config
{
    public string? Format { get; set; }
    public string? Out { get; set; }
    public string? Project { get; set; }
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Loads configuration from buildstamp.config.json if available.
    /// </summary>
    public static Config Load()
	{
		// Search upwards for buildstamp.config.json
		string? dir = Directory.GetCurrentDirectory();
		string? foundPath = null;

		while (dir != null)
		{
			string candidate = Path.Combine(dir, "buildstamp.config.json");
			if (File.Exists(candidate))
			{
				foundPath = candidate;
				break;
			}
			dir = Directory.GetParent(dir)?.FullName;
		}

		if (foundPath == null)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("[WARN] buildstamp.config.json not found; using defaults.");
			Console.ResetColor();
			return new Config();
		}

		try
		{
			var json = File.ReadAllText(foundPath);
			var cfg = JsonSerializer.Deserialize<Config>(
				json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
			);
			Console.WriteLine($"[INFO] Loaded config from {foundPath}");
			return cfg ?? new Config();
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[WARN] Invalid config file ({ex.Message})");
			Console.ResetColor();
			return new Config();
		}
	}

    /// <summary>
    /// Merges CLI arguments over config values.
    /// </summary>
    public void MergeWithArgs(Dictionary<string, string> args)
    {
        if (args.TryGetValue("format", out var fmt)) Format = fmt;
        if (args.TryGetValue("out", out var path)) Out = path;
        if (args.TryGetValue("project", out var proj)) Project = proj;
    }
}
