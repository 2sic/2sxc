using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    abstract partial class Razor14
    {
        /// <inheritdoc cref="IDynamicCode.App" />
        public new IApp App => _DynCodeRoot.App;

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc cref="IDynamicCode12.Resources" />
        public dynamic Settings => _DynCodeRoot.Settings;

    }


}
