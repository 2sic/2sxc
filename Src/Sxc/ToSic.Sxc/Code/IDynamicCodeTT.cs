using ToSic.Eav.Documentation;
using ToSic.Sxc.Services.Kits;

namespace ToSic.Sxc.Code
{
    [PrivateApi("WIP v14.02")]
    public interface IDynamicCode<out TModel, out TKit>: IDynamicCode, IDynamicCodeKit<TKit>
        where TKit: KitBase
    {
        [PrivateApi]
        TModel Model { get; }

        TKit Kit { get; }
    }
}
