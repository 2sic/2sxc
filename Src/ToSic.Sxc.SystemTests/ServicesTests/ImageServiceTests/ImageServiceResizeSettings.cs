using ToSic.Sxc.Images;
using ToSic.Sxc.Images.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.Internal.Url;
using Xunit.Sdk;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class ImageServiceResizeSettings(IImageService imgSvc) 
{
    /// <summary>
    /// Main internal test to see if the value is expected, and the rest of the values are not set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settings"></param>
    /// <param name="name"></param>
    /// <param name="getValue"></param>
    /// <param name="expected"></param>
    private void EqualsAndRestEmpty<T>(IResizeSettings settings, string name, Func<IResizeSettings, T> getValue, T expected)
    {
        Equal(expected, getValue(settings));
        AssertAllEmptyExceptSpecified(settings, name);
    }

    /// <summary>
    /// Make sure the test setup would throw an error if the value doesn't match
    /// </summary>
    [Fact]
    //[ExpectedException(typeof(AssertFailedException))]
    public void VerifyTestValue() =>
        Throws<EqualException>(() => EqualsAndRestEmpty(imgSvc.SettingsTac(width: 50), nameof(IResizeSettings.Width), s => s.Width, 100));

    /// <summary>
    /// Make sure the test setup would throw an error if the wrong ones are empty
    /// </summary>
    [Fact]
    //[ExpectedException(typeof(AssertFailedException))]
    public void VerifyTestRestEmpty() =>
        Throws<EqualException>(() => EqualsAndRestEmpty(imgSvc.SettingsTac(width: 50), nameof(IResizeSettings.Height), s => s.Width, 100));

    [Fact]
    public void EmptyOnlyWidth_Param() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(width: 100), nameof(IResizeSettings.Width), s => s.Width, 100);

    [Fact]
    public void EmptyOnlyWidth_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Width(100)), nameof(IResizeSettings.Width), s => s.Width, 100);

    [Fact]
    public void EmptyOnlyWidth_Mix() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(width: 42, tweak: t => t.Width(100)), nameof(IResizeSettings.Width), s => s.Width, 100);

    [Fact]
    public void EmptyOnlyHeight_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(height: 100), nameof(IResizeSettings.Height), s => s.Height, 100);

    [Fact]
    public void EmptyOnlyHeight_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Height(100)), nameof(IResizeSettings.Height), s => s.Height, 100);

    [Fact]
    public void EmptyOnlyFormat_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(format: "jpg"), nameof(IResizeSettings.Format), s => s.Format, "jpg");

    [Fact]
    public void EmptyOnlyFormat_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Format("jpg")), nameof(IResizeSettings.Format), s => s.Format, "jpg");

    [Fact]
    public void EmptyOnlyResizeMode_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(resizeMode: ImageConstants.ModeCrop), nameof(IResizeSettings.ResizeMode), s => s.ResizeMode, ImageConstants.ModeCrop);

    [Fact]
    public void EmptyOnlyResizeMode_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.ResizeMode(ImageConstants.ModeCrop)), nameof(IResizeSettings.ResizeMode), s => s.ResizeMode, ImageConstants.ModeCrop);

    [Fact]
    public void EmptyOnlyScaleMode_Params()
    {
        // todo: use constants for the final result
        EqualsAndRestEmpty(imgSvc.SettingsTac(scaleMode: "up"), nameof(IResizeSettings.ScaleMode), s => s.ScaleMode, "upscaleonly");
    }

    [Fact]
    public void EmptyOnlyScaleMode_Tweak()
    {
        // todo: use constants for the final result
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.ScaleMode("up")), nameof(IResizeSettings.ScaleMode), s => s.ScaleMode, "upscaleonly");
    }

    [Fact]
    public void EmptyOnlyParameters_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(parameters: "count=17"), nameof(IResizeSettings.Parameters), s => s.Parameters.NvcToString(), "count=17");

    [Fact]
    public void EmptyOnlyParameters_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Parameters("count=17")), nameof(IResizeSettings.Parameters), s => s.Parameters.NvcToString(), "count=17");

    [Fact]
    public void EmptyOnlyQuality75_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(quality: 75), nameof(IResizeSettings.Quality), s => s.Quality, 75);

    [Fact]
    public void EmptyOnlyQuality75_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Quality(75)), nameof(IResizeSettings.Quality), s => s.Quality, 75);

    [Fact]
    public void EmptyOnlyQualityDot75_Params() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(quality: .75f), nameof(IResizeSettings.Quality), s => s.Quality, 75);

    [Fact]
    public void EmptyOnlyQualityDot75_Tweak() =>
        EqualsAndRestEmpty(imgSvc.SettingsTac(tweak: t => t.Quality(75f)), nameof(IResizeSettings.Quality), s => s.Quality, 75);

    // 2022-03-17 not valid any more, this property will be removed
    //[Fact]
    //public void EmptyOnlySrcSet()
    //{
    //    var settings = Build<IImageService>().ResizeSettings(rules: new MultiResizeRule {SrcSet = "100,200,300" });
    //    Assert.Equal("100,200,300", settings.SrcSet);
    //    AssertAllEmptyExceptSpecified(settings, nameof(settings.SrcSet));
    //}



    [Fact]
    public void EmptyWidthAndHeight()
    {
        var settings = imgSvc.SettingsTac(width: 100, height: 49);
        Equal(100, settings.Width);
        Equal(49, settings.Height);
        AssertAllEmptyExceptSpecified(settings, [nameof(settings.Width), nameof(settings.Height)]);
    }



    private void AssertAllEmptyExceptSpecified(IResizeSettings settings, string nameToSkip)
        => AssertAllEmptyExceptSpecified(settings, [nameToSkip]);

    private void AssertAllEmptyExceptSpecified(IResizeSettings settings, string[] namesToSkip)
    {
        var count = 0;
        count += MaybeTestOneProperty(settings.Height, 0, namesToSkip, nameof(settings.Height));
        count += MaybeTestOneProperty(settings.Width, 0, namesToSkip, nameof(settings.Width));
        count += MaybeTestOneProperty(settings.Format, null, namesToSkip, nameof(settings.Format));
        count += MaybeTestOneProperty(settings.ResizeMode, null, namesToSkip, nameof(settings.ResizeMode));
        count += MaybeTestOneProperty(settings.Parameters, null, namesToSkip, nameof(settings.Parameters));
        count += MaybeTestOneProperty(settings.Quality, 0, namesToSkip, nameof(settings.Quality));
        count += MaybeTestOneProperty(settings.ScaleMode, null, namesToSkip, nameof(settings.ScaleMode));
        // 2022-03-17 2dm removing this property
        //count += MaybeTestOneProperty(settings.SrcSet, null, namesToSkip, nameof(settings.SrcSet));

        // Verify the total count matches expected
        const int expectedCount = 7;
        Equal(expectedCount - namesToSkip.Length, count);//, "count should be total fields minus untested");
    }

    private static int MaybeTestOneProperty<T>(T actual, T expected, string[] namesToSkip, string notToTest)
    {
        if (namesToSkip.Any(n => n == notToTest)) return 0;
        Equal(expected, actual);//, notToTest);
        return 1;

    }
}