using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ToSic.Eav.Configuration;

namespace ToSic.SexyContent.WebApi.Cors
{
    class CorsLocalDevelopmentHandler : DelegatingHandler
    {
        private readonly Regex considerLocal = new Regex("^https?://(local(host)?|.*.dnndev.me)(:[0-9]+)?$", RegexOptions.IgnoreCase);
        private readonly string allowedMethods = "*";
        private readonly string allowedHeaders = "*";

        //private static bool webApiAllowLocalEnabled;

        public CorsLocalDevelopmentHandler()
        {
            // Features seem not to be initialized at the time this is called
            // value is always false
            // webApiAllowLocalEnabled = Features.Enabled(FeatureIds.WebApiOptionsAllowLocal);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var webApiAllowLocalEnabled = Features.Enabled(FeatureIds.WebApiOptionsAllowLocal);
                var origin = request.Headers.Origin();

                // Respond to preflight requests
                if (webApiAllowLocalEnabled
                    && request.Method == HttpMethod.Options
                    && origin != null
                    && request.Headers.AccessControlRequestMethod() != null
                    && considerLocal.Match(origin).Success)
                {
                    var preflightResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    AppendCorsHeaders(preflightResponse.Headers, origin);
                    return preflightResponse;
                }
            }
            catch(Exception) { } // fail silently on errors
            
            // Let other handlers process the request
            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {

                    try
                    {
                        var webApiAllowLocalEnabled = Features.Enabled(FeatureIds.WebApiOptionsAllowLocal);

                        // Modify cors headers if needed
                        var origin = request.Headers.Origin();
                        if (webApiAllowLocalEnabled
                            && origin != null
                            && considerLocal.Match(origin).Success)
                        {
                            AppendCorsHeaders(task.Result.Headers, origin);
                        }
                    }
                    catch (Exception) { } // fail silently on errors

                    return task.Result;
                }, cancellationToken);
        }

        private void AppendCorsHeaders(HttpResponseHeaders headerCollection, string origin)
        {
            headerCollection.Set("Access-Control-Allow-Origin", origin);
            headerCollection.Set("Access-Control-Allow-Methods", allowedMethods);
            headerCollection.Set("Access-Control-Allow-Headers", allowedHeaders);
            headerCollection.Set("Access-Control-Allow-Credentials", "true");
        }
    }
}
