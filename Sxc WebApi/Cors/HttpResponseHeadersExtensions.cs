using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ToSic.SexyContent.WebApi.Cors
{
    internal static class HttpResponseHeadersExtensions
    {
        public static void Set(this HttpResponseHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
                headers.Remove(name);
            headers.Add(name, value);
        }
    }
}
