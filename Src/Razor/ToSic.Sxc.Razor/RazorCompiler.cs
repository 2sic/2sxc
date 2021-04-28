using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;

namespace ToSic.Sxc.Razor
{
    public class RazorCompiler : IRazorCompiler
    {
        #region Constructor and DI
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public RazorCompiler(
            IRazorViewEngine viewEngine,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _viewEngine = viewEngine;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }
        #endregion

        public (IView view, ActionContext context) CompileView(string partialName, Action<RazorView> configure = null)
        {
            var actionContext = _actionContextAccessor.ActionContext ?? NewActionContext();
            var partial = FindView(actionContext, partialName);
            // do callback to configure the object we received
            if (partial is RazorView rzv) configure?.Invoke(rzv);
            return (partial, actionContext);
        }

        private IView FindView(ActionContext actionContext, string partialName)
        {
            var firstAttempt = _viewEngine.GetView(null, partialName, false);
            if (firstAttempt.Success)
                return firstAttempt.View;

            var secondAttempt = _viewEngine.FindView(actionContext, partialName, false);
            if (secondAttempt.Success)
                return secondAttempt.View;
            var searchedLocations = firstAttempt.SearchedLocations.Concat(secondAttempt.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext NewActionContext()
        {
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

    }
}