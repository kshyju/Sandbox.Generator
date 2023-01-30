using System.Collections.Generic;
using System.Text;

namespace Sandbox.Generator;

public class Emitter
{
    public string Create(IEnumerable<FuncInfo> functions)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(@"
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Azure.Functions.Worker.Core.FunctionMetadata;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Invocation;
namespace FunctionApp26
{
    internal class DirectFunctionExecutor : IFunctionExecutor
    {
        public async Task ExecuteAsync(FunctionContext context)
        {");      
        foreach (var function in functions)
        {
            sb.Append($"   {CreateSingleFunc(function)}");
        }
        sb.AppendLine(@"
        }
    }
}");
        return sb.ToString();
    }

    private string CreateSingleFunc(FuncInfo funcInfo)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(
            $@"if (string.Equals(context.FunctionDefinition.Name, ""{funcInfo.FunctionName}"",StringComparison.OrdinalIgnoreCase))
            {{");
        int paramCounter = 0;
        var paramInputs = new List<string>();
        foreach (var argumentTypeName in funcInfo.ParentClass.ConstructorArgumentTypeNames)
        {
            paramCounter++;
            sb.AppendLine($@"
                var p{paramCounter} = context.InstanceServices.GetService<{argumentTypeName}>();");
            paramInputs.Add($"p{paramCounter}");
        }

        //sb.AppendLine(cons);
        var inputs = string.Join(",", paramInputs);

        // var inputArgs = @$"                 var modelBindingFeature = context.Features.Get<IModelBindingFeature>()!;
        //                 var inputArguments = await modelBindingFeature.BindFunctionInputAsync(context);";
        // var trg = $"var t = new {funcInfo.ParentClass.ClassName}({inputs});";
        //
        // sb.AppendLine(inputArgs);
        // sb.AppendLine(trg);

        sb.Append(@$"                var modelBindingFeature = context.Features.Get<IModelBindingFeature>()!;
                var inputArguments = await modelBindingFeature.BindFunctionInputAsync(context);
                var t = new {funcInfo.ParentClass.ClassName}({inputs});");
        
        int paramCounter2 = 0;
        var paramInputs2 = new List<string>();
        foreach (var argumentTypeName in funcInfo.ParameterTypeNames)
        {
            paramCounter2++;
            paramInputs2.Add($"({argumentTypeName})inputArguments[{paramCounter2}]");
        }
        var methodInputs = string.Join(",", paramInputs2);
        sb.Append(@$"
                 context.GetInvocationResult().Value = t.{funcInfo.MethodName}({methodInputs});
         }}
        ");

        return sb.ToString();
    }
}