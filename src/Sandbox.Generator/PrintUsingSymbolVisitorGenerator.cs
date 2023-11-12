using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Linq;
using System;

namespace Sandbox.Generator
{
    [Generator]
    public sealed class PrintUsingSymbolVisitorGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            StringBuilder sw = new();
            sw.AppendLine($"// Printing symbols");

            var assemblySymbols = compilation.SourceModule.ReferencedAssemblySymbols;
            
            foreach (var assemblySymbol in assemblySymbols)
            {
                sw.AppendLine($"// Assembly {assemblySymbol.Name}");

                var visitor = new MethodSymbolVisitor();
                visitor.Visit(assemblySymbol);
                sw.AppendLine(visitor.PrintBuilder.ToString());
            }

            var sourceText = SourceText.From(sw.ToString(), encoding: Encoding.UTF8, checksumAlgorithm: SourceHashAlgorithm.Sha256);
            context.AddSource($"MyGeneratedFile.g.cs", sourceText);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }

    public sealed class MethodSymbolVisitor : SymbolVisitor
    {
        public readonly StringBuilder PrintBuilder = new();

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            var namespaceSymbol = symbol.GlobalNamespace;
            PrintBuilder.AppendLine($"//VisitAssembly({symbol.Name})> {namespaceSymbol.Name}");

            namespaceSymbol.Accept(this);

            base.VisitAssembly(symbol);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            PrintBuilder.AppendLine($"//    Inside VisitNamespace for {symbol.Name}");
            var childOfNs = symbol.GetMembers().ToList();

            PrintBuilder.AppendLine($"//----");

            foreach (var nsChild in childOfNs)
            {
                Console.WriteLine(nsChild.Name);
                //sb.AppendLine($"    VisitNamespace(${symbol.Name})> {nsChild.Name}");
                nsChild.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            PrintBuilder.AppendLine($"//        Inside VisitNamedType for {symbol.Name}");

            foreach (var typedSymbolChild in symbol.GetMembers())
            {
                Console.WriteLine(typedSymbolChild.Name);
                typedSymbolChild.Accept(this);
            }
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            if (symbol.MethodKind == MethodKind.Ordinary)
            {
                PrintBuilder.AppendLine($"//            VisitMethod: {symbol.Name}");
            }
        }
    }
}