using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Mocks;
public class ExecutionContextMock : ExecutionContext
{
    public ExecutionContextMock(Generator<App, AppForCodeShared.Options> appGen, Dependencies services) : base(services, "Mck")
    {
        var app = appGen.New(new(null, KnownAppsConstants.PresetIdentity.PureIdentity(), null));
        AttachApp(app);
    }
}
