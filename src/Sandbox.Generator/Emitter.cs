using System.Collections.Generic;
using System.Text;

namespace Sandbox.Generator;

public class Emitter
{
    public string Create(IEnumerable<FuncInfo> functions)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("internal class DirectFunctionExecutor : IFunctionExecutor");
        sb.AppendLine(
            @"public async Task ExecuteAsync(FunctionContext context)
              {{"
        );
        foreach (var function in functions)
        {
            sb.Append(CreateSingleFunc(function));
        }

        sb.AppendLine("}}");
        return sb.ToString();
    }

    private string CreateSingleFunc(FuncInfo funcInfo)
    {
        StringBuilder sb = new StringBuilder();
        var c =
            $@"if (string.Equals(context.FunctionDefinition.Name, ""{funcInfo.FunctionName}"",StringComparison.OrdinalIgnoreCase))
{{
";

        sb.AppendLine(c);
        var cons = "";
        int paramCounter = 1;
        var paramInputs = new List<string>();
        foreach (var argumentTypeName in funcInfo.ParentClass.ConstructorArgumentTypeNames)
        {
            cons += $"var p{paramCounter} = context.InstanceServices.GetService<{argumentTypeName}>()\r\n;";
            paramInputs.Add($"p{paramCounter}");
        }

        sb.AppendLine(cons);
        var inputs = string.Join(",", paramInputs);

        var inputArgs = @$"var modelBindingFeature = context.Features.Get<IModelBindingFeature>()!;
                        var inputArguments = await modelBindingFeature.BindFunctionInputAsync(context);";
        var trg = $"var t = new {funcInfo.ParentClass.ClassName}({inputs})";

        sb.AppendLine(inputArgs);
        sb.AppendLine(trg);
        
        int paramCounter2 = 1;
        var paramInputs2 = new List<string>();
        foreach (var argumentTypeName in funcInfo.ParameterTypeNames)
        {
            paramInputs2.Add($"({argumentTypeName})inputArguments[{paramCounter2}]");
        }
        var methodInputs = string.Join(",", paramInputs2);
        sb.AppendLine($"context.GetInvocationResult().Value = t.{funcInfo.MethodName}({methodInputs})");
        sb.AppendLine("}");

        return sb.ToString();
    }
}