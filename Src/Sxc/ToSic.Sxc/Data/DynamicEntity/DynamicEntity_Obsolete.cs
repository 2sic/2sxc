using System;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
#if NETFRAMEWORK
        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public System.Web.IHtmlString Toolbar
        {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (_Dependencies.BlockOrNull == null)
                    return new System.Web.HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = _Dependencies.BlockOrNull?.Context.UserMayEdit ?? false;

                if (!userMayEdit)
                    return new System.Web.HtmlString("");

                if (_Dependencies.CompatibilityLevel > Constants.MaxLevelForEntityDotToolbar)
                    throw new Exception("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://r.2sxc.org/EditToolbar");

                var toolbar = new Edit.Toolbar.ItemToolbar(Entity).Toolbar;
                return new System.Web.HtmlString(toolbar);
            }
        }

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public System.Web.IHtmlString Render()
        {
            if (_Dependencies.CompatibilityLevel > Constants.MaxLevelForEntityDotRender)
                throw new Exception("content.Render() is deprecated in the new RazorComponent. Use ToSic.Sxc.Blocks.Render.One(content) instead. See https://r.2sxc.org/EditToolbar");

            return Factory.StaticBuild<Services.IRenderService>().One(this);
        }

        [PrivateApi("shouldn't be used, but it may be published by accident, so shouldn't be removed. ")]
        [Obsolete("please use Get instead")]
        public object GetEntityValue(string field) => GetInternal(field);

#endif

    }
}
