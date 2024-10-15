using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using System;
using ToSic.Sxc.Oqt.Shared.Interfaces;

namespace ToSic.Sxc.Oqt.Server.Services
{
    /// <inheritdoc />
    public class RenderInfoService(IHttpContextAccessor httpContextAccessor) : IRenderInfoService
    {
        private readonly List<string> _enhancedNavValues = ["allow", "on"];
        private const string BlazorEnhancedNav = "blazor-enhanced-nav";
        private const string SsrFraming = "ssr-framing";

        /// <inheritdoc />
        public bool IsStaticSsr(string renderMode = RenderModes.Interactive)
          => string.Equals($"{renderMode}", RenderModes.Static, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc />
        public bool IsBlazorEnhancedNav(string renderMode)
        {
            if (!IsStaticSsr(renderMode))
                return false;

            var context = httpContextAccessor.HttpContext;
            if (context != null && context.Response.Headers.TryGetValue(BlazorEnhancedNav, out var enhancedNav))
                return _enhancedNavValues.Contains($"{enhancedNav}", StringComparer.OrdinalIgnoreCase);

            return false;
        }

        /// <inheritdoc />
        public bool IsSsrFraming(string renderMode)
        {
            if (!IsBlazorEnhancedNav(renderMode))
                return false;

            var context = httpContextAccessor.HttpContext;
            return context != null && context.Response.Headers.TryGetValue(SsrFraming, out _);
        }
    }
}
