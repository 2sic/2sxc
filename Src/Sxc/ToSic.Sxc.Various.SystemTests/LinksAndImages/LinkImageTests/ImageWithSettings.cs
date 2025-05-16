namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class ImageWithSettings(LinkImageTestHelper helper)//: LinkImageTestBase
{
    [Theory]
    [InlineData(true)]
    //[InlineData(false)]
    public void BasicHWandAR(bool makeDyn)
    {
        var raw = new { Width = 200, Height = 300 };
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=300", "test.jpg", makeDyn ? helper.ToDyn(raw) : raw);

        var raw2 = new { Width = 200, Height = 300, AspectRatio = 1 };
        helper.TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", makeDyn ? helper.ToDyn(raw2) : raw2);

        // if h & ar are given, ar should take precedence
        var raw3 = new { Width = 200, Height = 300, AspectRatio = 1 };
        helper.TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", makeDyn ? helper.ToDyn(raw3) : raw3);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void BasicFormat(bool makeDyn)
    {
        var raw = new { Format = "webp" };
        // helper.TestOnLinkerAndHelper("test.jpg?format=webp", "test.jpg", format: "webp");
        helper.TestOnLinkerAndHelper("test.jpg?format=webp", "test.jpg", makeDyn ? helper.ToDyn(raw) : raw);
    }


    [Fact]
    public void SettingsWithOverride()
    {
        var settings = helper.ToDyn(new { Width = 200, Height = 300 });
        helper.TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);

        helper.TestOnLinkerAndHelper("test.jpg?w=700&h=300", "test.jpg", settings, width: 700);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=550", "test.jpg", settings, height: 550);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", settings, aspectRatio: 2);
        helper.TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);
        helper.TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", settings, height: 0);

        var settings2 = helper.ToDyn(new { Width = 200, AspectRatio = 1 });
        helper.TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings2);

        // if h & ar are given, ar should take precedence
        var settings3 = helper.ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
        helper.TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings3);
    }



}