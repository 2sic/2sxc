using System;
using System.Web;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.ContentBlocks
{
    public static class Render
    {
        public static IHtmlString One(DynamicEntity context,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            IDynamicEntity item = null,
            string field = null,
            Guid? newGuid = null)
            => Sxc.Blocks.Render.One(context, dontRelyOnParameterOrder, item, field, newGuid);

        public static IHtmlString All(DynamicEntity context,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string field = null,
            string merge = null)
            => Sxc.Blocks.Render.All(context, dontRelyOnParameterOrder, field, merge);
    }
}
