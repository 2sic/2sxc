using ToSic.Sys.Logging;

namespace ToSic.Sxc.Dnn;

public class DnnLogFactoryRouteTests
{
    [Fact]
    public void Create_UsesLegacyLog_WithStartingEntry()
    {
        var log = IsType<Log>(new DnnBusinessController().Log);

        Contains(log.Entries, entry => entry.Message == "starting");
    }
}
