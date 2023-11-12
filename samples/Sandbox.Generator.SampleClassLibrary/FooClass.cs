namespace Sandbox.Generator.SampleClassLibrary
{
    public class FooClass
    {
        public FooClass(string source)
        {
            Source = source;
        }
        public string Source { get; }

        public string GetFoo() => "Foo";

        public void UpdateFoo(int id) { }
    }

    public class BarOuterClass
    {
        public class BarBazInnerClass
        {
            public string Name => "BarBaz";
            public string GetBarBaz() => "BarBaz";
        }
    }
}