// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Sandbox.Generator.V1
{
    /// <summary>
    /// </summary>
    internal sealed class AssemblyMethodVisitor : SymbolVisitor
    {
        private readonly Compilation _compilation;

        /// <summary>
        /// Gets all methods which are valid Azure Functions.
        /// </summary>
        internal readonly List<IMethodSymbol> FunctionMethods = new();

        internal AssemblyMethodVisitor(Compilation compilation)
        {
            _compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
        }

        public override void VisitModule(IModuleSymbol moduleSymbol)
        {
            var allAssemblySymbols = moduleSymbol.ReferencedAssemblySymbols.Concat(new[] { moduleSymbol.ContainingAssembly });
            foreach (var assemblySymbol in allAssemblySymbols)
            {
                assemblySymbol.Accept(this);
            }
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            var namespaceSymbol = symbol.GlobalNamespace;
            namespaceSymbol.Accept(this);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            // Get classes in this namespace or child namespaces
            var classesOrNamespaces = symbol.GetMembers()
                .Where(a => a.Kind is SymbolKind.Namespace or SymbolKind.NamedType);

            foreach (var childSymbol in classesOrNamespaces)
            {
                childSymbol.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            // Get methods in this class or nested child classes
            var methodsOrClasses = symbol.GetMembers()
                .Where(a => a.Kind is SymbolKind.NamedType or SymbolKind.Method);

            foreach (var childSymbol in methodsOrClasses)
            {
                childSymbol.Accept(this);
            }
        }

        public override void VisitMethod(IMethodSymbol methodSymbol)
        {
            if (methodSymbol.MethodKind == MethodKind.Ordinary)
            {
                if (methodSymbol.GetAttributes().Any(a => a.AttributeClass.Name.Contains("MyMagic")))
                {
                    FunctionMethods.Add(methodSymbol);
                }
            }
        }
    }
}
