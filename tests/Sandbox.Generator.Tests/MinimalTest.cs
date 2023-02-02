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
// TryGetValue=True, enablePlaceHolder:True
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<MinimalSourceGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}