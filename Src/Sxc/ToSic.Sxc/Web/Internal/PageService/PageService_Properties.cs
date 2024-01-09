namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageService
{
    /// <inheritdoc />
    public string SetTitle(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Title, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetDescription(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Description, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetKeywords(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Keywords, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetHttpStatus(int statusCode, string message = null)
    {
        PageServiceShared.HttpStatusCode = statusCode;
        PageServiceShared.HttpStatusMessage = message;
        return "";
    }

    /// <inheritdoc />
    public string SetBase(string url) => PageServiceShared.Queue(PageProperties.Base, url, PageChangeModes.Replace, null);

}