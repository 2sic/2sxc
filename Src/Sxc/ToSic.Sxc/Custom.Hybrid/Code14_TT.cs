using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// New base class for v12 Dynamic Code
    /// Adds new properties and methods, and doesn't keep old / legacy APIs
    /// </summary>
    [PrivateApi("WIP v14.02")]
    public class Code14<TModel, TServiceKit>: DynamicCode<TModel, TServiceKit>, IDynamicCode14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot?.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot?.Settings;

        public IDevTools DevTools => _DynCodeRoot.DevTools;
    }
}
