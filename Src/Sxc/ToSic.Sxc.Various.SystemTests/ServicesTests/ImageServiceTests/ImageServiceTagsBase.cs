using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

public abstract class ImageServiceTagsBase(IImageService svc, ITestOutputHelper output, TestScenario? testScenario = default)
{
    protected const string ImgUrl = "/abc/def/test.jpg";
    protected const string Img120x24 = ImgUrl + "?w=120&amp;h=24";
    protected const string Img120x24x = Img120x24 + " 1x";
    protected const string Img240x48 = ImgUrl + "?w=240&amp;h=48";
    protected const string Img240x48x = Img240x48 + " 2x";
    protected const string SrcSet12 = "1,2";
    protected const string SrcSet12ResultWebP = "srcset='" + Img120x24 + "&amp;format=webp 1x," + ImgUrl + "?w=240&amp;h=48&amp;format=webp 2x'";
    protected const string SrcSet12ResultJpg = "srcset='" + Img120x24x + "," + Img240x48x + "'";
    protected const string SrcWebP12 = "<source type='image/webp' " + SrcSet12ResultWebP + ">";
    protected const string SrcJpg12 = "<source type='image/jpeg' " + SrcSet12ResultJpg + ">";
    protected const string ImgTagJpg12 = "<img src='" + Img120x24 + "' " + SrcSet12ResultJpg + ">";

    protected const string SrcSetNone = null;
    protected const string SrcWebPNone = "<source type='image/webp' srcset='" + Img120x24 + "&amp;format=webp'>";
    protected const string SrcJpgNone = "<source type='image/jpeg' srcset='" + Img120x24 + "'>";
    protected const string ImgTagJpgNone = "<img src='" + Img120x24 + "'>";
    protected const string ImgTagJpgNoneF05 = "<img src='" + ImgUrl + "?w=60&amp;h=12'>";

    #region Tests which both Img and Picture should do

    protected virtual bool TestModeImg => true;

    [Fact]
    public void UrlResized()
    {
        
        var url = TestModeImg ? svc.Img(ImgUrl).Src : svc.Picture(ImgUrl).Src;
        Equal(ImgUrl, url);
    }

    [Fact]
    public void ImgAlt()
    {
        const string imgAlt = "test-alt";
        
        var result = TestModeImg 
            ? svc.Img(ImgUrl, imgAlt: imgAlt).ToString()
            : svc.Picture(ImgUrl, imgAlt: imgAlt).Img.ToString();
        Equal($"<img src='{ImgUrl}' alt='{imgAlt}'>", result);
    }

    [Fact]
    public void ImgClass()
    {
        
        var cls = "class-dummy";
        var result = TestModeImg
            ? svc.Img(ImgUrl, imgClass: cls).ToString()
            : svc.Picture(ImgUrl, imgClass: cls).Img.ToString();
        Equal($"<img src='{ImgUrl}' class='{cls}'>", result);
    }

    #endregion


    protected void PictureTagInner(string expectedParts, string variants, bool inPicTag, string testName)
    {
        
        var rule = new Recipe(variants: variants);
        var settings = svc.SettingsTac(width: 120, height: 24, recipe: inPicTag ? null : rule);
        var pic = svc.Picture(link: ImgUrl, settings: settings, recipe: inPicTag ? rule : null);

        var expected = $"<picture>{expectedParts}<img src='{Img120x24}'></picture>";
        output.WriteLine($"Expected: '{expected}'");
        output.WriteLine($"Actual  : '{pic}'");
        Equal(expected, pic.ToString());//, $"Test failed: {testName}");
    }


    /// <summary>
    /// Run a batch of tests on the source tags, with permutations of where the settings are given
    /// </summary>
    protected void BatchTestManySrcSets(string expected, string variants, string testName)
    {
        var testSet = ImageTagsTestPermutations.GenerateTestParams(testName, variants);
        TestManyButThrowOnceOnly(testSet.Select(ts => (ts.Name, ts)), test =>
        {
            output.WriteLine($"Test: {test}");

            
            var settings = svc.SettingsTac(width: test.Set.Width, height: test.Set.Height,
                recipe: test.Set.SrcSetRule);
            var pic = svc.Picture(link: ImgUrl, settings: settings, recipe: test.Pic.SrcSetRule).Sources;

            output.WriteLine($"Expected: '{expected}'");
            output.WriteLine($"Actual  : '{pic}'");
            Equal(expected, pic.ToString());//, $"Failed: {test.Name}");

        });
    }



    public void TestManyButThrowOnceOnly<T>(IEnumerable<(string Name, T Data)> tests, Action<T> innerCall)
    {
        Exception lastException = null;
        foreach (var (name, data) in tests)
        {
            output.WriteLine(name);
            try
            {
                innerCall(data);
                output.WriteLine(" ok");
            }
            catch (Exception ex)
            {
                output.WriteLine(" error");
                Console.WriteLine(ex);
                lastException = ex;
            }

        }
        if (lastException != null) throw lastException;
    }
}