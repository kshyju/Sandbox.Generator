using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox.Generator;

public class ClassInfo
{
    public ClassInfo(string className)
    {
        ClassName = className;
    }

    public IEnumerable<string>? ConstructorArgumentTypeNames { set; get; }= Enumerable.Empty<string>();

    public string ClassName { get; }

}
public class FuncInfo
{
    public FuncInfo(string functionName)
    {
        FunctionName = functionName;
    }
    
    public string MethodName { get; set; }

    public bool IsStatic { get; set; }
    public string FunctionName { get; }
    
    public ClassInfo ParentClass { set; get; }

    public IEnumerable<string> ParameterTypeNames { set; get; } = Enumerable.Empty<string>();
}