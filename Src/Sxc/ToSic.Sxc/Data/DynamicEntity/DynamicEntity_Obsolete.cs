#if NETFRAMEWORK

using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Markup;

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
                if (_Services.BlockOrNull == null)
                    return new System.Web.HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = _Services.BlockOrNull?.Context.UserMayEdit ?? false;

                if (!userMayEdit)
                    return new System.Web.HtmlString("");

                if (_Services.AsC.CompatibilityLevel > Constants.MaxLevelForEntityDotToolbar)
                    throw new Exception("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://go.2sxc.org/EditToolbar");

                var toolbar = new Edit.Toolbar.ItemToolbar(Entity).ToolbarAsTag;
                return new System.Web.HtmlString(toolbar);
            }
        }

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IRawHtmlString Render()
        {
            if (_Services.AsC.CompatibilityLevel > Constants.MaxLevelForEntityDotRender)
                throw new Exception("content.Render() is deprecated in the new RazorComponent. Use GetService&lt;ToSic.Sxc.Services.IRenderService&gt;().One(content) instead.");

            return _Services.RenderService.One(this);
        }

        [PrivateApi("shouldn't be used, but it may be published by accident, so shouldn't be removed. ")]
        [Obsolete("please use Get instead")]
        public object GetEntityValue(string field) => GetInternal(field).Result;


    }
}

#endif
