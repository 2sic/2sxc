using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Apps;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Mocks;
public class ExecutionContextMock : ExecutionContext
{
    public ExecutionContextMock(App app, MyServices services) : base(services, "Mck")
    {
        app.Init(null, KnownAppsConstants.PresetIdentity.PureIdentity(), null);
        AttachApp(app);
    }
}
