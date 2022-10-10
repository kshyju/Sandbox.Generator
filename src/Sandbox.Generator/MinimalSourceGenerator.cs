using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace Sandbox.Generator
{
    [Generator]
    public class MinimalSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            SourceText sourceText;
            using (var stringWriter = new StringWriter())
            using (var indentedTextWriter = new IndentedTextWriter(stringWriter))
            {
                indentedTextWriter.WriteLine("// Auto-generated code");
                indentedTextWriter.WriteLine("using System;");
                indentedTextWriter.WriteLine("public class FooClass");
                indentedTextWriter.WriteLine("{");
                indentedTextWriter.WriteLine("}");

                indentedTextWriter.Flush();
                sourceText = SourceText.From(stringWriter.ToString(), encoding: Encoding.UTF8, checksumAlgorithm: SourceHashAlgorithm.Sha256);
            }

            context.AddSource($"MyGeneratedFile.g.cs", sourceText);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}