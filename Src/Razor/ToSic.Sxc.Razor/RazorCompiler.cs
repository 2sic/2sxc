using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Sys;
using ToSic.Sys.Utils;
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
    public async Task<(IView view, ActionContext context)> CompileView(string partialName, Action<RazorView> configure, IApp app, HotBuildSpec spec)
    {
        var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{partialName},appCodePath:{app}");
        var actionContext = actionContextAccessor.ActionContext ?? NewActionContext();
        var partial = await FindViewAsync(actionContext, partialName, app, spec);
        // do callback to configure the object we received
        if (partial is RazorView rzv)
            configure.Invoke(rzv);
        return l.ReturnAsOk((partial, actionContext));
    }

    private static bool _executedAlready;
    private async Task<IView> FindViewAsync(ActionContext actionContext, string partialName, IApp app, HotBuildSpec spec)
    {
        var l = Log.Fn<IView>($"partialName:{partialName}");
        var searchedLocations = new List<string>();
        var exceptions = new List<Exception>();
        try
        {
            List<ApplicationPart>? removeThis = null;
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
            var shouldRegisterAppCode = razorType.IsHotBuildSupported();
            l.A($"Source analysis: {Describe(razorType)}; {nameof(partialName)}:'{partialName}'; {nameof(app.RelativePath)}:'{app.RelativePath}'; {nameof(spec.Edition)}:'{spec.Edition}'; {spec}; AppCode registration attempted:{shouldRegisterAppCode}");

            if (shouldRegisterAppCode)
            {
                var appCodeReferenceAvailable = AddAppCodeAssembly(partialName, app, spec);
                l.A($"AppCode registration result: attempted:true; reference available:{appCodeReferenceAvailable}");
                if (!appCodeReferenceAvailable)
                    l.W($"AppCode reference missing after registration attempt for '{partialName}'. Razor compilation will continue unchanged.");
            }

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

    private bool AddAppCodeAssembly(string partialName, IApp app, HotBuildSpec spec)
    {
        var log = Log.Fn<bool>($"{nameof(partialName)}:{partialName}; {nameof(app.RelativePath)}:{app.RelativePath}; {spec}", timer: true);

        // Get assembly - try to get from cache, otherwise compile
        var (assemblyResult, resultSpec) = appCodeLoader.Value.GetAppCode(spec);
        var assembly = assemblyResult?.Assembly;
        var resolverKey = AppRelativePathWithEdition(app, spec);
        var resultResolverKey = AppRelativePathWithEdition(app, resultSpec);
        var resolverKeys = AppCodeResolverKeys.Build(partialName,
        [
            resolverKey,
            resultResolverKey,
            app.RelativePath
        ]);
        log.A($"AppCode loader result: requestedSpec:'{spec}'; resultSpec:'{resultSpec}'; hasResult:{assemblyResult != null}; hasAssembly:{assemblyResult?.HasAssembly}; assembly:'{assembly?.FullName}'; assemblyLocation:'{assembly?.Location}'; assemblyLocations:'{string.Join(";", assemblyResult?.AssemblyLocations ?? [])}'; errorMessages:'{assemblyResult?.ErrorMessages}'; resolverKey:'{resolverKey}'; resultResolverKey:'{resultResolverKey}'");

        if (assembly != null)
        {
            // Add assembly to resolver, so it will be provided to the compiler when used in cshtml
            foreach (var key in resolverKeys)
                assemblyResolver.AddAssembly(assembly, key);

            var resolverLookups = AppCodeResolverKeys.Resolve(assemblyResolver, resolverKeys);
            var matchedResolver = AppCodeResolverKeys.PickBest(resolverLookups);
            var referenceAvailable = matchedResolver?.Exists == true;
            log.A($"Resolver lookup after registration: keys:'{AppCodeResolverKeys.Describe(resolverKeys)}'; results:'{AppCodeResolverKeys.DescribeResults(resolverLookups)}'");
            if (!referenceAvailable)
            {
                log.W($"AppCode assembly was loaded but no file reference is available for any resolver key. Requested:'{resolverKey}'; result:'{resultResolverKey}'.");
                return log.ReturnFalse("no file reference");
            }

            log.A($"Resolver lookup matched key:'{matchedResolver!.Key.Backslash()}'; location:'{matchedResolver.Location}'");
            return log.ReturnTrue("reference available");
        }

        log.W($"AppCode registration requested for '{partialName}' but AppCodeLoader returned no assembly.");

        return log.ReturnFalse("no assembly");
    }

    private static string AppRelativePathWithEdition(IApp app, HotBuildSpec spec)
        => spec.Edition.HasValue()
            ? Path.Combine(app.RelativePath, spec.Edition)
            : app.RelativePath;

    private static string Describe(CodeFileInfo codeFileInfo)
        => $"{nameof(codeFileInfo.Inherits)}:'{codeFileInfo.Inherits}'; {nameof(codeFileInfo.Type)}:'{codeFileInfo.Type}'; {nameof(codeFileInfo.AppCode)}:{codeFileInfo.AppCode}; {nameof(codeFileInfo.RelativePath)}:'{codeFileInfo.RelativePath}'; {nameof(codeFileInfo.FullPath)}:'{codeFileInfo.FullPath}'";
}
