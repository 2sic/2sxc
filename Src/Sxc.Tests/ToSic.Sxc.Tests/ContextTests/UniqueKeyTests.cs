using System;
using ToSic.Sxc.Services;
using static ToSic.Sxc.Services.UniqueKeysServices;

namespace ToSic.Sxc.Tests.ContextTests;

[TestClass]
public class UniqueKeyTests
{
    private UniqueKeysServices GetNew() => new();

    [TestMethod]
    public void UniqueKeyLength() => AreEqual(UniqueKeysServices.UniqueKeyLength, GetNew().UniqueKey.Length);

    public string TestKeyOf(object data) => UniqueKeyOf(data);
    public string TestKeysOf(params object[] data) => UniqueKeysOf(data);

    [TestMethod]
    public void Generate100AllDistinct()
    {
        var list = Enumerable.Range(0, 100).Select(i => GetNew()).ToList();
        AreEqual(100, list.Count);
        var distinct = list.Distinct().ToList();
        AreEqual(100, distinct.Count);
    }

    [TestMethod]
    [DataRow(NullValue, null, "null check")]
    [DataRow(PfxBool + "true", true)]
    [DataRow(PfxBool + "false", false)]
    [DataRow(PfxNum + "0", 0)]
    [DataRow(PfxNum + "1", 1)]
    [DataRow(PfxNum + "-1", -1)]
    [DataRow(PfxNum + "17_5", 17.5)]
    // TODO: strings, chars
    public void UniqueKeyOfValues(string expected, object data, string testName = default) => 
        AreEqual(expected, TestKeyOf(data));

#if NETFRAMEWORK
    [DataRow(PfxString + "-327419862", "hello")]
    [DataRow(PfxString + "221721854", "abcdefg")]
    [DataRow(PfxString + "222795742", "Abcdefg")]
    [TestMethod]
    public void UniqueKeyOfString(string expected, object data, string testName = default) => 
        AreEqual(expected, TestKeyOf(data));
#else
    // Note that .net 9.0 uses a more random hash-code which is not deterministic for strings!
    // https://learn.microsoft.com/en-us/dotnet/api/system.string.gethashcode?view=net-9.0
    // https://learn.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/userandomizedstringhashalgorithm-element
    [DataRow("hello")]
    [DataRow("abcdefg")]
    [DataRow("Abcdefg")]
    [TestMethod]
    public void UniqueKeyOfString(object data, string testName = default) => 
        AreEqual(PfxString + data.GetHashCode(), TestKeyOf(data));
#endif

    [TestMethod]
    [DataRow("20230824", "2023-08-24")]
    [DataRow("20230824063", "2023-08-24 06:30")]
    [DataRow("202308240645", "2023-08-24 06:45")]
    [DataRow("20230824000017", "2023-08-24 00:00:17")]
    [DataRow("202308240000001234", "2023-08-24 00:00:00.1234")]
    public void UniqueKeyOfDate(string expected, string date, string testName = default) => 
        AreEqual($"{PfxDate}{expected}", TestKeyOf(DateTime.Parse(date)), $"{date} ({testName})");

    [TestMethod]
    [DataRow("hFAeLybz", "2f1e5084-f326-4661-8176-305678db2230")]
    [DataRow("KgTK0W8g", "d1ca042a-206f-4a92-b74c-eabab90f0a80")]
    [DataRow("jt2C0tFj", "d282dd8e-63d1-4024-8f30-d9db23366478")]
    [DataRow("7N8R9ncf", "f611dfec-1f77-42f1-b789-d5038396a4c7")]
    [DataRow("2MlSvPdC", "bc52c9d8-42f7-4976-af37-650433eb9a7c")]
    [DataRow("AAAAAAAA", "00000000-0000-0000-0000-000000000000")]
    public void UniqueKeyOfGuid(string expected, string guid) => 
        AreEqual($"{PfxGuid}{expected}", TestKeyOf(Guid.Parse(guid)));


    [TestMethod]
    public void UniqueKeyOfNullableNumber() =>
        AreEqual($"{PfxNum}7", TestKeyOf((int?)7));

    [TestMethod]
    public void UniqueKeyOfNullableGuid() =>
        AreEqual($"{PfxGuid}hFAeLybz", TestKeyOf((Guid?)Guid.Parse("2f1e5084-f326-4661-8176-305678db2230")));

    #region UniqueKeys - Many

    [TestMethod]
    // Note: we always nee
    [DataRow("n1-n2-n3", 1, 2, 3)]
    [DataRow("btrue-n2-n3", true, 2, 3)]
    public void ManyKeys(string expected, params object[] data) => 
        AreEqual(expected, TestKeysOf(data));

    #endregion
}