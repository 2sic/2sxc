using ToSic.Eav.Security.Files;

namespace ToSic.Sxc.Tests.Adam;


public class AdamSecurity
{
    private readonly string[] _badFiles =
    [
        "hello.exe",
        "unhappy.exe ",
        "unhappy 2. exe",
        "bad.cshtml",
        "_notgood.cshtml",
        "trying to be smart.jpg.js"
    ];
        
    private readonly string[] _goodFiles =
    [
        "hello.doc",
        "good.jpg",
        "_notbad.png",
        "this is a dot. and we love it.txt",
        "list of flowers.csv"
    ];

    [Fact]
    public void BadExtensionsCaught()
    {
        var exts = _badFiles.Select(System.IO.Path.GetExtension).ToList();

        exts.ForEach(e =>
            True( FileNames.IsKnownRiskyExtension(e), $"expected {e} to be marked as bad")
        );
    }


    [Fact]
    public void GoodExtensionsNotCaught()
    {
        var exts = _goodFiles.Select(System.IO.Path.GetExtension).ToList();

        exts.ForEach(e =>
            False(FileNames.IsKnownRiskyExtension(e), $"expected {e} to be marked as good")
        );
    }

}