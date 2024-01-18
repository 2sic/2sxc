#if NETFRAMEWORK
using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Razor.Markup;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.ContentBlocks
{
    [Obsolete]
    public static class Render
    {
        [Obsolete]
        public static IRawHtmlString One(DynamicEntity context,
            NoParamOrder noParamOrder = default,
            ICanBeEntity item = null,
            string field = null,
            Guid? newGuid = null)
            => Sxc.Blocks.Render.One(context, noParamOrder, item: item, field: field, newGuid: newGuid);

        [Obsolete]
        public static IRawHtmlString All(DynamicEntity context,
            NoParamOrder noParamOrder = default,
            string field = null,
            string merge = null)
            => Sxc.Blocks.Render.All(context, noParamOrder, field: field, merge: merge);
    }
}
#endif