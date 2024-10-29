using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class TweakMediaTests
{
    private static ITweakMedia ResizeOnly() => new TweakMedia(new(null, null, 100, 100, 2.5, 0.5, null, "not-based-on-anything"), null, null, null, null);

    [TestMethod]
    public void Width() => Assert.AreEqual(42, (ResizeOnly().Width(42) as TweakMedia).Resize.Width);

    [TestMethod]
    public void ResizeChangeMakesCopy()
    {
        var x = ResizeOnly();
        var y = x.Width(42);
        var z = x.Factor(0.75);
        Assert.AreNotEqual(x, y);
        Assert.AreNotEqual(x, z);
        Assert.AreNotEqual(y, z);
    }
}