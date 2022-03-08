using System;
using System.Collections.Specialized;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Parameters;

namespace ToSic.Sxc.LookUp
{
    public class QueryStringLookUp : LookUpBase
    {
        /// <summary>
        /// Constructor for DI
        /// </summary>

        public QueryStringLookUp(Lazy<IHttp> httpLazy)
        {
            Name = "QueryString";
            _httpLazy = httpLazy;
        }
        private readonly Lazy<IHttp> _httpLazy;
        private NameValueCollection _source;
        private NameValueCollection _originalParams;


        public override string Get(string key, string format)
        {
            if (_source == null)
                _source = _httpLazy.Value?.QueryStringParams ?? new NameValueCollection();

            // Special handling when having original parameters in query string.
            var originalParametersQueryStringValue = _source[OriginalParameters.NameInUrlForOriginalParameters];
            var overrideParam = GetOverrideParam(key, originalParametersQueryStringValue);

            return !string.IsNullOrEmpty(overrideParam) ? overrideParam : _source[key];
        }

        private string GetOverrideParam(string key, string originalParametersQueryStringValue)
        {
            if (string.IsNullOrEmpty(originalParametersQueryStringValue)) return string.Empty;

            if (_originalParams == null)
            {
                var originalParams = new NameValueCollection { { OriginalParameters.NameInUrlForOriginalParameters, originalParametersQueryStringValue } };
                _originalParams = OriginalParameters.GetOverrideParams(originalParams);
            }

            return _originalParams[key];
        }
    }
}