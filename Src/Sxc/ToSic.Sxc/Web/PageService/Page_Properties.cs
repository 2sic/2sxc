using System.Collections.Generic;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page
    {
        public IList<PagePropertyChange> PropertyChanges { get; } = new List<PagePropertyChange>();

        /// <inheritdoc />
        public void SetTitle(string value, string token = null) => Queue(PageProperties.Title, value, PageChangeModes.Prepend, token);

        /// <inheritdoc />
        public void SetDescription(string value, string token = null) => Queue(PageProperties.Description, value, PageChangeModes.Prepend, token);

        /// <inheritdoc />
        public void SetKeywords(string value, string token = null) => Queue(PageProperties.Keywords, value, PageChangeModes.Append, token);

        /// <inheritdoc />
        public void SetBase(string url) => Queue(PageProperties.Base, url, PageChangeModes.Replace, null);

        /// <summary>
        /// Add something to the queue for setting a page property
        /// </summary>
        private void Queue(PageProperties property, string value, PageChangeModes change, string token)
        {
            PropertyChanges.Add(new PagePropertyChange
            {
                ChangeMode = GetMode(change),
                Property = property, 
                Value = value,
                ReplacementIdentifier = token,
            });

        }

    }
}
