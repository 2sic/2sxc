namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public void SetTitle(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Title, value, PageChangeModes.Prepend, placeholder);

        /// <inheritdoc />
        public void SetDescription(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Description, value, PageChangeModes.Prepend, placeholder);

        /// <inheritdoc />
        public void SetKeywords(string value, string placeholder = null) => PageServiceShared.Queue(PageProperties.Keywords, value, PageChangeModes.Append, placeholder);

        /// <inheritdoc />
        public void SetHttpStatus(int statusCode, string message = null)
        {
            PageServiceShared.HttpStatusCode = statusCode;
            PageServiceShared.HttpStatusMessage = message;
        }

        /// <inheritdoc />
        public void SetBase(string url) => PageServiceShared.Queue(PageProperties.Base, url, PageChangeModes.Replace, null);

    }
}
