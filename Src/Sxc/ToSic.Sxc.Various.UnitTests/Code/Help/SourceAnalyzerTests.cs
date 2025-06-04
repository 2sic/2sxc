using ToSic.Sxc.Code.Internal.SourceCode;

namespace ToSic.Sxc.Tests.Code.Help;


public class SourceAnalyzerTests
{
    private static string ExtractBaseClass(string sourceCode, string className) 
        => SourceAnalyzer.ExtractBaseClass(sourceCode, className);

    [Fact]
    public void ExtractBaseClass_ValidClass_ShouldReturnBaseClass()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "MyClass";
        Equal("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_ValidClassCaseInsensitive_ShouldReturnBaseClass()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "myclass";
        Equal("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_ClassWithoutBase_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass { }";
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_InvalidClassName_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "UnknownClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_ClassWithGenericBase_ShouldHandleCorrectly()
    {
        var sourceCode = "public class MyClass : List<string> { }";
        var className = "MyClass";
        Equal("List<string>", ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_ClassWithInterfaces_ShouldReturnFirstInterface()
    {
        var sourceCode = "public class MyClass : IInterface, MyBaseClass { }";
        var className = "MyClass";
        Equal("IInterface", ExtractBaseClass(sourceCode, className));
    }

    [Fact(Skip = "unsure")]
    //[Ignore]
    public void ExtractBaseClass_NestedClass_ShouldReturnNull() // or correct behavior if intended
    {
        var sourceCode = "public class OuterClass { public class MyClass : MyBaseClass { } }";
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact(Skip = "unsure")]
    //[Ignore]
    public void ExtractBaseClass_ClassInComments_ShouldReturnNull()
    {
        var sourceCode = "// public class MyClass : MyBaseClass { }";
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact(Skip = "unsure")]
    //[Ignore]
    public void ExtractBaseClass_ClassInStringLiteral_ShouldReturnNull()
    {
        var sourceCode = "string code = \"public class MyClass : MyBaseClass { }\";";
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_UnusualFormatting_ShouldHandleCorrectly()
    {
        var sourceCode = "public    class     MyClass\n:\nMyBaseClass { }";
        var className = "MyClass";
        Equal("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_NullSourceCode_ShouldReturnNull()
    {
        string sourceCode = null;
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact]
    public void ExtractBaseClass_EmptySourceCode_ShouldReturnNull()
    {
        var sourceCode = "";
        var className = "MyClass";
        Null(ExtractBaseClass(sourceCode, className));
    }

    [Fact(Skip = "unsure")]
    //[Ignore]
    public void ExtractBaseClass_NullClassName_ShouldThrowArgumentNullException()
    {
        Throws<ArgumentNullException>(() =>
        {
            var sourceCode = "public class MyClass : MyBaseClass { }";
            string className = null;
            ExtractBaseClass(sourceCode, className);
        });
    }

    [Fact]
    public void ExtractBaseClass_EmptyClassName_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "";
        Null(ExtractBaseClass(sourceCode, className));
    }
}