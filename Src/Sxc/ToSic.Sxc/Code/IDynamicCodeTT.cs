using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    // TODO: @2dm - probably remove this interface as it's not useful any more
    // But it's still being used to attach the Kit...?
    [PrivateApi("WIP v14.02")]
    public interface IDynamicCode<out TModel, out TServiceKit>: IDynamicCode, IDynamicCodeKit<TServiceKit>
        where TModel : class
        where TServiceKit: ServiceKit
    {
        //[PrivateApi]
        //TModel Model { get; }

    }
}
