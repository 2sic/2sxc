using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    /// <summary>
    /// **_BETA_**
    /// 
    /// Base class for v14 Dynamic Code files.
    /// 
    /// Will provide the ServiceKit specified by the type `TServiceKit` on property `Kit`.
    /// </summary>
    /// <remarks>
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    /// <typeparam name="TModel">_not yet used_ - pls always use `dynamic`</typeparam>
    /// <typeparam name="TServiceKit">The ServiceKit provided on `Kit` - for now, use <see cref="ServiceKit14"/></typeparam>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP v14.07")]
    public abstract class Code14<TModel, TServiceKit>: DynamicCode, IDynamicCode14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        [PrivateApi("Not yet ready")]
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TServiceKit> root) ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new GetOnce<TServiceKit>();

        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot?.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot?.Settings;

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;
    }
}
