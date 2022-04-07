using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public int? HttpStatusCode { get; set; } = null;
        public string HttpStatusMessage { get; set; } = null;


        #region Headers WIP / BETA

        [PrivateApi]
        internal void AddToHttp(string header) => HttpHeaders.Add(header);

        public List<string> HttpHeaders = new List<string>();

        #endregion

    }
}
