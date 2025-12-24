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

    [Theory]
    [InlineData(BuildStampFormat.Json)]
    [InlineData(BuildStampFormat.Text)]
    [InlineData(BuildStampFormat.Markdown)]
    [InlineData(BuildStampFormat.CSharp)]
    public void Output_MustNotContain_CarriageReturn(BuildStampFormat format)
    {
        var result = BuildStampEngine.Run(
            new BuildStampRequest(
                Project: BuildStampGoldenSamples.Project,
                Version: BuildStampGoldenSamples.Version,
                Branch: BuildStampGoldenSamples.Branch,
                Commit: BuildStampGoldenSamples.Commit,
                Timestamp: BuildStampGoldenSamples.Timestamp,
                Format: format));

        result.Content.Should().NotContain("\r",
            "BuildStamp output must use LF only and never CRLF");
    }

    [Fact]
    public void Run_TextFormat_ProducesDeterministicText()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Text));

        result.FileExtension.Should().Be("txt");
        result.Content.Should().Be(BuildStampGoldenSamples.Text);
    }

    [Fact]
    public void Run_JsonFormat_ProducesDeterministicJson()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Json));

        result.FileExtension.Should().Be("json");
        result.Content.Should().Be(BuildStampGoldenSamples.Json);
    }

    [Fact]
    public void Run_MarkdownFormat_ProducesDeterministicMarkdown()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.Markdown));

        result.FileExtension.Should().Be("md");
        result.Content.Should().Be(BuildStampGoldenSamples.Markdown);
    }

    [Fact]
    public void Run_CSharpFormat_ProducesDeterministicCSharp()
    {
        var result = BuildStampEngine.Run(
            CreateRequest(BuildStampFormat.CSharp));

        result.FileExtension.Should().Be("cs");
        result.Content.Should().Be(BuildStampGoldenSamples.CSharp);
    }

    [Fact]
    public void Run_EmptyProject_ThrowsInvalidRequest()
    {
        var request = new BuildStampRequest(
            Project: "",
            Version: "1.0.0",
            Branch: null,
            Commit: null,
            Timestamp: DateTime.UtcNow,
            Format: BuildStampFormat.Text);

        Action act = () => BuildStampEngine.Run(request);

        act.Should().Throw<BuildStampException>()
           .Which.Code.Should().Be(BuildStampErrorCode.InvalidRequest);
    }

    [Fact]
    public void Run_EmptyVersion_ThrowsInvalidRequest()
    {
        var request = new BuildStampRequest(
            Project: "Test",
            Version: "",
            Branch: null,
            Commit: null,
            Timestamp: DateTime.UtcNow,
            Format: BuildStampFormat.Text);

        Action act = () => BuildStampEngine.Run(request);

        act.Should().Throw<BuildStampException>()
           .Which.Code.Should().Be(BuildStampErrorCode.InvalidRequest);
    }

    [Fact]
    public void Run_SameRequestTwice_ReturnsIdenticalResult()
    {
        var request = CreateRequest(BuildStampFormat.Json);

        var result1 = BuildStampEngine.Run(request);
        var result2 = BuildStampEngine.Run(request);

        result1.Content.Should().Be(result2.Content);
        result1.FileExtension.Should().Be(result2.FileExtension);
    }

    [Fact]
    public void Run_Json_EscapesQuotesAndBackslashes()
    {
        var request = new BuildStampRequest(
            Project: "Test\"Project\\Name",
            Version: "1.0\"\\0",
            Branch: "main",
            Commit: "abc",
            Timestamp: BuildStampGoldenSamples.Timestamp,
            Format: BuildStampFormat.Json);

        var result = BuildStampEngine.Run(request);

        result.Content.Should().Contain("\\\"");
        result.Content.Should().Contain("\\\\");
    }

    [Fact]
    public void Run_CSharp_EscapesStringLiterals()
    {
        var request = new BuildStampRequest(
            Project: "Test.Project",
            Version: "1.0\"\\0",
            Branch: "dev",
            Commit: "abc",
            Timestamp: BuildStampGoldenSamples.Timestamp,
            Format: BuildStampFormat.CSharp);

        var result = BuildStampEngine.Run(request);

        result.Content.Should().Contain("\\\"");
        result.Content.Should().Contain("\\\\");
    }

    [Fact]
    public void Run_UnsupportedFormat_Throws()
    {
        var request = CreateRequest((BuildStampFormat)999);

        Action act = () => BuildStampEngine.Run(request);

        act.Should().Throw<BuildStampException>()
           .Which.Code.Should().Be(BuildStampErrorCode.UnsupportedFormat);
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
