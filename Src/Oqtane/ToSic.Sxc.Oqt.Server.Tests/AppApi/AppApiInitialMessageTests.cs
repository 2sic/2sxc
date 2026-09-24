using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sys.Logging;
using ToSic.Sys.Run.Startup;
using Xunit;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.AppApi;

public class AppApiInitialMessageTests
{
    [Fact]
    public void ActionInvoker_InitialMessageAppearsInAdmittedAppApiSnapshot()
    {
        var services = new ServiceCollection();
        services.AddSysCoreMelInsightsLogging();
        using var provider = services.BuildServiceProvider();

        var invoker = ActivatorUtilities.CreateInstance<AppApiActionInvoker>(provider);
        invoker.Log.A("after constructor");

        var group = Single(provider.GetRequiredService<IInsightsLogSnapshotReader>().ListGroups());
        Contains("app-api", group.Segments);
        Equal(["AppApiActionInvoker", "after constructor"], group.Events.Select(entry => entry.Message));
    }
}
