using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Web.PageService
{
    public partial class Page
    {
        public IList<PagePropertyChange> PropertyChanges { get; } = new List<PagePropertyChange>();
        
        public IList<PagePropertyChange> GetPropertyChangesAndFlush()
        {
            var changes = PropertyChanges.ToArray().ToList();
            PropertyChanges.Clear();
            return changes;
        }

        /// <inheritdoc />
        public void SetTitle(string value, string placeholder = null) => Queue(PageProperties.Title, value, PageChangeModes.Prepend, placeholder);

        /// <inheritdoc />
        public void SetDescription(string value, string placeholder = null) => Queue(PageProperties.Description, value, PageChangeModes.Prepend, placeholder);

        /// <inheritdoc />
        public void SetKeywords(string value, string placeholder = null) => Queue(PageProperties.Keywords, value, PageChangeModes.Append, placeholder);

        public int? HttpStatusCode = null;
        public string HttpStatusMessage = null;
        
        /// <inheritdoc />
        public void SetHttpStatus(int statusCode, string message = null)
        {
            HttpStatusCode = statusCode;
            HttpStatusMessage = message;
        }

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
