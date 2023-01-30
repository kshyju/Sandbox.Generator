using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sandbox.Generator
{
    [Generator]
    public class MinimalSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver || receiver.CandidateMethods.Count == 0)
            {
                return;
            }

            Parser p = new Parser(context);
            var funcClasses = p.GetStuff(receiver.CandidateMethods);

            var sourceText = new Emitter().Create(funcClasses);
            foreach(var cls in funcClasses)
            {

            }
            context.AddSource($"MyGeneratedFile.g.cs", sourceText);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> CandidateMethods { get; } = new List<MethodDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MethodDeclarationSyntax methodSyntax)
                {
                    if (methodSyntax.AttributeLists.Count > 0) // collect all methods with attributes - we will verify they are functions when we have access to symbols to get the full name
                    {
                        CandidateMethods.Add(methodSyntax);
                       
                    }
                }
            }
        }
    }
}