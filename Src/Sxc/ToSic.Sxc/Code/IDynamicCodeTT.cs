using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi("WIP v14.02")]
    public interface IDynamicCode<out TModel, out TServiceKit>: IDynamicCode, IDynamicCodeKit<TServiceKit>
        where TModel : class
        where TServiceKit: ServiceKit
    {
        [PrivateApi]
        TModel Model { get; }

        // 2022-09-12 2dm disabled this, already in the base interface
        //TServiceKit Kit { get; }
    }
}
