using ToSic.Sxc.Oqt.Server.Integration;

namespace ToSic.Sxc.Oqt.Integration;

public class OqtFolderHelperTests
{
    [Fact]
    public void EnsureOqtaneFolderFormat_UsesForwardSlashes()
    {
        var result = @"Content\Tenants/1\Sites/2".EnsureOqtaneFolderFormat();

        Equal("Content/Tenants/1/Sites/2/", result);
    }
}
