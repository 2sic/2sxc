using ToSic.Razor.Html5;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public void AddJsonLd(string jsonString)
            => AddToHead(new ScriptJsonLd(_jsonService.ToJson(jsonString)));

        /// <inheritdoc />
        public void AddJsonLd(object jsonObject)
            => AddToHead(new ScriptJsonLd(_jsonService.ToJson(jsonObject)));

    }
}
