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
        private readonly string allowedMethods = "GET, POST, PUT, PATCH, DELETE";

        public CorsLocalDevelopmentHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try // Wrap everything in a try/catch block to prevent errors
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
                    AppendCorsHeaders(preflightResponse.Headers, origin, request.Headers.AccessControlRequestHeaders());
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
                            AppendCorsHeaders(task.Result.Headers, origin, request.Headers.AccessControlRequestHeaders());
                        }
                    }
                    catch (Exception) { } // fail silently on errors

                    return task.Result;
                }, cancellationToken);
        }

        private void AppendCorsHeaders(HttpResponseHeaders headerCollection, string origin, string allowedHeaders)
        {
            headerCollection.Set("Access-Control-Allow-Origin", origin);
            headerCollection.Set("Access-Control-Allow-Methods", allowedMethods);
            headerCollection.Set("Access-Control-Allow-Headers", allowedHeaders);
            headerCollection.Set("Access-Control-Allow-Credentials", "true");
        }
    }
}
