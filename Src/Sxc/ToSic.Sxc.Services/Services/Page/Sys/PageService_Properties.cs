using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Services.Page.Sys;

partial class PageService
{
    /// <inheritdoc />
    public string SetTitle(string value, string? placeholder = null)
        => AddToQueue(PageProperties.Title, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetDescription(string value, string? placeholder = null)
        => AddToQueue(PageProperties.Description, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetKeywords(string value, string? placeholder = null)
        => AddToQueue(PageProperties.Keywords, value, PageChangeModes.Prepend, placeholder);

    /// <inheritdoc />
    public string SetHttpStatus(int statusCode, string? message = null)
    {
        PageServiceShared.HttpStatusCode = statusCode;
        PageServiceShared.HttpStatusMessage = message;
        return "";
    }

    /// <inheritdoc />
    public string SetBase(string? url)
        => AddToQueue(PageProperties.Base, url, PageChangeModes.Replace, null);

    private string AddToQueue(PageProperties property, string? value, PageChangeModes change, string? token)
    {
        var result = PageServiceShared.Queue(property, value, change, token);
        Listeners.AddToPageChangeQueue(result);
        return ""; // empty so it can be used directly in razor.
    }
}