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
            string txt = "// ";
            if(context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.FunctionsEnablePlaceholder", 
                out var enablePlaceHolder))
            {
                txt += $"TryGetValue=True, enablePlaceHolder:{enablePlaceHolder}";
            }
            else
            {
                txt += "TryGetValue=false";
            }

            SourceText sourceText;
            using (var stringWriter = new StringWriter())
            using (var indentedTextWriter = new IndentedTextWriter(stringWriter))
            {
                indentedTextWriter.WriteLine("// Auto-generated code");
                indentedTextWriter.WriteLine(txt);

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