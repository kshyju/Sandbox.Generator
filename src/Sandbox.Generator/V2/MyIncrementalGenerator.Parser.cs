

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sandbox.Generator
{
    internal sealed class MyMagicMethod
    {
        public string MethodName { get; set; }
    }

    public partial class MyIncrementalGenerator
    {
        internal sealed class Parser
        {
            internal static List<IMethodSymbol> GetMethodsWithMagicAttribute(Compilation compilation, StringBuilder debugPrintBuilder)
            {
                List<IMethodSymbol> methods = [];

                var globalNamespaceSymbol = compilation.GlobalNamespace;
                foreach (var namespaceSymbol in globalNamespaceSymbol.GetMembers())
                {
                    var visitor = new MyMethodSymbolVisitor(compilation, debugPrintBuilder);
                    visitor.Visit(namespaceSymbol);
                    methods.AddRange(visitor.MethodsWithAttribute);
                }

                return methods;
            }

            internal static MyMagicMethod CreateMyMagicMethodFromMethodSymbol(Compilation compilation, IMethodSymbol methodSymbol, CancellationToken token)
            {
                return new MyMagicMethod
                {
                    MethodName = methodSymbol.Name,
                };
            }

        }
    }
}
