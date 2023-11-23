using System.Collections.Generic;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public int? HttpStatusCode { get; set; } = null;

        public string HttpStatusMessage { get; set; } = null;


        #region Headers WIP / BETA

        [PrivateApi]
        internal void AddToHttp(string name, string value) =>
            HttpHeaders.Add(new HttpHeader(name, value));

        [PrivateApi]
        public List<HttpHeader> HttpHeaders { get; } = new();

        #endregion

    }
}
