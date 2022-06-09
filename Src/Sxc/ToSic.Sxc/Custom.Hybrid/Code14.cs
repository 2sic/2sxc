using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Services.Kits;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// New base class for v12 Dynamic Code
    /// Adds new properties and methods, and doesn't keep old / legacy APIs
    /// </summary>
    [PrivateApi("WIP v14.02")]
    public class Code14<TModel, TKit>: DynamicCode<TModel, TKit>, IDynamicCode14<TModel, TKit>
        where TKit : KitBase
    {
        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot?.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot?.Settings;

        public IDevTools DevTools => _DynCodeRoot.DevTools;
    }
}
