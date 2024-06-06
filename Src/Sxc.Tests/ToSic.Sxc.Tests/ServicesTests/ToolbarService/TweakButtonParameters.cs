using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Edit.Toolbar.Internal;
using static ToSic.Sxc.Edit.Toolbar.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;

[TestClass]
public class TweakButtonParameters: TweakButtonTestsBase
{

    [TestMethod]
    public void FormParameters1String()
        => AssertParams([$"{RuleParamPrefixForm}Hello"], NewTb().TacFormParameters("Hello"));

    [TestMethod]
    public void FormParameters2Strings()
        => AssertParams([$"{RuleParamPrefixForm}Hello=World"], NewTb().TacFormParameters("Hello", "World"));

    [TestMethod]
    public void FormParameters1ObjectA()
        => AssertParams([$"{RuleParamPrefixForm}hello=world"], NewTb().TacFormParameters(new { hello = "world"}));

    [TestMethod]
    public void FormParameters1ObjectB()
        => AssertParams([$"{RuleParamPrefixForm}hello=world&{RuleParamPrefixForm}name=iJungleboy"], NewTb().TacFormParameters(new { hello = "world", name = "iJungleboy"}));

    [TestMethod]
    public void Parameters1String()
        => AssertParams(["Hello"], NewTb().TacParameters("Hello"));

    [TestMethod]
    public void Parameters2Strings()
        => AssertParams(["Hello=World"], NewTb().TacParameters("Hello", "World"));

    [TestMethod]
    public void Parameters1ObjectA()
        => AssertParams([new { hello = "world" }], NewTb().TacParameters(new { hello = "world"}));

    [TestMethod]
    public void Parameters1ObjectB()
        => AssertParams([new { hello = "world", name = "iJungleboy" }], NewTb().TacParameters(new { hello = "world", name = "iJungleboy"}));

    [TestMethod]
    public void Prefill1String()
        => AssertParams([$"{RuleParamPrefixPrefill}Hello"], NewTb().TacPrefill("Hello"));

    [TestMethod]
    public void Prefill2Strings()
        => AssertParams([$"{RuleParamPrefixPrefill}Hello=World"], NewTb().TacPrefill("Hello", "World"));

    private static readonly Guid Guid1 = new("640df5cf-ec2b-4943-9962-5a98bb1e8d01");
    private static readonly Guid Guid2 = new("7b42eae8-2ead-479e-af67-8e1349939ed6");
    private static readonly Guid[] Guids = [Guid1, Guid2];
    private static readonly string GuidsExpected = $"[\"{Guid1}\",\"{Guid2}\"]";

    [TestMethod]
    public void PrefillGuidArray()
        => AssertParams([$"{RuleParamPrefixPrefill}Children={GuidsExpected}"], NewTb().TacPrefill("Children", Guids));

    // todo: also prefill w/second param being an object

    [TestMethod]
    public void Prefill1ObjectA()
        => AssertParams([$"{RuleParamPrefixPrefill}hello=world"], NewTb().TacPrefill(new { hello = "world"}));

    [TestMethod]
    public void Prefill1ObjectB()
        => AssertParams([$"{RuleParamPrefixPrefill}hello=world&{RuleParamPrefixPrefill}name=iJungleboy"], NewTb().TacPrefill(new { hello = "world", name = "iJungleboy"}));

    // TODO: filters must test for arrays of string, int, guid
    [TestMethod]
    public void Filter1Object()
    => AssertParams([$"{RuleParamPrefixFilter}hello=world"], NewTb().TacFilter(new { hello = "world"}));

    [TestMethod]
    public void Filter2Objects()
        => AssertParams([$"{RuleParamPrefixFilter}hello=world&{RuleParamPrefixFilter}name=iJungleboy"], NewTb().TacFilter(new { hello = "world", name = "iJungleboy"}));

    [TestMethod]
    public void Filter1String()
        => AssertParams([$"{RuleParamPrefixFilter}Hello"], NewTb().TacFilter("Hello"));

    [TestMethod]
    public void Filter2Strings()
        => AssertParams([$"{RuleParamPrefixFilter}Hello=World"], NewTb().TacFilter("Hello", "World"));

    [TestMethod]
    public void FilterGuidArray()
        => AssertParams([$"{RuleParamPrefixFilter}Children={GuidsExpected}"], NewTb().TacFilter("Children", Guids));

    [TestMethod]
    public void FilterIntArray()
        => AssertParams([$"{RuleParamPrefixFilter}Children=[2,7,42]"],
            NewTb().TacFilter("Children", new[] { 2, 7, 42 }));

}