using System.Reflection;
using System.Threading.Tasks;
using Sandbox.Generator.V1;
using Xunit;

namespace Sandbox.Generator.Tests.v1
{
    public sealed class MinimalGeneratorTests
    {
        private readonly Assembly[] _referencedExtensionAssemblies;

        public MinimalGeneratorTests()
        {
            _referencedExtensionAssemblies = [];
        }

        [Fact]
        public async Task PrintTopLevelNamespacesTest()
        {
            var inputCode = @"
using System;
namespace MyNamespaceB
{
    public class BarClassB
    {
        public void MyMethodB1() { }
    }
}
".Replace("'", "\"\"");

            var expectedGeneratedFileName = "MyMinimalGeneratedFile.g.cs";
            string expectedOutput = $$"""
// <auto-generated/>
// Printing only top level namespaces in the compilation
// Total Namespaces 3
// MyNamespaceB
// Microsoft
// System

""";

            await TestHelpers.RunTestAsync<MinimalSourceGenerator>(
                _referencedExtensionAssemblies,
                inputCode,
                expectedGeneratedFileName,
                expectedOutput);
        }
    }
}