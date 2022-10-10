namespace Sandbox;

public class BarFunctionClass
{
    public void FooMethod([MyTriggerBinding] string triggerData, string other)
    {
    }
}
[ConverterInfo(typeof(MyConverter))]
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
    public string Convert() => "From MyConvert";
}