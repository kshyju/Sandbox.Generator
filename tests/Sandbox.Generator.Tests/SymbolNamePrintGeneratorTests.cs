using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Sandbox.Generator.Tests
{
    public class SymbolNamePrintGeneratorTests
    {
        private readonly Assembly[] _referencedExtensionAssemblies;

        public SymbolNamePrintGeneratorTests()
        {
            var hostingExtension = Assembly.LoadFrom("Microsoft.Extensions.Hosting.dll");
            var diExtension = Assembly.LoadFrom("Microsoft.Extensions.DependencyInjection.dll");
            var hostingAbExtension = Assembly.LoadFrom("Microsoft.Extensions.Hosting.Abstractions.dll");
            var diAbExtension = Assembly.LoadFrom("Microsoft.Extensions.DependencyInjection.Abstractions.dll");
            var dependentAssembly = Assembly.LoadFrom("Sandbox.Generator.SampleClassLibrary.dll");

            _referencedExtensionAssemblies = new[]
            {
                hostingExtension,
                hostingAbExtension,
                diExtension,
                diAbExtension,
                dependentAssembly
            };
        }

        [Fact]
        public async Task MinimalClass()
        {
            var inputCode = @"
using System;
namespace MyNamespaceB
{
    public class BarClassB
    {
        public void MyMethodB1() { }
        public void MyMethodB2() { }
    }
}
namespace MyNamespaceB.MyNamespaceC
{
    public class BarClassBC
    {
        public void MyMethodBc() { }

        public class BarClasBCD
        {
            public void MyMethodBCD() { }
        }
    }
}
".Replace("'", "\"\"");

            var expectedGeneratedFileName = "MyGeneratedFile.g.cs";
            var expectedOutput = @"// Printing methods from all namespaces
// Namespace:MyNamespaceB

".Replace("'", "\"");

            await TestHelpers.RunTestAsync<PrintUsingSymbolVisitorGenerator>(
                _referencedExtensionAssemblies,
                inputCode,
                expectedGeneratedFileName,
                expectedOutput);
        }
    }
}