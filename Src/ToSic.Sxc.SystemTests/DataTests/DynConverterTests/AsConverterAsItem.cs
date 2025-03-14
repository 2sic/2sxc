﻿using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.DataTests.DynConverterTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class AsConverterAsItem(CodeDataFactory cdf)//: TestBaseSxcDb
{
    //public CodeDataFactory Cdf => field ??= GetService<CodeDataFactory>();

    [Fact]
    public void AsItemWithFakeOk()
    {
        var item = cdf.AsItemTac(cdf.FakeEntityTac(0), propsRequired: true);
        NotNull(item);
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void AsItemWithAnonFail()
    {
        var data = new
        {
            Title = "This is a title",
            Birthday = new DateTime(2012, 02, 07)
        };

        Throws<ArgumentException>(() =>
        {
            var item = cdf.AsItemTac(data, propsRequired: true);
        });
    }
}