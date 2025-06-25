using ToSic.Sxc.Sys.Render.PageContext;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.PageProperty;


public class UpdatePropertyTest
{
    protected const string Suffix = " - Blog - MySite";

    private string UpdatePropertyTestAccessor(string original, PagePropertyChange change) =>
        Helpers.UpdateProperty(original, change);

    [Fact]
    public void PlaceholderSimple()
    {
        var result = UpdatePropertyTestAccessor("[placeholder]" + Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("My Title" + Suffix, result);
    }

    [Fact]
    public void PlaceholderEnd()
    {
        var result = UpdatePropertyTestAccessor(Suffix + "[placeholder]",
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal(Suffix + "My Title", result);
    }

    [Fact]
    public void PlaceholderMiddle()
    {
        var result = UpdatePropertyTestAccessor("Before-[placeholder]-After",
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("Before-My Title-After", result);
    }

    [Fact]
    public void PlaceholderOtherCase()
    {
        var result = UpdatePropertyTestAccessor("[PlaceHolder]" + Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("My Title" + Suffix, result);
    }

    [Fact]
    public void PlaceholderOnly()
    {
        var result = UpdatePropertyTestAccessor("[PlaceHolder]",
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("My Title", result);
    }

    [Fact]
    public void PlaceholderNotFound()
    {
        var result = UpdatePropertyTestAccessor(Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("My Title", result);
    }

    [Fact]
    public void PlaceholderNotFoundReplace()
    {
        var result = UpdatePropertyTestAccessor(Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Replace });
        Equal("My Title", result);
    }

    [Fact]
    public void PlaceholderNotFoundPrepend()
    {
        var result = UpdatePropertyTestAccessor(Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Prepend });
        Equal("My Title" + Suffix, result);
    }

    [Fact]
    public void PlaceholderNotFoundAppend()
    {
        var result = UpdatePropertyTestAccessor(Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Append });
        Equal(Suffix + "My Title", result);
    }
    [Fact]
    public void PlaceholderNotFoundAuto()
    {
        var result = UpdatePropertyTestAccessor(Suffix,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title", ChangeMode = PageChangeModes.Auto });
        Equal("My Title", result);
    }

    [Fact]
    public void NullOriginal()
    {
        var result = UpdatePropertyTestAccessor(null,
            new() { ReplacementIdentifier = "[placeholder]", Value = "My Title" });
        Equal("My Title", result);
    }

    [Fact]
    public void ValueNull()
    {
        var result = UpdatePropertyTestAccessor("Some Title",
            new() { ReplacementIdentifier = "[placeholder]", Value = null });
        Equal("Some Title", result);
    }

    [Fact]
    public void AllNull()
    {
        var result = UpdatePropertyTestAccessor(null,
            new() { ReplacementIdentifier = "[placeholder]", Value = null });
        Equal(null, result);
    }

    [Fact]
    public void ValueEmpty()
    {
        var result = UpdatePropertyTestAccessor("Some Title",
            new() { ReplacementIdentifier = "[placeholder]", Value = "" });
        Equal("", result);
    }
}