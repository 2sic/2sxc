using System.Globalization;

namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class ImageBasic(LinkImageTestHelper helper)// : LinkImageTestBase
{
    [Fact]
    public void UrlOnly()
    {
        var urls = new[]
        {
            "test.jpg",
            "test.png",
            "/test.jpg",
            "//test.jpg",
            "http://www.2sxc.org/test.jpg",
            "weird-extension.abc"
        };

        foreach (var url in urls) helper.TestOnLinkerAndHelper(url, url);
    }

    [Fact]
    public void BadCharacters()
    {
        helper.TestOnLinkerAndHelper("test%20picture.jpg?w=200", "test picture.jpg", width: 200);
        helper.TestOnLinkerAndHelper("gr%C3%A4%C3%9Flich.jpg?h=200", "gräßlich.jpg", height: 200);
        helper.TestOnLinkerAndHelper("test%20picture.jpg?x=chuchich%C3%A4schtly&w=200", "test picture.jpg?x=chuchichäschtly", width: 200);
    }


    [Fact]
    public void BasicWidthAndHeight()
    {
        helper.TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200);
        helper.TestOnLinkerAndHelper("test.jpg?h=200", "test.jpg", height: 200);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, height:200);
    }

    [Fact]
    public void BasicWidthAndAspectRatio()
    {
        helper.TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: 0);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: 1);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: 0.5);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: 2);
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: 2.5);
            
        // Note: in this case it should be 112.5 and will be rounded down by default
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=112", "test.jpg", width: 200, aspectRatio: 16f/9);
    }

    [Fact]
    public void BasicWidthAndAspectRatioString()
    {
        // Simple Strings
        helper.TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: "0");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
    }

    [Fact]
    public void BasicWidthAndAspectRatioCommaBadCulture()
    {
        // test before setting culture
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0,5");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");

        // Now set culture and run again
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "0.5");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
    }

    [Fact]
    public void BasicWidthAndAspectRatioStringWithSeparator()
    {
        // Simple Strings
        helper.TestOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: "0");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1:1");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: "1/1");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "1:2");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: "1/2");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2:1");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2/1");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: "2");
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: "2.5");
            
        // Note: in this case it should be 112.5 and will be rounded down by default
        helper.TestOnLinkerAndHelper("test.jpg?w=200&h=112", "test.jpg", width: 200, aspectRatio: "16/9");
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ErrorHeightAndAspectRatio()
    {
        var linker = helper.GetLinker();
        Throws<ArgumentOutOfRangeException>(() => linker.ImageUrl("test.jpg", height: 200, aspectRatio: 1));
    }


}