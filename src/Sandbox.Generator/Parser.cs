using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace Sandbox.Generator
{
    internal class Parser
    {
        private readonly GeneratorExecutionContext _context;

        private Compilation Compilation => _context.Compilation;

        private CancellationToken CancellationToken => _context.CancellationToken;

        public Parser(GeneratorExecutionContext context)
        {
            _context = context;
        }

        public IEnumerable<ClassInfo> GetStuff(List<MethodDeclarationSyntax> methods)
        {
            
            var result = new List<ClassInfo>();
            foreach (MethodDeclarationSyntax method in methods)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var model = Compilation.GetSemanticModel(method.SyntaxTree);

                if (!FuncHelper.IsValidMethodAzureFunction(model, method, out string? functionName))
                {
                    continue;
                }

                var methodParameterList = new List<string>(method.ParameterList.Parameters.Count);

                foreach (var methodParam in method.ParameterList.Parameters)
                {
                    if (model.GetDeclaredSymbol(methodParam) is not IParameterSymbol parameterSymbol) continue;

                    methodParameterList.Add(parameterSymbol.Type.Name);
                }

                var funcInfo = new FuncInfo(functionName!)
                {
                    ParameterTypeNames = methodParameterList
                };

                ClassDeclarationSyntax functionClass = (ClassDeclarationSyntax)method.Parent!;
                var className = functionClass.Identifier.Text;
                ClassInfo classInfo = new ClassInfo(className);
                result.Add(classInfo);

                var constructorParams = GetConstructorParamTypeNames(functionClass, model);
                classInfo.ConstructorArgumentTypeNames = constructorParams;
            }

            return result;
        }

        private static IEnumerable<string> GetConstructorParamTypeNames(ClassDeclarationSyntax functionClass,
            SemanticModel model)
        {
            var firstConstructorMember = GetBestConstructor(functionClass);

            if (firstConstructorMember is not ConstructorDeclarationSyntax constructorSyntax)
            {
                return Enumerable.Empty<string>();
            }

            var constructorParams = new List<string>(constructorSyntax.ParameterList.Parameters.Count);

            foreach (var param in constructorSyntax.ParameterList.Parameters)
            {
                if (model.GetDeclaredSymbol(param) is not IParameterSymbol parameterSymbol) continue;

                constructorParams.Add(parameterSymbol.Type.Name);
            }

            return constructorParams;
        }

        private static MemberDeclarationSyntax GetBestConstructor(ClassDeclarationSyntax functionClass)
        {
            // to do: Use a better algo for this instead of picking first constructor.
            var firstConstructorMember =
                functionClass.Members.FirstOrDefault(member => member is ConstructorDeclarationSyntax);

            return firstConstructorMember;
        }
    }
}