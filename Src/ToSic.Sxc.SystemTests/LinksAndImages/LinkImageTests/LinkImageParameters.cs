namespace ToSic.Sxc.LinksAndImages.LinkImageTests;


public class LinkImageParameters(LinkImageTestHelper helper)//: LinkImageTestBase
{
    [Fact]
    public void BasicParameters() =>
        Equal("test.jpg?name=daniel", helper.GetLinkHelper().Image("test.jpg", parameters: "name=daniel"));

    [Fact]
    public void KeyOnly() =>
        Equal("test.jpg?active", helper.GetLinkHelper().Image("test.jpg", parameters: "active"));

    [Fact]
    public void AddKeyToExisting() =>
        Equal("test.jpg?wx=200&active", helper.GetLinkHelper().Image("test.jpg?wx=200", parameters: "active"));

    [Fact]
    public void AddPairToExisting() =>
        Equal("test.jpg?wx=200&active=true", helper.GetLinkHelper().Image("test.jpg?wx=200", parameters: "active=true"));


    [Fact]
    public void AddPairToWh() =>
        Equal("test.jpg?w=200&h=200&active=true", helper.GetLinkHelper().Image("test.jpg", width: 200, height: 200, parameters: "active=true"));

    [Fact]
    public void AddPairToWhAndExisting() =>
        Equal("test.jpg?wx=700&w=200&h=200&active=true", helper.GetLinkHelper().Image("test.jpg?wx=700", width: 200, height: 200, parameters: "active=true"));
}