using static ToSic.Sxc.Edit.Toolbar.Internal.ToolbarConstants;

namespace ToSic.Sxc.Tests.ServicesTests.ToolbarService;


public class TweakButtonParameters: TweakButtonTestsBase
{

    [Fact]
    public void FormParameters1String()
        => AssertParams([$"{RuleParamPrefixForm}Hello"], NewTb().TacFormParameters("Hello"));

    [Fact]
    public void FormParameters2Strings()
        => AssertParams([$"{RuleParamPrefixForm}Hello=World"], NewTb().TacFormParameters("Hello", "World"));

    [Fact]
    public void FormParameters1ObjectA()
        => AssertParams([$"{RuleParamPrefixForm}hello=world"], NewTb().TacFormParameters(new { hello = "world"}));

    [Fact]
    public void FormParameters1ObjectB()
        => AssertParams([$"{RuleParamPrefixForm}hello=world&{RuleParamPrefixForm}name=iJungleboy"], NewTb().TacFormParameters(new { hello = "world", name = "iJungleboy"}));

    [Fact]
    public void Parameters1String()
        => AssertParams(["Hello"], NewTb().TacParameters("Hello"));

    [Fact]
    public void Parameters2Strings()
        => AssertParams(["Hello=World"], NewTb().TacParameters("Hello", "World"));

    [Fact]
    public void Parameters2Int()
        => AssertParams(["Hello=42"], NewTb().TacParameters("Hello", 42));
    [Fact]
    public void Parameters2Bool()
        => AssertParams(["Hello=true"], NewTb().TacParameters("Hello", true));

    [Fact]
    public void Parameters1ObjectA()
        => AssertParams([new { hello = "world" }], NewTb().TacParameters(new { hello = "world"}));

    [Fact]
    public void Parameters1ObjectB()
        => AssertParams([new { hello = "world", name = "iJungleboy" }], NewTb().TacParameters(new { hello = "world", name = "iJungleboy"}));

    [Fact]
    public void Prefill1String()
        => AssertParams([$"{RuleParamPrefixPrefill}Hello"], NewTb().TacPrefill("Hello"));

    [Fact]
    public void Prefill2Strings()
        => AssertParams([$"{RuleParamPrefixPrefill}Hello=World"], NewTb().TacPrefill("Hello", "World"));

    private static readonly Guid Guid1 = new("640df5cf-ec2b-4943-9962-5a98bb1e8d01");
    private static readonly Guid Guid2 = new("7b42eae8-2ead-479e-af67-8e1349939ed6");
    private static readonly Guid[] Guids = [Guid1, Guid2];
    private static readonly string GuidsExpected = $"[\"{Guid1}\",\"{Guid2}\"]";

    [Fact]
    public void PrefillGuidArray()
        => AssertParams([$"{RuleParamPrefixPrefill}Children={GuidsExpected}"], NewTb().TacPrefill("Children", Guids));

    // todo: also prefill w/second param being an object

    [Fact]
    public void Prefill1ObjectA()
        => AssertParams([$"{RuleParamPrefixPrefill}hello=world"], NewTb().TacPrefill(new { hello = "world"}));

    [Fact]
    public void Prefill1ObjectB()
        => AssertParams([$"{RuleParamPrefixPrefill}hello=world&{RuleParamPrefixPrefill}name=iJungleboy"], NewTb().TacPrefill(new { hello = "world", name = "iJungleboy"}));

    // TODO: filters must test for arrays of string, int, guid
    [Fact]
    public void Filter1Object()
    => AssertParams([$"{RuleParamPrefixFilter}hello=world"], NewTb().TacFilter(new { hello = "world"}));

    [Fact]
    public void Filter2Objects()
        => AssertParams([$"{RuleParamPrefixFilter}hello=world&{RuleParamPrefixFilter}name=iJungleboy"], NewTb().TacFilter(new { hello = "world", name = "iJungleboy"}));

    [Fact]
    public void Filter1String()
        => AssertParams([$"{RuleParamPrefixFilter}Hello"], NewTb().TacFilter("Hello"));

    [Fact]
    public void Filter2Strings()
        => AssertParams([$"{RuleParamPrefixFilter}Hello=World"], NewTb().TacFilter("Hello", "World"));

    [Fact]
    public void FilterGuidArray()
        => AssertParams([$"{RuleParamPrefixFilter}Children={GuidsExpected}"], NewTb().TacFilter("Children", Guids));

    [Fact]
    public void FilterIntArray()
        => AssertParams([$"{RuleParamPrefixFilter}Children=[2,7,42]"],
            NewTb().TacFilter("Children", new[] { 2, 7, 42 }));

}