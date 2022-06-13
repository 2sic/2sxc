using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public string AddJsonLd(string jsonString)
            => AddToHead(new ScriptJsonLd(jsonString));

        /// <inheritdoc />
        public string AddJsonLd(object jsonObject)
            => AddToHead(new ScriptJsonLd(jsonObject));

    }
}
