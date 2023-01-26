using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sandbox.Generator
{
    internal static class FuncHelper
    {
        /// <summary>
        /// Checks if a candidate method has a Function attribute on it.
        /// </summary>
        internal static bool IsValidMethodAzureFunction(SemanticModel model, MethodDeclarationSyntax method, out string? functionName)
        {
            functionName = null;
            var methodSymbol = model.GetDeclaredSymbol(method);
            if (methodSymbol == null)
            {
                return false;
            }
            var compilation = model.Compilation;

            foreach (var attr in methodSymbol.GetAttributes())
            {
                if (attr.AttributeClass != null &&
                    SymbolEqualityComparer.Default.Equals(attr.AttributeClass,
                        compilation.GetTypeByMetadataName(Constants.Types.FunctionName)))
                {
                    functionName =
                        (string)attr.ConstructorArguments.First()
                            .Value!; // If this is a function attribute this won't be null
                    return true;
                }
            }

            return false;
        }
    }
}
