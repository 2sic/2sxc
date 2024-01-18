namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string AddJsonLd(string jsonString)
        => AddToHead(_htmlTagsLazy.Value.ScriptJsonLd(jsonString));

    /// <inheritdoc />
    public string AddJsonLd(object jsonObject)
        => AddToHead(_htmlTagsLazy.Value.ScriptJsonLd(jsonObject));

}