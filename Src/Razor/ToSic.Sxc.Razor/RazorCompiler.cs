using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Internal;
using IView = Microsoft.AspNetCore.Mvc.ViewEngines.IView;

namespace ToSic.Sxc.Razor;

internal class RazorCompiler(
    ApplicationPartManager applicationPartManager,
    IRazorViewEngine viewEngine,
    IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor,
    IActionContextAccessor actionContextAccessor,
    LazySvc<AppCodeLoader> appCodeLoader,
    AssemblyResolver assemblyResolver,
    SourceAnalyzer sourceAnalyzer)
    : ServiceBase($"{SxcLogging.SxcLogName}.RzrCmp",
        connect:
        [
            applicationPartManager, viewEngine, /* never! serviceProvider,*/ httpContextAccessor, actionContextAccessor, appCodeLoader, assemblyResolver, sourceAnalyzer
        ]), IRazorCompiler
{
    public async Task<(IView view, ActionContext context)> CompileView(string partialName, Action<RazorView> configure = null, IApp app = null, HotBuildSpec spec = default)
    {
        var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{partialName},appCodePath:{app}");
        var actionContext = actionContextAccessor.ActionContext ?? NewActionContext();
        var partial = await FindViewAsync(actionContext, partialName, app, spec);
        // do callback to configure the object we received
        if (partial is RazorView rzv) configure?.Invoke(rzv);
        return l.ReturnAsOk((partial, actionContext));
    }

    private static bool _executedAlready = false;
    private async Task<IView> FindViewAsync(ActionContext actionContext, string partialName, IApp app, HotBuildSpec spec)
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
                // fix special case that happens with oqtane custom module server project assembly (like Name.Server.Oqtane.dll)
                // that has empty reference paths (for unknown reason) and IRazorViewEngine.GetView breaks when there are empty
                // reference paths in the ApplicationParts/AssemblyPart
                removeThis = applicationPartManager.ApplicationParts.Where(part =>
                    part is not ICompilationReferencesProvider
                    && part is AssemblyPart assemblyPart
                    && assemblyPart.GetReferencePaths().Any(string.IsNullOrEmpty)).ToList();
                foreach (var part in removeThis)
                    applicationPartManager.ApplicationParts.Remove(part);
                l.A($"removed:{removeThis.Count}");
                _executedAlready = true;
            }

            // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
            // 1. probably change so the CodeFileInfo contains the source code
            var razorType = sourceAnalyzer.TypeOfVirtualPath(partialName);
            if (razorType.IsHotBuildSupported())
                AddAppCodeAssembly(partialName, app, spec);

            var firstAttempt = viewEngine.GetView(null, partialName, false);
            l.A($"firstAttempt: {firstAttempt}");

            if (removeThis != null)
            {
                foreach (var part in removeThis)
                    applicationPartManager.ApplicationParts.Add(part);
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
            var secondAttempt = viewEngine.FindView(actionContext, partialName, false);
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
        var httpContext = httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = serviceProvider };
        return l.ReturnAsOk(new(httpContext, new(), new()));
    }

    private void AddAppCodeAssembly(string partialName, IApp app, HotBuildSpec spec)
    {
        var log = Log.Fn($"{nameof(partialName)}:{partialName}; {nameof(app.RelativePath)}:{app.RelativePath}; {spec}", timer: true);

        // Get assembly - try to get from cache, otherwise compile
        var (assemblyResult, _) = appCodeLoader.Value.GetAppCode(spec);
        log.A($"has AppCode assembly: {assemblyResult?.HasAssembly}");

        if (assemblyResult?.Assembly != null)
        {
            var appRelativePathWithEdition = spec.Edition.HasValue() ? Path.Combine(app.RelativePath, spec.Edition) : app.RelativePath;
            log.A($"{nameof(appRelativePathWithEdition)}: '{appRelativePathWithEdition}'");

            // Add assembly to resolver, so it will be provided to the compiler when used in cshtml
            assemblyResolver.AddAssembly(assemblyResult.Assembly, appRelativePathWithEdition);
        };

        log.Done();
    }
}