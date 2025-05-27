using System;
using ToSic.Sxc.Services;
using static ToSic.Sxc.Services.UniqueKeysServices;
#pragma warning disable xUnit1026

namespace ToSic.Sxc.Tests.ContextTests;


public class UniqueKeyTests
{
    private UniqueKeysServices GetNew() => new();

    [Fact]
    public void UniqueKeyLength() => Equal(UniqueKeysServices.UniqueKeyLength, GetNew().UniqueKey.Length);

    public string TestKeyOf(object data) => UniqueKeyOf(data);
    public string TestKeysOf(params object[] data) => UniqueKeysOf(data);

    [Fact]
    public void Generate100AllDistinct()
    {
        var list = Enumerable.Range(0, 100).Select(i => GetNew()).ToList();
        Equal(100, list.Count);
        var distinct = list.Distinct().ToList();
        Equal(100, distinct.Count);
    }

    [Theory]
    [InlineData(NullValue, null, "null check")]
    [InlineData(PfxBool + "true", true)]
    [InlineData(PfxBool + "false", false)]
    [InlineData(PfxNum + "0", 0)]
    [InlineData(PfxNum + "1", 1)]
    [InlineData(PfxNum + "-1", -1)]
    [InlineData(PfxNum + "17_5", 17.5)]
    // TODO: strings, chars
    public void UniqueKeyOfValues(string expected, object data, string testName = default) => 
        Equal(expected, TestKeyOf(data));

#if NETFRAMEWORK
    [Theory]
    [InlineData(PfxString + "-327419862", "hello")]
    [InlineData(PfxString + "221721854", "abcdefg")]
    [InlineData(PfxString + "222795742", "Abcdefg")]
    public void UniqueKeyOfString(string expected, object data, string testName = default) => 
        Equal(expected, TestKeyOf(data));
#else
    // Note that .net 9.0 uses a more random hash-code which is not deterministic for strings!
    // https://learn.microsoft.com/en-us/dotnet/api/system.string.gethashcode?view=net-9.0
    // https://learn.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/userandomizedstringhashalgorithm-element
    [Theory]
    [InlineData("hello")]
    [InlineData("abcdefg")]
    [InlineData("Abcdefg")]
    public void UniqueKeyOfString(object data, string testName = default) => 
        Equal(PfxString + data.GetHashCode(), TestKeyOf(data));
#endif

    [Theory]
    [InlineData("20230824", "2023-08-24")]
    [InlineData("20230824063", "2023-08-24 06:30")]
    [InlineData("202308240645", "2023-08-24 06:45")]
    [InlineData("20230824000017", "2023-08-24 00:00:17")]
    [InlineData("202308240000001234", "2023-08-24 00:00:00.1234")]
    public void UniqueKeyOfDate(string expected, string date, string testName = default) => 
        Equal($"{PfxDate}{expected}", TestKeyOf(DateTime.Parse(date)));//, $"{date} ({testName})");

    [Theory]
    [InlineData("hFAeLybz", "2f1e5084-f326-4661-8176-305678db2230")]
    [InlineData("KgTK0W8g", "d1ca042a-206f-4a92-b74c-eabab90f0a80")]
    [InlineData("jt2C0tFj", "d282dd8e-63d1-4024-8f30-d9db23366478")]
    [InlineData("7N8R9ncf", "f611dfec-1f77-42f1-b789-d5038396a4c7")]
    [InlineData("2MlSvPdC", "bc52c9d8-42f7-4976-af37-650433eb9a7c")]
    [InlineData("AAAAAAAA", "00000000-0000-0000-0000-000000000000")]
    public void UniqueKeyOfGuid(string expected, string guid) => 
        Equal($"{PfxGuid}{expected}", TestKeyOf(Guid.Parse(guid)));


    [Fact]
    public void UniqueKeyOfNullableNumber() =>
        Equal($"{PfxNum}7", TestKeyOf((int?)7));

    [Fact]
    public void UniqueKeyOfNullableGuid() =>
        Equal($"{PfxGuid}hFAeLybz", TestKeyOf((Guid?)Guid.Parse("2f1e5084-f326-4661-8176-305678db2230")));

    #region UniqueKeys - Many

    [Theory]
    // Note: we always nee
    [InlineData("n1-n2-n3", 1, 2, 3)]
    [InlineData("btrue-n2-n3", true, 2, 3)]
    public void ManyKeys(string expected, params object[] data) => 
        Equal(expected, TestKeysOf(data));

    #endregion
}