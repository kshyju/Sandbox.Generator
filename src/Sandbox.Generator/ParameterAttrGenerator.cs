using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sandbox.Generator;

[Generator]
public class ParameterAttrGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var methodSyntaxReceiver = (MyMethodSyntaxReceiver)context.SyntaxReceiver;

        var methods = methodSyntaxReceiver!.CandidateMethods;

        if (!methods.Any()) return;

        var methodsToConvertersMap = ProcessMethods(context, methods);

        //trees.Where(t=>t.GetT)

        SourceText sourceText;
        using (var stringWriter = new StringWriter())
        using (var indentedTextWriter = new IndentedTextWriter(stringWriter))
        {
            indentedTextWriter.WriteLine("// Auto-generated code");
            indentedTextWriter.WriteLine("using System.Collections.Generic;");
            indentedTextWriter.WriteLine("public class FooClass2");
            indentedTextWriter.WriteLine("{");
            indentedTextWriter.Indent++;
            indentedTextWriter.WriteLine("public IDictionary<string, string> GetConverterMapping()");
            indentedTextWriter.WriteLine("{");
            indentedTextWriter.Indent++;
            indentedTextWriter.WriteLine("var dict = new Dictionary<string, string>();");
            foreach (var methodToConverter in methodsToConvertersMap)
            {
                indentedTextWriter.WriteLine($"dict.Add(\"{methodToConverter.Key}\", \"{methodToConverter.Value}\");");
            }
            indentedTextWriter.WriteLine("return dict;");
            indentedTextWriter.Indent--;
            indentedTextWriter.WriteLine("}");
            indentedTextWriter.Indent--;
            indentedTextWriter.WriteLine("}");

            indentedTextWriter.Flush();
            sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8, SourceHashAlgorithm.Sha256);
        }

        context.AddSource("ParamAttr.g.cs", sourceText);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MyMethodSyntaxReceiver());
    }

    private Dictionary<string, string> ProcessMethods(GeneratorExecutionContext context, List<MethodDeclarationSyntax> methods)
    {
        var methodsToConverters = new Dictionary<string, string>();
        foreach (var method in methods)
        {
            var model = context.Compilation.GetSemanticModel(method.SyntaxTree);
            var methodName = method.Identifier.Text;
            // Get params with attributes
            var paramsWithAttr = method.ParameterList.Parameters.Where(a => a.AttributeLists.Any());

            var converterInfoAttSymbol = context.Compilation.GetTypeByMetadataName("Sandbox.ConverterInfoAttribute");
            var bindingBaseNamedTypeSymbol = context.Compilation.GetTypeByMetadataName("Sandbox.MyBindingBase");

            foreach (var parameterSyntax in paramsWithAttr)
            {
                if (model.GetDeclaredSymbol(parameterSyntax) is not IParameterSymbol parameterSymbol) continue;
                var paramName = parameterSyntax.Identifier.Text;

                foreach (var attributeData in parameterSymbol.GetAttributes())
                {
                    if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass.BaseType,
                            bindingBaseNamedTypeSymbol))
                        continue;

                    var bindingAttrSymbol = attributeData.AttributeClass;
                    foreach (var triggerAttr in bindingAttrSymbol.GetAttributes())
                    {
                        if (!SymbolEqualityComparer.Default.Equals(triggerAttr.AttributeClass,
                                converterInfoAttSymbol))
                            continue;

                        // This is converterInfo attribute
                        var firstConstructorArg = triggerAttr.ConstructorArguments.FirstOrDefault();
                        if (firstConstructorArg.Value is INamedTypeSymbol argValueSymbol)
                        {
                            var fullyQualifiedConverterName = argValueSymbol.ToDisplayString();

                            methodsToConverters[$"{methodName}.{paramName}"] = fullyQualifiedConverterName;
                        }
                    }
                }
            }
        }

        return methodsToConverters;
    }
}

public class MyMethodSyntaxReceiver : ISyntaxReceiver
{
    public List<MethodDeclarationSyntax> CandidateMethods { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax &&
            methodDeclarationSyntax.ParameterList.Parameters.Any())
            CandidateMethods.Add(methodDeclarationSyntax);
    }
}