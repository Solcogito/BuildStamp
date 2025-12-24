// ============================================================================
// File:        CliProgramTests.cs
// Project:     Solcogito.BuildStamp.Tests
// ============================================================================

using System;
using System.IO;

using FluentAssertions;

using Microsoft.VisualStudio.TestPlatform.TestHost;

using Solcogito.BuildStamp.Cli;

using Xunit;

namespace Solcogito.BuildStamp.Tests;

public sealed class CliProgramTests
{
    [Fact]
    public void Emit_Writes_Default_Json_File()
    {
        using var temp = new TempDirectory();
        string output = Path.Combine(temp.Path, "buildinfo.json");

        int exitCode = Solcogito.BuildStamp.Cli.Program.Run(new[]
        {
            "TestProject",
            "1.2.3",
            "--out", output
        });

        exitCode.Should().Be(0);
        File.Exists(output).Should().BeTrue();

        File.ReadAllText(output)
            .Should().Contain("\"Version\": \"1.2.3\"");
    }

    [Fact]
    public void NoArguments_PrintsHelp()
    {
        int exitCode = Solcogito.BuildStamp.Cli.Program.Run(Array.Empty<string>());
        exitCode.Should().Be(0);
    }

    [Fact]
    public void InvalidFormat_ReturnsErrorCode()
    {
        int exitCode = Solcogito.BuildStamp.Cli.Program.Run(new[]
        {
            "TestProject",
            "1.0.0",
            "--format", "nonsense"
        });

        exitCode.Should().Be(2);
    }
}
