

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Sandbox.Generator
{
    public partial class MyIncrementalGenerator : IIncrementalGenerator
    {
        static StringBuilder debugPrintBuilder = new();
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
           
            // Get the methods with magic atttribute on them.
            IncrementalValuesProvider<IMethodSymbol> assembliesValueProvider = context.CompilationProvider
                .SelectMany(static (compilation, token) =>
                {
                    return Parser.GetMethodsWithMagicAttribute(compilation, debugPrintBuilder);
                }
                );

            // Compilation with the methdo symbols
            IncrementalValueProvider<(Compilation Compilation, ImmutableArray<IMethodSymbol> MethodSymbols)>
                compilationWithAssemblies
                    = context.CompilationProvider.Combine(assembliesValueProvider.Collect());

            // Build a data model from the above (the method symbols and compilation)

            IncrementalValuesProvider<MyMagicMethod> magicTypeEntries = compilationWithAssemblies.SelectMany(
                static (tuple, token) => tuple.MethodSymbols.Select(methodSymbol =>
                    Parser.CreateMyMagicMethodFromMethodSymbol(tuple.Compilation, methodSymbol, token)));

            var finalMagicTypeArray = magicTypeEntries.Collect();

            context.RegisterSourceOutput(finalMagicTypeArray, Execute);
        }

        private void Execute(SourceProductionContext context, ImmutableArray<MyMagicMethod> array)
        {
            SourceText sourceText = Emitter.Emit(array, debugPrintBuilder);
            context.AddSource($"MagicMethods.g.cs", sourceText);
        }
    }
}
