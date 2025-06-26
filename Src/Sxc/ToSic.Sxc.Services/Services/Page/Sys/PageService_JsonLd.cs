namespace ToSic.Sxc.Services.Page.Sys;

partial class PageService
{
    /// <inheritdoc />
    public string AddJsonLd(string jsonString)
        => AddToHead(htmlTagsLazy.Value.ScriptJsonLd(jsonString));

    /// <inheritdoc />
    public string AddJsonLd(object jsonObject)
        => AddToHead(htmlTagsLazy.Value.ScriptJsonLd(jsonObject));

}