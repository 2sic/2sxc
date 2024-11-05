using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class TweakMediaTests
{
    #region Resize Settings - not implemented, as we should probably have an entire Resize(name, t => ...) API for this instead

    private static ITweakMedia ResizeOnly() => new TweakMedia(null, null, new(null, null, 100, 100, 2.5, 0.5, null, "not-based-on-anything"), null, null, null, null);

    [TestMethod]
    public void Width() => Assert.AreEqual(42, (ResizeOnly().Resize(null as string, tweak: t => t.Width(42)) as TweakMedia).ResizeSettings.Width);

    [TestMethod]
    public void ResizeChangeMakesCopy()
    {
        var x = ResizeOnly();
        var y = x.Resize(null as string, tweak: t => t.Width(42));
        var z = x.Resize(null as string, tweak: t => t.Factor(0.75));
        Assert.AreNotEqual(x, y);
        Assert.AreNotEqual(x, z);
        Assert.AreNotEqual(y, z);
    }

    #endregion
}