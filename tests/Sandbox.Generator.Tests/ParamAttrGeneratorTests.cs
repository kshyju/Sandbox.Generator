using System.Threading.Tasks;
using Xunit;

namespace Sandbox.Generator.Tests
{
    public class ParamAttrGeneratorTests
    {
        [Fact]
        public async Task ParameterAttrsTest()
        {
            string code = @"
using System;
namespace Sandbox
{
public class BarClass
{
    public void FooMethod([MyTriggerBinding] string triggerData, int age)
    {
    }
    private void AnotherMethod()
    {
    }
}
[ConverterInfoAttribute(typeof(MyConverter))]
public class MyTriggerBinding : MyBindingBase
{
}
public class MyBindingBase : Attribute
{
}
public class ConverterInfoAttribute : Attribute
{
    public ConverterInfoAttribute(Type converterType)
    {
        ConverterType = converterType;
    }

    public Type ConverterType { get; }
}

public class MyConverter
{
    public string Convert() => ""From MyConvert"";
}
}
".Replace("'", "\"\"");

        string generatedFileName = "ParamAttr.g.cs";
        string expectedOutput = $@"// Auto-generated code
using System.Collections.Generic;
public class FooClass2
{{
    public IDictionary<string, string> GetConverterMapping()
    {{
        var dict = new Dictionary<string, string>();
        dict.Add('FooMethod.triggerData', 'Sandbox.MyConverter');
        return dict;
    }}
}}
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<ParameterAttrGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}