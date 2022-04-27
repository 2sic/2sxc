using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Services;

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
        public List<HttpHeader> HttpHeaders => _httpHeaders.Concat(CspHeaders).ToList();

        private readonly List<HttpHeader> _httpHeaders = new List<HttpHeader>();

        internal void AddCspService(CspService provider)
        {
            _cspServices = _cspServices ?? new List<CspService>();
            _cspServices.Add(provider);
        }
        private List<CspService> _cspServices;

        public List<HttpHeader> CspHeaders
        {
            get
            {
                var byType = _cspServices?
                    .Where(cs => cs != null)
                    .GroupBy(cs => cs.Name)
                    .ToList();

                if (byType == null || !byType.Any()) return new List<HttpHeader>();

                return byType.Select(list => CspHttpHeader(list.ToList())).ToList();
            }
        }

        private static HttpHeader CspHttpHeader(IReadOnlyCollection<CspService> servicesOfThisTypeOrNull)
        {
            if (servicesOfThisTypeOrNull == null) return null;

            var relevant = servicesOfThisTypeOrNull.Where(cs => cs != null).ToList();
            if (relevant?.Any() != true) return null;
            var first = relevant.First();
            var policy = first.Policy;

            if (relevant.Count == 1)
                return new HttpHeader(first.Name, policy.ToString());

            // If many, merge the settings
            foreach (var cspS in relevant.Skip(1)) policy.Add(cspS.Policy);

            return new HttpHeader(first.Name, policy.ToString());
        }

        #endregion

    }
}
