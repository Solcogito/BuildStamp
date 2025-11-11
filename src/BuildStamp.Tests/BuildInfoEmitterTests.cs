// ============================================================================
// File:        BuildInfoEmitterTests.cs
// Project:     Solcogito.BuildStamp.Tests
// Version:     0.7.0
// Author:      Solcogito S.E.N.C.
// Description: Verifies built-in emitter generation using default templates.
// ============================================================================

using System;
using System.IO;
using Xunit;
using Solcogito.BuildStamp.Core.Metadata;

namespace Solcogito.BuildStamp.Tests
{
    public class BuildInfoEmitterTests
    {
        [Fact]
        public void Generates_BuildInfo_File()
        {
            CleanupTemplates();

            // Prepare temp output directory
            string outDir = Path.Combine(Path.GetTempPath(), "BuildStampTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "TestBuildInfo.cs");

            var emitter = new BuildInfoEmitter(
                outputPath: outPath,
                ns: "TestNamespace",
                className: "BuildInfo",
                emitAssemblyAttributes: true,
                includeGit: false,
                includeTimestampUtc: false,
                format: "cs",
                project: "TestProject",
                versionOverride: "1.0.0-test",
                tags: new System.Collections.Generic.List<string> { "unittest" }
            );

            string generated = emitter.Generate();

            Assert.True(File.Exists(generated));
            string content = File.ReadAllText(generated);
            Assert.Contains("public const string Version", content);

            // Clean up
            File.Delete(generated);
            CleanupTemplates();
        }

        private static void CleanupTemplates()
        {
            if (Directory.Exists(".buildstamp"))
                Directory.Delete(".buildstamp", recursive: true);
        }
    }
}
