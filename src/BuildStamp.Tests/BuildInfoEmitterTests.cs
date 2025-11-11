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
            string outPath = Path.GetFullPath("./Builds/TestBuildInfo.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

            var emitter = new BuildInfoEmitter(
                outPath, "TestNamespace", "BuildInfo",
                emitAssemblyAttributes: true, includeGit: false, includeTimestampUtc: false
            );

            string generated = emitter.Generate();

            Assert.True(File.Exists(generated));
            string content = File.ReadAllText(generated);
            Assert.Contains("public const string Version", content);
        }
    }
}
