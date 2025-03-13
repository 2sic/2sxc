﻿using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests;

[TestClass]
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


    [TestMethod]
    public void WithArray()
    {
        var obj = new
        {
            array = new int[] { 32, 16, 8 }
        };
        AreEqual("array=32,16,8", O2uSerialize(obj));
    }

    [TestMethod]
    public void StringOnly() => AreEqual(TestCase1Str, O2uSerialize(TestCase1Str));

    [TestMethod]
    public void StringNull() => AreEqual(null as string, O2uSerialize(null));
    [TestMethod]
    public void StringEmpty() => AreEqual("", O2uSerialize(""));

    [TestMethod]
    public void StringsInArray() => AreEqual(TestCase1Str, O2uSerialize( new object[] { TestCase1Str }));

    [TestMethod]
    public void StringsInArrayNullOnly() => AreEqual("", O2uSerialize( new object[] { null }));
    [TestMethod]
    public void StringsInArrayEmptyOnly() => AreEqual("", O2uSerialize( new object[] { "" }));

    [TestMethod]
    public void StringsInArrayWithNull() => AreEqual(TestCase1Str, O2uSerialize( new object[] { TestCase1Str, null }));

    [TestMethod]
    public void StringsInArray2() => AreEqual($"{TestCase1Str}&{TestCase2Str}", O2uSerialize( new object[] { TestCase1Str, TestCase2Str }));

    [TestMethod]
    public void BasicObj()
    {
        AreEqual(TestCase1Str, O2uSerialize(TestCase1));
    }
    [TestMethod]
    public void WithSubArray()
    {
        var obj = new
        {
            prefill = new
            {
                array = new int[] { 32, 16, 8 }
            }
        };
        AreEqual("prefill:array=32,16,8", O2uSerialize(obj));
    }



    [TestMethod]
    public void MergeObject1And2()
    {
        AreEqual($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1, TestCase2 }));
    }

    [TestMethod]
    public void MergeObject1AndString2()
    {
        AreEqual($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1, TestCase2Str }));
    }

    [TestMethod]
    public void MergeString1AndObject2()
    {
        AreEqual($"{TestCase1Str}&{TestCase2Str}", O2uSerialize(new object[] { TestCase1Str, TestCase2 }));
    }

    [TestMethod]
    public void BasicWithPrefix()
    {
        var prefix = "prefix:";
        AreEqual($"{prefix}test=7&{prefix}name=daniel", new ObjectToUrl(prefix: prefix).Serialize(TestCase1));
    }

    [TestMethod]
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
        AreEqual("test=7&name=daniel&prefill:title=new%20title", O2uSerialize(obj));
    }

    [TestMethod]
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
        AreEqual("prefill:title=new%20title&prefill:entities:name=daniel&prefill:entities:and=ok", O2uSerialize(obj));
    }
}