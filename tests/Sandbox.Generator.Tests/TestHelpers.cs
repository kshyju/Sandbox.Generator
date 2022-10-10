using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Generator.Tests
{
    static class TestHelpers
    {
        public static Task RunTestAsync<TSourceGenerator>(
            string expectedFileName,
            string inputSource,
            string expectedOutputSource
            ) where TSourceGenerator : ISourceGenerator, new()
        {
            CSharpSourceGeneratorVerifier<TSourceGenerator>.Test test = new()
            {
                TestState =
                {
                    Sources = { inputSource },
                    GeneratedSources =
                    {
                        (typeof(TSourceGenerator), expectedFileName, SourceText.From(expectedOutputSource, Encoding.UTF8, SourceHashAlgorithm.Sha256)),
                    },
                    AdditionalReferences =
                    {
                        // Durable Task SDK
                        ///typeof(TaskActivityContext).Assembly,
                    },
                },
            };

            return test.RunAsync();
        }

    }
}
