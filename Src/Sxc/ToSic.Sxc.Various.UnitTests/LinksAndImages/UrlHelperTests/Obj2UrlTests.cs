using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests;


public class Obj2UrlTests
{
    private static readonly object TestCase1 = new
    {
        test = 7,
        name = "daniel"
    };
    private static readonly string TestCase1Str = "test=7&name=daniel";

    private static readonly object TestCase2 = new
    {
        lastName = "mettler"
    };

    private static readonly string TestCase2Str = "lastName=mettler";

    private string O2uSerialize(object data) => new ObjectToUrl().Serialize(data);


    [Fact]
    public void WithArray()
    {
        var obj = new
        {
            array = new[] { 32, 16, 8 }
        };
        Equal("array=32,16,8", O2uSerialize(obj));
    }

    [Fact]
    public void StringOnly() => Equal(TestCase1Str, O2uSerialize(TestCase1Str));

    [Fact]
    public void StringNull() => Equal(null as string, O2uSerialize(null));
    [Fact]
    public void StringEmpty() => Equal("", O2uSerialize(""));

    [Fact]
    public void StringsInArray() => Equal(TestCase1Str, O2uSerialize( new object[] { TestCase1Str }));

    [Fact]
    public void StringsInArrayNullOnly() => Equal("", O2uSerialize( new object[] { null }));
    [Fact]
    public void StringsInArrayEmptyOnly() => Equal("", O2uSerialize( new object[] { "" }));

    [Fact]
    public void StringsInArrayWithNull() => Equal(TestCase1Str, O2uSerialize( new object[] { TestCase1Str, null }));

    [Fact]
    public void StringsInArray2() => Equal($"{TestCase1Str}&{TestCase2Str}", O2uSerialize( new object[] { TestCase1Str, TestCase2Str }));

    [Fact]
    public void BasicObj()
    {
        Equal(TestCase1Str, O2uSerialize(TestCase1));
    }
    [Fact]
    public void WithSubArray()
    {
        var obj = new
        {
            prefill = new
            {
                array = new int[] { 32, 16, 8 }
            }
        };
        Equal("prefill:array=32,16,8", O2uSerialize(obj));
    }



    [Fact]
    public void MergeObject1And2()
    {
        Equal($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1, TestCase2 }));
    }

    [Fact]
    public void MergeObject1AndString2()
    {
        Equal($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1, TestCase2Str }));
    }

    [Fact]
    public void MergeString1AndObject2()
    {
        Equal($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1Str, TestCase2 }));
    }

    [Fact]
    public void BasicWithPrefix()
    {
        var prefix = "prefix:";
        Equal($"{prefix}test=7&{prefix}name=daniel", new ObjectToUrl(prefix: prefix).Serialize(TestCase1));
    }

    [Fact]
    public void SubObj()
    {
        var obj = new
        {
            test = 7,
            name = "daniel",
            prefill = new
            {
                title = "new title"
            }
        };
        Equal("test=7&name=daniel&prefill:title=new%20title", O2uSerialize(obj));
    }

    [Fact]
    public void SubSubObj()
    {
        var obj = new
        {
            prefill = new
            {
                title = "new title",
                entities = new
                {
                    name = "daniel",
                    and = "ok"
                }
            }
        };
        Equal("prefill:title=new%20title&prefill:entities:name=daniel&prefill:entities:and=ok", O2uSerialize(obj));
    }
}