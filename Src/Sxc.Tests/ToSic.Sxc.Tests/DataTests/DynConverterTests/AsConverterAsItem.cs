using System;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Tests.DataTests.DynConverterTests;

[TestClass]
public class AsConverterAsItem: TestBaseSxcDb
{
    public CodeDataFactory Cdf => field ??= GetService<CodeDataFactory>();

    [TestMethod]
    public void AsItemWithFakeOk()
    {
        var item = Cdf.AsItemTac(Cdf.FakeEntityTac(0), propsRequired: true);
        IsNotNull(item);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AsItemWithAnonFail()
    {
        var data = new
        {
            Title = "This is a title",
            Birthday = new DateTime(2012, 02, 07)
        };

        var item = Cdf.AsItemTac(data, propsRequired: true);
    }
}