using System;
using System.Web;
using ToSic.Sxc.Data;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif


// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.ContentBlocks
{
    [Obsolete]
    public static class Render
    {
        [Obsolete]
        public static IHtmlString One(DynamicEntity context,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicEntity item = null,
            string field = null,
            Guid? newGuid = null)
            => Sxc.Blocks.Render.One(context, noParamOrder, item: item, field: field, newGuid: newGuid);

        [Obsolete]
        public static IHtmlString All(DynamicEntity context,
            string noParamOrder = Eav.Parameters.Protector,
            string field = null,
            string merge = null)
            => Sxc.Blocks.Render.All(context, noParamOrder, field: field, merge: merge);
    }
}
