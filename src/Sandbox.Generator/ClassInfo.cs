using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Generator;

internal class ClassInfo
{
    public ClassInfo(string className)
    {
        ClassName = className;
    }

    public IEnumerable<string>? ConstructorArgumentTypeNames { set; get; }= Enumerable.Empty<string>();

    public string ClassName { get; }

    public FuncInfo[]? Functions { set; get; }
}
internal class FuncInfo
{
    public FuncInfo(string functionName)
    {
        FunctionName = functionName;
    }

    public bool IsStatic { get; set; }
    public string FunctionName { get; }

    public IEnumerable<string> ParameterTypeNames { set; get; } = Enumerable.Empty<string>();
}