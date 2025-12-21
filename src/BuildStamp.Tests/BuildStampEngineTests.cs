// ============================================================================
// File:        BuildStampEngineTests.cs
// Project:     Solcogito.BuildStamp.Tests
// Author:      Solcogito S.E.N.C.
// Description:
//     Contract tests for BuildStampEngine.
// ============================================================================

using System;
using FluentAssertions;
using Xunit;

using Solcogito.BuildStamp;

namespace Solcogito.BuildStamp.Tests;

public sealed class BuildStampEngineTests
{
    private static BuildStampRequest CreateRequest(BuildStampFormat format)
    {
        return new BuildStampRequest(
            Project: BuildStampGoldenSamples.Project,
            Version: BuildStampGoldenSamples.Version,
            Branch: BuildStampGoldenSamples.Branch,
            Commit: BuildStampGoldenSamples.Commit,
            Timestamp: BuildStampGoldenSamples.Timestamp,
            Format: format
        );
    }

    [Fact]
    public void Text_Run_ReturnsExpectedOutput()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Text));

        result.FileExtension.Should().Be("txt");
        result.Content.Should().Be(BuildStampGoldenSamples.Text);
    }

    [Fact]
    public void Json_Run_ReturnsExpectedOutput()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Json));

        result.FileExtension.Should().Be("json");
        result.Content.Should().Be(BuildStampGoldenSamples.Json);
    }

    [Fact]
    public void Markdown_Run_ReturnsExpectedOutput()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Markdown));

        result.FileExtension.Should().Be("md");
        result.Content.Should().Be(BuildStampGoldenSamples.Markdown);
    }

    [Fact]
    public void CSharp_Run_ReturnsExpectedOutput()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.CSharp));

        result.FileExtension.Should().Be("cs");
        result.Content.Should().Be(BuildStampGoldenSamples.CSharp);
    }

    [Fact]
    public void NullRequest_ThrowsInvalidRequest()
    {
        Action act = () => BuildStampEngine.Run(null!);

        act.Should().Throw<BuildStampException>()
           .Which.Code.Should().Be(BuildStampErrorCode.InvalidRequest);
    }
}
