using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Typed;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.DataTests;

[TestClass]
public class HasKeysTests
{
    [TestMethod]
    public void IsFilledNull() 
        => AreEqual(false, HasKeysHelper.IsNotEmpty(null, blankIsEmpty: null));

    [TestMethod]
    public void IsEmptyNull() 
        => AreEqual(true, HasKeysHelper.IsEmpty(null, blankIsEmpty: null));

    public static IEnumerable<object[]> BlankStrings =>
    [
        [""],
        [" "],
        ["   "],
        ["\t", "tab"],
        ["\t \t", "tabs"],
        ["\n \r", "new lines"],
        ["\u00A0", "non-breaking space"],
        ["&nbsp;", "non-breaking space HTML"],
        [" &nbsp; \n", "non-breaking space HTML"]
    ];

    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankIsDefault(string value, string testName = default) 
        => AreEqual(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");

    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankIsFalse(string value, string testName = default) 
        => AreEqual(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: false");

    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsFilled_StringsBlank_BlankIsTrue(string value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: true");
        
    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankIsDefault(string value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");

    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankIsFalse(string value, string testName = default) 
        => AreEqual(false, HasKeysHelper.IsEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: null");

    [TestMethod]
    [DynamicData(nameof(BlankStrings))]
    public void IsEmpty_StringsBlank_BlankIsTrue(string value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: null");


    public static IEnumerable<object[]> SimpleData =>
    [
        [0],
        [-1],
        [27],
        [true],
        [false],
        ["hello"],
        ['x', "Character"]
    ];

    [TestMethod]
    [DynamicData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankIsDefault(object value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");

    [TestMethod]
    [DynamicData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankIsFalse(object value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: false");

    [TestMethod]
    [DynamicData(nameof(SimpleData))]
    public void IsFilled_SimpleData_BlankIsTrue(object value, string testName = default) 
        => AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: true");
        

    [TestMethod]
    public void ContainsDataObject()
    {
        var value = new object();
        var testName = "object";
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: false");
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: true");
    }

    [TestMethod]
    public void ContainsDataListEmpty()
    {
        var value = new List<string>();
        var testName = "object";
        AreEqual(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");
        AreEqual(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: false");
        AreEqual(false, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: true");
    }

    [TestMethod]
    public void ContainsDataListNonEmpty()
    {
        var value = new List<string> { "hello" };
        var testName = "object";
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: null), testName ?? value + " blankIs: null");
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: false), testName ?? value + " blankIs: false");
        AreEqual(true, HasKeysHelper.IsNotEmpty(value, blankIsEmpty: true), testName ?? value + " blankIs: true");
    }
}