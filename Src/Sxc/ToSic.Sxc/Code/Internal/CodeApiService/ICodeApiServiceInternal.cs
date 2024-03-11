using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

internal interface ICodeApiServiceInternal
{
    [PrivateApi]
    void AttachApp(IApp app);

    [PrivateApi("WIP")]
    IBlock _Block { get; }

    TKit GetKit<TKit>() where TKit : ServiceKit;
}