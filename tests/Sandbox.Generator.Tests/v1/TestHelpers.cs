using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using System.Reflection;
using System.Collections.Generic;


namespace Sandbox.Generator.Tests.v1
{
    static class TestHelpers
    {
        public static Task RunTestAsync<TSourceGenerator>(
            IEnumerable<Assembly> extensionAssemblyReferences,
            string inputSource,
            string? expectedFileName,
            string? expectedOutputSource,
            List<DiagnosticResult>? expectedDiagnosticResults = null,
            IDictionary<string, string>? buildPropertiesDictionary = null) where TSourceGenerator : ISourceGenerator, new()
        {
            CSharpSourceGeneratorVerifier<TSourceGenerator>.Test test = new()
            {
                TestState =
                {
                    Sources = { inputSource },
                }
            };

            if (expectedOutputSource != null && expectedFileName != null)
            {
                test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), expectedFileName, SourceText.From(expectedOutputSource, Encoding.UTF8)));
            }

            var config = $@"is_global = true
                            build_property.RootNamespace = TestProject";

            // Add test specific MSBuild properties.
            if (buildPropertiesDictionary is not null)
            {
                foreach (var buildProperty in buildPropertiesDictionary)
                {
                    config += $@"
                                {buildProperty.Key} = {buildProperty.Value}";
                }
            }

            test.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", config));

            foreach (var item in extensionAssemblyReferences)
            {
                test.TestState.AdditionalReferences.Add(item);
            }

            if (expectedDiagnosticResults != null)
            {
                test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnosticResults);
            }

            return test.RunAsync();
        }
    }
}