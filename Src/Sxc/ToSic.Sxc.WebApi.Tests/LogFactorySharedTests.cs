using ToSic.Sxc.Web.Sys.Html;
using static Xunit.Assert;

namespace ToSic.Sxc.WebApi.Tests;

public class LogFactorySharedTests
{
    [Fact]
    public void Create_UsesLegacyLog_WithoutParent()
        => IsType<Log>(new TestHtmlStringLog("Tst.Html").Log);

    [Fact]
    public void Create_UsesLegacyLog_WithParent()
    {
        var root = new Log("Tst.Root");
        var log = IsType<Log>(new TestHtmlStringLog(root, "Tst.Html").Log);

        Same(root, log.Parent);
    }

    private sealed record TestHtmlStringLog : HybridHtmlStringLog
    {
        public TestHtmlStringLog(string logName) : base(logName) { }

        public TestHtmlStringLog(ILog parentLog, string logName) : base(parentLog, logName) { }
    }
}
