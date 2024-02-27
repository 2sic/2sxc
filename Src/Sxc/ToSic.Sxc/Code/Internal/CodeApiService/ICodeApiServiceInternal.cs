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

    ///// <summary>
    ///// Special GetService which will cache the found service so any other kit would use it as well.
    ///// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    ///// </summary>
    ///// <typeparam name="TService"></typeparam>
    ///// <returns></returns>
    //[PrivateApi("new v17.02")]
    //TService GetService<TService>(NoParamOrder protector = default, bool reuse = false) where TService : class;

    TKit GetKit<TKit>() where TKit : ServiceKit;
}