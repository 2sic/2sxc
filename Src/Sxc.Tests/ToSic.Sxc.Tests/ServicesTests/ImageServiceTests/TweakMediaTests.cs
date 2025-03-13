using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

/// <summary>
/// Tests to check that the TweakMedia API works as expected.
/// </summary>
[TestClass]
public class TweakMediaTests
{
    #region Resize Settings - not implemented, as we should probably have an entire Resize(name, t => ...) API for this instead

    /// <summary>
    /// Create a TweakMedia with Resize settings for testing changes on the resize.
    /// </summary>
    private static ITweakMedia ResizeOnly() => new TweakMedia(
        null,
        null,
        new(null, null, 100, 100, 2.5, 0.5, null, "not-based-on-anything"),
        null,
        null,
        null,
        null
    );

    [TestMethod]
    public void ChangeWidth() => AreEqual(42, ((TweakMedia)ResizeOnly().Resize(null as string, tweak: t => t.Width(42))).ResizeSettings.Width);

    [TestMethod]
    public void ResizeChangeMakesCopyOfTweakMedia()
    {
        var original = ResizeOnly();
        var changeWidth = original.Resize(null as string, tweak: t => t.Width(42));
        var changeFactor = original.Resize(null as string, tweak: t => t.Factor(0.75));
        // Verify that the objects are different
        AreNotEqual(original, changeWidth);
        AreNotEqual(original, changeFactor);
        AreNotEqual(changeWidth, changeFactor);
    }

    #endregion
}