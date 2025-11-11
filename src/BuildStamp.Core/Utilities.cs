// ============================================================================
// File:        Utilities.cs
// Project:     Solcogito.BuildStamp
// Author:      Solcogito S.E.N.C.
// Description: Central helper utilities for Git metadata and version detection.
// Version:     0.5.5
// ============================================================================

using System.Diagnostics;
using System.Text.Json;

namespace Solcogito.BuildStamp;

/// <summary>
/// Provides helper methods for Git branch / commit detection and
/// AutoVersion-aware version discovery (autoversion.json → version.json → version.txt).
/// </summary>
internal static class Utilities
{
    // ------------------------------------------------------------------------
    // GIT METADATA
    // ------------------------------------------------------------------------

    /// <summary>Detects the current Git branch name.</summary>
    public static string DetectGitBranch()
        => RunGitCommand("rev-parse --abbrev-ref HEAD", "unknown");

    /// <summary>Detects the current short Git commit hash.</summary>
    public static string DetectGitCommit()
        => RunGitCommand("rev-parse --short HEAD", "0000000");

    /// <summary>Executes a Git command and returns its trimmed output.</summary>
    private static string RunGitCommand(string args, string fallback)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            return string.IsNullOrWhiteSpace(output) ? fallback : output;
        }
        catch
        {
            return fallback;
        }
    }

    // ------------------------------------------------------------------------
    // VERSION DETECTION
    // ------------------------------------------------------------------------

    /// <summary>
    /// Determines the project version in priority order:
    /// autoversion.json → version.json → version.txt → 0.0.0 (fallback).
    /// Searches upward from the current working directory.
    /// </summary>
    public static string DetectVersion()
    {
        try
        {
            string? dir = Directory.GetCurrentDirectory();
            string? autoversionPath = null;
            string? versionFilePath = null;
            string? detectedVersion = null;
            int depth = 0;

            // STEP 1: Look for autoversion.json and extract "versionFile" reference
            while (dir != null)
            {
                string candidate = Path.Combine(dir, "autoversion.json");
                if (File.Exists(candidate))
                {
                    autoversionPath = candidate;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[INFO] Found autoversion.json at {autoversionPath}");
                    Console.ResetColor();

                    try
                    {
                        string json = File.ReadAllText(candidate);
                        using var doc = JsonDocument.Parse(json);

                        if (doc.RootElement.TryGetProperty("versionFile", out var vf))
                        {
                            string versionFile = vf.GetString() ?? "version.json";
                            string resolved = Path.Combine(Path.GetDirectoryName(candidate)!, versionFile);

                            if (File.Exists(resolved))
                            {
                                versionFilePath = resolved;
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine($"[INFO] Using version file from AutoVersion config → {versionFilePath}");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"[WARN] versionFile '{versionFile}' not found; falling back to version.json or version.txt");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("[WARN] 'versionFile' not defined in autoversion.json; falling back.");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[WARN] Failed to parse autoversion.json: {ex.Message}");
                        Console.ResetColor();
                    }

                    break; // stop searching upward after first autoversion.json
                }

                dir = Directory.GetParent(dir)?.FullName;
                depth++;
            }

            // STEP 2: If not found or invalid, look for version.json / version.txt manually
            if (versionFilePath == null)
            {
                dir = Directory.GetCurrentDirectory();
                while (dir != null)
                {
                    string jsonCandidate = Path.Combine(dir, "version.json");
                    string txtCandidate = Path.Combine(dir, "version.txt");

                    if (File.Exists(jsonCandidate))
                    {
                        versionFilePath = jsonCandidate;
                        break;
                    }
                    if (File.Exists(txtCandidate))
                    {
                        versionFilePath = txtCandidate;
                        break;
                    }

                    dir = Directory.GetParent(dir)?.FullName;
                }
            }

            // STEP 3: Parse whichever version file we found
            if (versionFilePath != null && File.Exists(versionFilePath))
            {
                try
                {
                    switch (Path.GetExtension(versionFilePath))
                    {
                        case ".json":
                            string json = File.ReadAllText(versionFilePath);
                            using (var doc = JsonDocument.Parse(json))
                            {
                                if (doc.RootElement.TryGetProperty("version", out var ver))
                                {
                                    detectedVersion = ver.GetString();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"[OK] Version detected → {detectedVersion}");
                                    Console.ResetColor();
                                    return detectedVersion ?? "0.0.0";
                                }
                            }
                            break;

                        case ".txt":
                            string txt = File.ReadAllText(versionFilePath).Trim();
                            if (!string.IsNullOrWhiteSpace(txt))
                            {
                                detectedVersion = txt;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[OK] Version detected → {detectedVersion}");
                                Console.ResetColor();
                                return detectedVersion;
                            }
                            break;
                    }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARN] {Path.GetFileName(versionFilePath)} found but missing valid version data.");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARN] Failed to read {versionFilePath}: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARN] No version file found after searching {depth} directories upward.");
                Console.WriteLine("[INFO] Priority order: autoversion.json → version.json → version.txt");
                Console.ResetColor();
            }

            return detectedVersion ?? "0.0.0";
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] Version detection failed: {ex.Message}");
            Console.ResetColor();
            return "0.0.0";
        }
    }
}
