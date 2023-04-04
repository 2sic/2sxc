using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToSic.Sxc.Razor
{
    public class RazorCompiler : IRazorCompiler // TODO: stv add logging
    {
        #region Constructor and DI

        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;

        public RazorCompiler(
            ApplicationPartManager applicationPartManager,
            IRazorViewEngine viewEngine,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _applicationPartManager = applicationPartManager;
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

        private static bool _executedAlready = false;
        private IView FindView(ActionContext actionContext, string partialName)
        {
            var searchedLocations = new List<string>();
            try
            {
                List<ApplicationPart> removeThis = null;
                if (!_executedAlready)
                {
                    // fix special case that happens with oqtane custom module server project assembly (eg. Name.Server.Oqtane.dll)
                    // that has empty reference paths (for unknown reason) and IRazorViewEngine.GetView breaks when there are empty
                    // reference paths in the ApplicationParts/AssemblyPart
                    removeThis = _applicationPartManager.ApplicationParts.Where(part =>
                        part is not ICompilationReferencesProvider
                        && part is AssemblyPart assemblyPart
                        && assemblyPart.GetReferencePaths().Any(string.IsNullOrEmpty)).ToList();
                    foreach (var part in removeThis)
                        _applicationPartManager.ApplicationParts.Remove(part);
                    _executedAlready = true;
                }

                var firstAttempt = _viewEngine.GetView(null, partialName, false);

                if (removeThis != null)
                    foreach (var part in removeThis)
                        _applicationPartManager.ApplicationParts.Add(part);

                if (firstAttempt.Success)
                    return firstAttempt.View;

                searchedLocations.AddRange(firstAttempt.SearchedLocations);
            }
            catch (Exception e)
            {
                //add logging
            }

            try
            {
                var secondAttempt = _viewEngine.FindView(actionContext, partialName, false);
                if (secondAttempt.Success)
                    return secondAttempt.View;

                searchedLocations.AddRange(secondAttempt.SearchedLocations);
            }
            catch (Exception e)
            {
                // add logging
            }

            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext NewActionContext()
        {
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

    }
}