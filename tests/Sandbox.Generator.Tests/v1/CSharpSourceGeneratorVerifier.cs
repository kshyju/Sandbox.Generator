﻿using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.IO;

namespace Sandbox.Generator.Tests.v1
{
    // Mostly copy/pasted from the Microsoft Source Generators testing documentation
    // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#unit-testing-of-generators
    public static class CSharpSourceGeneratorVerifier<TSourceGenerator> where TSourceGenerator : ISourceGenerator, new()
    {
        public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
        {
            public Test()
            {
                // See https://www.nuget.org/packages/Microsoft.NETCore.App.Ref/6.0.0
                ReferenceAssemblies = new ReferenceAssemblies(
                    targetFramework: "net6.0",
                    referenceAssemblyPackage: new PackageIdentity("Microsoft.NETCore.App.Ref", "6.0.0"),
                    referenceAssemblyPath: Path.Combine("ref", "net6.0"));
            }

            public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.CSharp9;

            protected override CompilationOptions CreateCompilationOptions()
            {
                CompilationOptions compilationOptions = base.CreateCompilationOptions();
                return compilationOptions.WithSpecificDiagnosticOptions(
                     compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
            }

            static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
            {
                string[] args = { "/warnaserror:nullable" };
                var commandLineArguments = CSharpCommandLineParser.Default.Parse(
                    args,
                    baseDirectory: Environment.CurrentDirectory,
                    sdkDirectory: Environment.CurrentDirectory);
                var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

                return nullableWarnings;
            }

            protected override ParseOptions CreateParseOptions()
            {
                return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
            }
        }
    }
}