using System;
using ToSic.Sxc.Code.Internal.SourceCode;

namespace ToSic.Sxc.Tests.Code.Help;

[TestClass]
public class SourceAnalyzerTests
{
    private static string ExtractBaseClass(string sourceCode, string className) 
        => SourceAnalyzer.ExtractBaseClass(sourceCode, className);

    [TestMethod]
    public void ExtractBaseClass_ValidClass_ShouldReturnBaseClass()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "MyClass";
        AreEqual("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_ValidClassCaseInsensitive_ShouldReturnBaseClass()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "myclass";
        AreEqual("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_ClassWithoutBase_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass { }";
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_InvalidClassName_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "UnknownClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_ClassWithGenericBase_ShouldHandleCorrectly()
    {
        var sourceCode = "public class MyClass : List<string> { }";
        var className = "MyClass";
        AreEqual("List<string>", ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_ClassWithInterfaces_ShouldReturnFirstInterface()
    {
        var sourceCode = "public class MyClass : IInterface, MyBaseClass { }";
        var className = "MyClass";
        AreEqual("IInterface", ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    [Ignore]
    public void ExtractBaseClass_NestedClass_ShouldReturnNull() // or correct behavior if intended
    {
        var sourceCode = "public class OuterClass { public class MyClass : MyBaseClass { } }";
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    [Ignore]
    public void ExtractBaseClass_ClassInComments_ShouldReturnNull()
    {
        var sourceCode = "// public class MyClass : MyBaseClass { }";
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    [Ignore]
    public void ExtractBaseClass_ClassInStringLiteral_ShouldReturnNull()
    {
        var sourceCode = "string code = \"public class MyClass : MyBaseClass { }\";";
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_UnusualFormatting_ShouldHandleCorrectly()
    {
        var sourceCode = "public    class     MyClass\n:\nMyBaseClass { }";
        var className = "MyClass";
        AreEqual("MyBaseClass", ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_NullSourceCode_ShouldReturnNull()
    {
        string sourceCode = null;
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    public void ExtractBaseClass_EmptySourceCode_ShouldReturnNull()
    {
        var sourceCode = "";
        var className = "MyClass";
        IsNull(ExtractBaseClass(sourceCode, className));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    [Ignore]
    public void ExtractBaseClass_NullClassName_ShouldThrowArgumentNullException()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        string className = null;
        ExtractBaseClass(sourceCode, className);
    }

    [TestMethod]
    public void ExtractBaseClass_EmptyClassName_ShouldReturnNull()
    {
        var sourceCode = "public class MyClass : MyBaseClass { }";
        var className = "";
        IsNull(ExtractBaseClass(sourceCode, className));
    }
}