using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// **_BETA_**
    /// 
    /// Base class for v14 Dynamic WebAPI files.
    /// Will provide the ServiceKit specified by the type `TServiceKit` on property `Kit`.
    /// </summary>
    /// <typeparam name="TModel">_not yet used_ - pls always use `dynamic`</typeparam>
    /// <typeparam name="TServiceKit">The ServiceKit provided on `Kit` - for now, use <see cref="ServiceKit14"/></typeparam>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v14.05")]
    public abstract class Api14<TModel, TServiceKit>: Api12, IDynamicCode<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        [PrivateApi("Not yet ready")]
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TServiceKit> root) ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly ValueGetOnce<TServiceKit> _kit = new ValueGetOnce<TServiceKit>();

    }
}
