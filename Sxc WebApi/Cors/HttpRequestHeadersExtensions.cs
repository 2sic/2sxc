using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.SexyContent.WebApi.Cors
{
    internal static class HttpRequestHeadersExtensions
    {
        private const string originHeaderKey = "Origin";
        private const string accessControlRequestHeadersKey = "Access-Control-Request-Headers";
        private const string accessControlRequestMethodKey = "Access-Control-Request-Method";

        public static string Origin(this HttpRequestHeaders headers)
        {
            if (headers.Contains(originHeaderKey) &&
                headers.GetValues(originHeaderKey).Any())
                return headers.GetValues(originHeaderKey).First();

            return null;
        }

        public static string AccessControlRequestMethod(this HttpRequestHeaders headers)
        {
            if (headers.Contains(accessControlRequestMethodKey) &&
                headers.GetValues(accessControlRequestMethodKey).Any())
                return headers.GetValues(accessControlRequestMethodKey).First();

            return null;
        }

    }
}
