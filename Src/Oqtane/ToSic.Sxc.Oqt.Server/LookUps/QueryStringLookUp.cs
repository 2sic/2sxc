using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class QueryStringLookUp : LookUpBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IQueryCollection _source;
        private NameValueCollection _originalParams;

        public QueryStringLookUp(IHttpContextAccessor httpContextAccessor)
        {
            Name = "QueryString";
            _httpContextAccessor = httpContextAccessor;
        }

        public override string Get(string key, string format)
        {
            _source ??= _httpContextAccessor?.HttpContext?.Request.Query;
            if (_source == null) return string.Empty;

            // Special handling when having original parameters in query string.
            var overrideParam = GetOverrideParam(key);
            if (!string.IsNullOrEmpty(overrideParam)) return overrideParam;

            return _source.TryGetValue(key, out var result) ? result.ToString() : string.Empty;
        }

        private string GetOverrideParam(string key)
        {
            if (!_source.TryGetValue(OriginalParameters.NameInUrlForOriginalParameters, out var queryStringValue))
                return string.Empty;

            if (_originalParams == null)
            {
                var originalParams = new NameValueCollection {{OriginalParameters.NameInUrlForOriginalParameters, queryStringValue.ToString()}};
                _originalParams = OriginalParameters.GetOverrideParams(originalParams);
            }

            return _originalParams[key];
        }
    }
}