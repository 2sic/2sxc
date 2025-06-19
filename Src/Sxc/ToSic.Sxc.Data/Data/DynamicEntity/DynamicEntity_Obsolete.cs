#if NETFRAMEWORK

using ToSic.Razor.Markup;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public System.Web.IHtmlString Toolbar => Cdf.GetService<IOldDynamicEntityFeatures>().GenerateOldToolbar(Cdf, Entity);

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IRawHtmlString Render() => Cdf.GetService<IOldDynamicEntityFeatures>().Render(Cdf, this);
    }
}

#endif
