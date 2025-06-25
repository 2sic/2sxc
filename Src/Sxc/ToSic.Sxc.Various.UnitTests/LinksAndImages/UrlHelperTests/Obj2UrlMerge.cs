namespace ToSic.Sxc.Tests.LinksAndImages.UrlHelperTests;


public class Obj2UrlMerge
{
    // Test accessor
    private string SerializeWithChild(object main, object child) =>
        new ObjectToUrl().SerializeWithChild(main, child, prefix);

    private const string prefix = "prefix:";

    [Theory]
    [InlineData((string)null)]
    [InlineData("")]
    [InlineData("icon=hello")]
    [InlineData("icon=hello&value=2")]
    public void FirstOnlyString(string ui)
    {
        Equal(ui, SerializeWithChild(ui, null));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("prefix:icon=hello", "icon=hello")]
    [InlineData("prefix:icon=hello&prefix:value=2", "icon=hello&value=2")]
    public void ChildOnlyString(string exp, string child)
    {
        Equal(exp, SerializeWithChild(null, child));
    }

    [Fact]
    public void MainObjectChildString() 
        => Equal("id=27&name=daniel&prefix:title=title2", SerializeWithChild(new { id = 27, name = "daniel" }, "title=title2"));

    [Fact]
    public void MainStringChildString() 
        => Equal("id=27&name=daniel&prefix:title=title2", SerializeWithChild("id=27&name=daniel", "title=title2"));

    [Fact]
    public void MainStringChildObject() 
        => Equal("id=27&name=daniel&prefix:title=title2", SerializeWithChild("id=27&name=daniel", new { title = "title2"}));

    [Fact]
    public void MainObjectChildObject() 
        => Equal("id=27&name=daniel&prefix:title=title2", SerializeWithChild(new { id = 27, name = "daniel" }, new { title = "title2"}));
}