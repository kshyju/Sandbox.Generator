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

        public IEnumerable<FuncInfo> GetStuff(List<MethodDeclarationSyntax> methods)
        {
            Dictionary<string, ClassInfo> classDict = new Dictionary<string, ClassInfo>();

            var functionList = new List<FuncInfo>();
            foreach (MethodDeclarationSyntax method in methods)
            {
                CancellationToken.ThrowIfCancellationRequested();

                var model = Compilation.GetSemanticModel(method.SyntaxTree);

                if (!FuncHelper.IsValidMethodAzureFunction(model, method, out string? functionName))
                {
                    continue;
                }

                var methodName = method.Identifier.Text;

                var methodParameterList = new List<string>(method.ParameterList.Parameters.Count);

                foreach (var methodParam in method.ParameterList.Parameters)
                {
                    if (model.GetDeclaredSymbol(methodParam) is not IParameterSymbol parameterSymbol) continue;

                    methodParameterList.Add(parameterSymbol.Type.Name);
                }

                var methodSymSemanticModel = Compilation.GetSemanticModel(method.SyntaxTree);
                var methodSymbol = methodSymSemanticModel.GetDeclaredSymbol(method);
                var fullyQualifiedClassName = methodSymbol.ContainingSymbol.ToDisplayString();

                ClassDeclarationSyntax functionClass = (ClassDeclarationSyntax)method.Parent!;
                var entryPoint = $"{fullyQualifiedClassName}.{methodName}";

                if (!classDict.TryGetValue(entryPoint, out var classInfo))
                {
                    classInfo = new ClassInfo(fullyQualifiedClassName)
                    {
                        ConstructorArgumentTypeNames = GetConstructorParamTypeNames(functionClass, model)
                    };
                    classDict[entryPoint] = classInfo;
                }

                var funcInfo = new FuncInfo(entryPoint!)
                {
                    ParameterTypeNames = methodParameterList,
                    MethodName = methodName
                };
                funcInfo.ParentClass = classInfo;

                functionList.Add(funcInfo);
            }

            return functionList;
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