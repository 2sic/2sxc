using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web.ContentSecurityPolicy;
using ToSic.Sxc.Web.Url;

//using BuiltInFeatures = ToSic.Sxc.Configuration.Features.BuiltInFeatures;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtPageChangesSupportService : IOqtPageChangesSupportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LazySvc<IFeaturesService> _featuresService;

        public OqtPageChangesSupportService(IHttpContextAccessor httpContextAccessor, LazySvc<IFeaturesService> featuresService)
        {
            _httpContextAccessor = httpContextAccessor;
            _featuresService = featuresService;
        }


        public int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page)
        {
            var logPrefix = $"{nameof(ApplyHttpHeaders)}(...) - ";

            page?.Log($"{logPrefix}stv#1 poc");

            if (_httpContextAccessor?.HttpContext?.Request?.Path.HasValue != true)
                return -1; // no path, no headers

            if (_httpContextAccessor.HttpContext.Request.Path.Value!.Contains("/_blazor"))
                return -1; // no headers for blazor

            if (!(result?.CspParameters?.Any() ?? false))
                return -1; // no headers if there are no CSP parameters

            // Register CSP changes for applying once all modules have been prepared
            // Note that in cached scenarios, CspEnabled is true, but it may have been turned off since
            if (result!.CspEnabled && _featuresService.Value.IsEnabled("ContentSecurityPolicy" /*BuiltInFeatures.ContentSecurityPolicy.NameId*/))
            {
                page?.Log($"{logPrefix}Register CSP changes");
                PageCsp(result.CspEnforced, page).Add(result.CspParameters.Select(p => new CspParameters(UrlHelpers.ParseQueryString(p))).ToList());
            }

            var httpHeaders = result.HttpHeaders;

            if (_httpContextAccessor.HttpContext?.Response == null)
            {
                page?.Log($"{logPrefix}error, HttpResponse is null");
                return 0;
            }

            if (_httpContextAccessor.HttpContext.Response.HasStarted)
            {
                page?.Log($"{logPrefix}error, to late for adding http headers");
                return 0;
            }

            if (httpHeaders?.Any() == true) // ????
            {
                page?.Log($"{logPrefix}ok, no headers to add");
                return 0;
            }

            // Register event to attach headers
            _httpContextAccessor.HttpContext.Response.OnStarting(() =>
            {
                try
                {
                    foreach (var httpHeader in httpHeaders!)
                    {
                        if (string.IsNullOrWhiteSpace(httpHeader.Name)) continue;

                        // TODO: The CSP header can only exist once
                        // So to do this well, we'll need to merge them in future, 
                        // Ideally combining the existing one with any additional ones added here
                        _httpContextAccessor.HttpContext.Response.Headers[httpHeader.Name] = httpHeader.Value;
                        page?.Log($"{logPrefix}{httpHeader.Name}={httpHeader.Value}");
                    }
                }
                catch (Exception e)
                {
                    page?.Log($"{logPrefix}Exception: {e.Message}");
                }

                return Task.CompletedTask;
            });

            page?.Log($"{logPrefix}httpHeaders.Count: {httpHeaders!.Count}");
            return httpHeaders!.Count;
        }

        public CspOfPage PageCsp(bool enforced, ModuleProBase page)
        {
            var logPrefix = $"{nameof(PageCsp)}(enforced:{enforced}) - ";

            var key = "2sxcPageLevelCsp";

            // If it's already registered, then the add-on-sending has already been added too
            // So we shouldn't repeat it, just return the cache which will be used later
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(key))
            {
                var result = (CspOfPage)_httpContextAccessor.HttpContext.Items[key];
                page?.Log($"already registered {logPrefix}{key}={result}");
                return result;
            }

            // Not yet registered. Create, and register for on-end of request
            var pageLevelCsp = new CspOfPage();
            _httpContextAccessor.HttpContext.Items[key] = pageLevelCsp;

            // Register event to attach headers once the request is done and all Apps have registered their Csp
            if (!_httpContextAccessor.HttpContext.Response.HasStarted)
                _httpContextAccessor.HttpContext.Response.OnStarting(() =>
                {
                    try
                    {
                        var headers = pageLevelCsp.CspHttpHeader();
                        if (headers != null)
                        {
                            var key = pageLevelCsp.HeaderName(enforced);
                            _httpContextAccessor.HttpContext.Response.Headers[key] = headers;
                            page?.Log($"{logPrefix}have headers {key}={headers}");
                        }

                    }
                    catch (Exception e)
                    {
                        page?.Log($"{logPrefix}Exception: {e.Message}");
                    }

                    return Task.CompletedTask;
                });
            page?.Log($"not yet registered {logPrefix}{key}={pageLevelCsp}");
            return pageLevelCsp;
        }
    }
}
