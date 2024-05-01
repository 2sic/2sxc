#if NETFRAMEWORK

using ToSic.Razor.Markup;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public System.Web.IHtmlString Toolbar
        {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (Cdf.BlockOrNull == null)
                    return new System.Web.HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = Cdf.BlockOrNull?.Context.Permissions.IsContentAdmin ?? false;

                if (!userMayEdit)
                    return new System.Web.HtmlString("");

                if (Cdf.CompatibilityLevel > CompatibilityLevels.MaxLevelForEntityDotToolbar)
                    throw new("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://go.2sxc.org/EditToolbar");

                var toolbar = new Edit.Toolbar.ItemToolbar(Entity).ToolbarAsTag;
                return new System.Web.HtmlString(toolbar);
            }
        }

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IRawHtmlString Render()
        {
            if (Cdf.CompatibilityLevel > CompatibilityLevels.MaxLevelForEntityDotRender)
                throw new("content.Render() is deprecated in the new RazorComponent. Use GetService&lt;ToSic.Sxc.Services.IRenderService&gt;().One(content) instead.");

            return Cdf.Services.RenderServiceGenerator.New().One(this);
        }

        [PrivateApi("shouldn't be used, but it may be published by accident, so shouldn't be removed. ")]
        [Obsolete("please use Get instead")]
        public object GetEntityValue(string name) => GetHelper.TryGet(name).Result;


    }
}

#endif
