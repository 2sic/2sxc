using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit.Toolbar;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public HtmlString Toolbar
        {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (Block == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = Block?.Context.UserMayEdit ?? false;

                if (!userMayEdit)
                    return new HtmlString("");

                if (CompatibilityLevel == 10)
                    throw new Exception("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://r.2sxc.org/EditToolbar");

                var toolbar = new ItemToolbar(Entity).Toolbar;
                return new HtmlString(toolbar);
            }
        }

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IHtmlString Render()
        {
            if (CompatibilityLevel == 10)
                throw new Exception("content.Render() is deprecated in the new RazorComponent. Use ToSic.Sxc.Blocks.Render.One(content) instead. See https://r.2sxc.org/EditToolbar");

            return Blocks.Render.One(this);
        }

        [PrivateApi("shouldn't be used, but it may be published by accident, so shouldn't be removed. ")]
        [Obsolete("please use Get instead")]
        public object GetEntityValue(string field) => _getValue(field);


    }
}
