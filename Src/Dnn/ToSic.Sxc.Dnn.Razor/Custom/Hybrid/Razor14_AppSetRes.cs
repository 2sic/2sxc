using ToSic.Sxc.Apps;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14
    {
        /// <inheritdoc />
        public new IApp App => _DynCodeRoot.App;

        /// <inheritdoc />
        public dynamic Resources => _DynCodeRoot.Resources;

        /// <inheritdoc />
        public dynamic Settings => _DynCodeRoot.Settings;

    }


}
