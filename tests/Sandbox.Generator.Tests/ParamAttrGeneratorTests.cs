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
public class BarClass
{
    public void FooMethod()
    {
    }
}
[ConverterInfoAttribute(typeof(MyConverter))]
public class MyBinding
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
".Replace("'", "\"\"");

        string generatedFileName = "ParamAttr.g.cs";
        string expectedOutput = @"// Auto-generated code
public class FooClass2
{
}
".Replace("'", "\"");

            await TestHelpers.RunTestAsync<ParameterAttrGenerator>(
                generatedFileName,
                code,
                expectedOutput);
        }
    }
}