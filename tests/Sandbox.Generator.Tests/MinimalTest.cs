using System.Threading.Tasks;
using Xunit;

namespace Sandbox.Generator.Tests
{
    public class MinimalGeneratorTests
    {
        [Fact]
        public async Task MinimalClass()
        {
            var code = @"
public class BarClass
{
}
".Replace("'", "\"\"");

        var generatedFileName = "MyGeneratedFile.g.cs";
        var expectedOutput = @"// Auto-generated code
using System;
public class FooClass
{
}
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<MinimalSourceGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}