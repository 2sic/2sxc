using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Code.Context.Sys;

namespace ToSic.Sxc.Mocks;
public class CodeContextMock : CodeContext
{
    public CodeContextMock(Generator<AppForCode, AppForCode.Options> appGen, Dependencies services) : base(services, "Mck")
    {
        var app = appGen.New(new(ReplaceSite: null, AppIdentity: KnownAppsConstants.PresetIdentity.PureIdentity(), DataSpecs: null));
        AttachApp(app);
    }
}
