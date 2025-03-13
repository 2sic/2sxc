namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests;

[TestClass]
public class ImageWithSettings: LinkImageTestBase
{
    [DataRow(true)]
    //[DataRow(false)]
    [TestMethod]
    public void BasicHWandAR(bool makeDyn)
    {
        var raw = new { Width = 200, Height = 300 };
        TestOnLinkerAndHelper("test.jpg?w=200&h=300", "test.jpg", makeDyn ? ToDyn(raw) : raw);

        var raw2 = new { Width = 200, Height = 300, AspectRatio = 1 };
        TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", makeDyn ? ToDyn(raw2) : raw2);

        // if h & ar are given, ar should take precedence
        var raw3 = new { Width = 200, Height = 300, AspectRatio = 1 };
        TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", makeDyn ? ToDyn(raw3) : raw3);
    }

    [DataRow(true)]
    [DataRow(false)]
    [TestMethod]
    public void BasicFormat(bool makeDyn)
    {
        var raw = new { Format = "webp" };
        // TestOnLinkerAndHelper("test.jpg?format=webp", "test.jpg", format: "webp");
        TestOnLinkerAndHelper("test.jpg?format=webp", "test.jpg", makeDyn ? ToDyn(raw) : raw);
    }


    [TestMethod]
    public void SettingsWithOverride()
    {
        var settings = ToDyn(new { Width = 200, Height = 300 });
        TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);

        TestOnLinkerAndHelper("test.jpg?w=700&h=300", "test.jpg", settings, width: 700);
        TestOnLinkerAndHelper("test.jpg?w=200&h=550", "test.jpg", settings, height: 550);
        TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", settings, aspectRatio: 2);
        TestOnLinkerAndHelper("test.jpg?h=300", "test.jpg", settings, width: 0);
        TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", settings, height: 0);

        var settings2 = ToDyn(new { Width = 200, AspectRatio = 1 });
        TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings2);

        // if h & ar are given, ar should take precedence
        var settings3 = ToDyn(new { Width = 200, Height = 300, AspectRatio = 1 });
        TestOnLinkerAndHelper("test.png?w=200&h=200", "test.png", settings3);
    }



}