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
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Razor
{
    public class RazorCompiler : ServiceBase, IRazorCompiler
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
            IActionContextAccessor actionContextAccessor) : base($"{Constants.SxcLogName}.RzrCmp")
        {
            ConnectServices(
                _applicationPartManager = applicationPartManager,
                _viewEngine = viewEngine,
                _serviceProvider = serviceProvider,
                _httpContextAccessor = httpContextAccessor,
                _actionContextAccessor = actionContextAccessor
            );
        }
        #endregion

        public (IView view, ActionContext context) CompileView(string partialName, Action<RazorView> configure = null)
        {
            var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{partialName}");
            var actionContext = _actionContextAccessor.ActionContext ?? NewActionContext();
            var partial = FindView(actionContext, partialName);
            // do callback to configure the object we received
            if (partial is RazorView rzv) configure?.Invoke(rzv);
            return l.ReturnAsOk((partial, actionContext));
        }

        private static bool _executedAlready = false;
        private IView FindView(ActionContext actionContext, string partialName)
        {
            var l = Log.Fn<IView>($"partialName:{partialName}");
            var searchedLocations = new List<string>();
            var exceptions = new List<Exception>();
            try
            {
                List<ApplicationPart> removeThis = null;
                if (!_executedAlready)
                {
                    l.A($"one time execute, remove problematic ApplicationPart assemblies");
                    // fix special case that happens with oqtane custom module server project assembly (eg. Name.Server.Oqtane.dll)
                    // that has empty reference paths (for unknown reason) and IRazorViewEngine.GetView breaks when there are empty
                    // reference paths in the ApplicationParts/AssemblyPart
                    removeThis = _applicationPartManager.ApplicationParts.Where(part =>
                        part is not ICompilationReferencesProvider
                        && part is AssemblyPart assemblyPart
                        && assemblyPart.GetReferencePaths().Any(string.IsNullOrEmpty)).ToList();
                    foreach (var part in removeThis)
                        _applicationPartManager.ApplicationParts.Remove(part);
                    l.A($"removed:{removeThis.Count}");
                    _executedAlready = true;
                }

                var firstAttempt = _viewEngine.GetView(null, partialName, false);
                l.A($"firstAttempt: {firstAttempt}");

                if (removeThis != null)
                {
                    foreach (var part in removeThis)
                        _applicationPartManager.ApplicationParts.Add(part);
                    l.A($"restore removed ApplicationParts:{removeThis.Count}");
                }

                if (firstAttempt.Success)
                    return l.ReturnAsOk(firstAttempt.View);

                searchedLocations.AddRange(firstAttempt.SearchedLocations);
                l.A($"searchedLocations({searchedLocations.Count}): {string.Join(";", searchedLocations)}");
            }
            catch (Exception e)
            {
                l.Ex(e);
                exceptions.Add(e);
            }

            try
            {
                var secondAttempt = _viewEngine.FindView(actionContext, partialName, false);
                l.A($"secondAttempt: {secondAttempt}");

                if (secondAttempt.Success)
                    return l.ReturnAsOk(secondAttempt.View);

                searchedLocations.AddRange(secondAttempt.SearchedLocations);
                l.A($"searchedLocations({searchedLocations.Count}): {string.Join(";", searchedLocations)}");
            }
            catch (Exception e)
            {
                l.Ex(e);
                exceptions.Add(e);
            }

            foreach (var exception in exceptions)
                throw exception;

            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations));
            l.A($"error:{errorMessage}");
            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext NewActionContext()
        {
            var l = Log.Fn<ActionContext>();
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
            return l.ReturnAsOk(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));
        }
    }
}