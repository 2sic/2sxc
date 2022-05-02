using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public int? HttpStatusCode { get; set; } = null;

        public string HttpStatusMessage { get; set; } = null;


        #region Headers WIP / BETA

        [PrivateApi]
        internal void AddToHttp(string name, string value) =>
            _httpHeaders.Add(new HttpHeader(name, value));

        [PrivateApi]
        public List<HttpHeader> HttpHeaders => _httpHeaders.Concat(Csp.CspHeaders()).ToList();

        private readonly List<HttpHeader> _httpHeaders = new List<HttpHeader>();

        #endregion

    }
}
