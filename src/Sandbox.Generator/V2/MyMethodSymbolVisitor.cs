

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox.Generator
{
    internal sealed class MyMethodSymbolVisitor : SymbolVisitor
    {
        private const string MyMagicAttributeFullName = "Sandbox.Generator.SampleClassLibrary.MyMagicAttribute";

        public readonly StringBuilder PrintBuilder = new();
        public List<IMethodSymbol> MethodsWithAttribute = [];

        private readonly Compilation _compilation;
        private readonly INamedTypeSymbol _magicAttributeType;
        internal MyMethodSymbolVisitor(Compilation compilation, StringBuilder printBuilder)
        {
            this._compilation = compilation;
            PrintBuilder = printBuilder;
            _magicAttributeType = compilation.GetTypeByMetadataName(MyMagicAttributeFullName);
        }

        public override void VisitNamespace(INamespaceSymbol namespaceSymbol)
        {
            PrintBuilder.AppendLine($"// {namespaceSymbol.Name}");

            foreach (var nsChild in namespaceSymbol.GetMembers())
            {
                nsChild.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            PrintBuilder.AppendLine($"//    {symbol.Name}");

            foreach (var typedSymbolChild in symbol.GetMembers())
            {
                typedSymbolChild.Accept(this);
            }
        }

        public override void VisitMethod(IMethodSymbol methodSymbol)
        {
            PrintBuilder.AppendLine($"//        {methodSymbol.Name}");

            if (methodSymbol.MethodKind != MethodKind.Ordinary)
            {
                return;
            }

            if (methodSymbol.GetAttributes().Any(attr => attr.AttributeClass != null
                && _magicAttributeType.Equals(attr.AttributeClass, SymbolEqualityComparer.Default)))
            {
                MethodsWithAttribute.Add(methodSymbol);
            }
        }
    }
}
