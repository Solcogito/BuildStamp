// ============================================================================
// File:        TemplateParsingTests.cs
// Project:     Solcogito.BuildStamp.Tests
// Version:     0.7.0
// Author:      Solcogito S.E.N.C.
// Description: Validates custom user template parsing and token replacement.
// ============================================================================

using System.IO;
using Xunit;
using Solcogito.BuildStamp.Core.Metadata;

namespace Solcogito.BuildStamp.Tests
{
    public class TemplateParsingTests
    {
        [Fact]
        public void CustomCsTemplate_IsApplied_WhenPresent()
        {
            CleanupTemplates();
            // 1. Create a custom template
            Directory.CreateDirectory(".buildstamp");
            File.WriteAllText(".buildstamp/template.cs.txt",
                "// tpl {PROJECT} {VERSION} {TAGS_JSON}");

            // 2. Prepare isolated output path
            string outDir = Path.Combine(Path.GetTempPath(), "BuildStampTemplateTests", System.Guid.NewGuid().ToString());
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "TestBuildInfo.cs");

            // 3. Use the emitter with override values
            var emitter = new BuildInfoEmitter(
                outputPath: outPath,
                ns: "X",
                className: "Y",
                emitAssemblyAttributes: false,
                includeGit: false,
                includeTimestampUtc: false,
                format: "cs",
                project: "Sample",
                versionOverride: "1.2.3",
                tags: new System.Collections.Generic.List<string> { "nightly", "internal" }
            );

            string generated = emitter.Generate();

            // 4. Validate custom template applied
            Assert.True(File.Exists(generated));
            string text = File.ReadAllText(generated);
            Assert.Contains("tpl Sample 1.2.3 [\"nightly\", \"internal\"]", text);

            // 5. Cleanup
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
