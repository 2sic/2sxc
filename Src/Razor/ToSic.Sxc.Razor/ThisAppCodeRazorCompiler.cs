using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;
using IView = Microsoft.AspNetCore.Mvc.ViewEngines.IView;

namespace ToSic.Sxc.Razor;

internal class ThisAppCodeRazorCompiler : ServiceBase, IThisAppCodeRazorCompiler
{
    // TODO: Copy of code from RazorCompiler.cs - should be refactored to use the same code

    #region Constructor and DI

    private readonly ApplicationPartManager _applicationPartManager;
    private readonly IRazorViewEngine _viewEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IRazorPageActivator _pageActivator;
    private readonly LazySvc<ThisAppLoader> _thisAppCodeLoader;
    private readonly LazySvc<IServerPaths> _serverPaths;
    private readonly AssemblyResolver _assemblyResolver;
    private readonly HotBuildReferenceManager _referenceManager;

    public ThisAppCodeRazorCompiler(
        ApplicationPartManager applicationPartManager,
        IRazorViewEngine viewEngine,
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor,
        IActionContextAccessor actionContextAccessor,
        IRazorPageActivator pageActivator,
        LazySvc<ThisAppLoader> thisAppCodeLoader,
        LazySvc<IServerPaths> serverPaths,
        AssemblyResolver assemblyResolver,
        HotBuildReferenceManager referenceManager) : base($"{SxcLogging.SxcLogName}.RzrCmp")
    {
        ConnectServices(
            _applicationPartManager = applicationPartManager,
            _viewEngine = viewEngine,
            _serviceProvider = serviceProvider,
            _httpContextAccessor = httpContextAccessor,
            _actionContextAccessor = actionContextAccessor,
            _pageActivator = pageActivator,
            _thisAppCodeLoader = thisAppCodeLoader,
            _serverPaths = serverPaths,
            _assemblyResolver = assemblyResolver,
            _referenceManager = referenceManager
        );
    }
    #endregion

    public async Task<(IView view, ActionContext context)> CompileView(string templatePath, Action<RazorView> configure = null, IApp app = null, HotBuildSpec spec = default)
    {
        var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{templatePath},appCodePath:{app}");
        var actionContext = _actionContextAccessor.ActionContext ?? NewActionContext();
        var partial = await FindViewAsync(actionContext, templatePath, app, spec);
        // do callback to configure the object we received
        if (partial is RazorView rzv) configure?.Invoke(rzv);
        return l.ReturnAsOk((partial, actionContext));
    }

    private static bool _executedAlready = false;
    private async Task<IView> FindViewAsync(ActionContext actionContext, string templatePath, IApp app, HotBuildSpec spec)
    {
        var l = Log.Fn<IView>($"partialName:{templatePath}");
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

            var firstAttempt = GetViewWithAppCodeAsync(templatePath, app, spec);
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
            var secondAttempt = _viewEngine.FindView(actionContext, templatePath, false);
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
            new[] { $"Unable to find partial '{templatePath}'. The following locations were searched:" }.Concat(searchedLocations));
        l.A($"error:{errorMessage}");
        throw new InvalidOperationException(errorMessage);
    }

    private ViewEngineResult GetViewWithAppCodeAsync(string templatePath, IApp app, HotBuildSpec spec)
    {
        var l = Log.Fn<ViewEngineResult>($"{nameof(templatePath)}:{templatePath}; {nameof(app.RelativePath)}:{app.RelativePath}; {spec}", timer: true);

        // get assembly - try to get from cache, otherwise compile
        //var codeAssembly = ThisAppLoader.TryGetAssemblyOfThisAppFromCache(spec, Log)?.Assembly
        //                   ?? _thisAppCodeLoader.Value.GetThisAppAssemblyOrThrow(spec);
        var (codeAssembly, _) = _thisAppCodeLoader.Value.TryGetOrFallback(spec);
        l.A($"has AppCode assembly: {codeAssembly != null}");

        if (codeAssembly != null)
        {
            var appRelativePathWithEdition = spec.Edition.HasValue() ? Path.Combine(app.RelativePath, spec.Edition) : app.RelativePath;
            l.A($"{nameof(appRelativePathWithEdition)}: {appRelativePathWithEdition}");

            // add assembly to resolver, so it will be provided to the compiler when used in cshtml
            _assemblyResolver.AddAssembly(codeAssembly, appRelativePathWithEdition);
        }

        // setup RazorProjectEngine
        var fileSystem = RazorProjectFileSystem.Create(app.PhysicalPath);
        var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, (RazorProjectEngineBuilder builder) =>
        {
            builder.AddDefaultImports(DefaultImports); // implicit usings
        });
        var projectItem = fileSystem.GetItem(_serverPaths.Value.FullContentPath(templatePath));

        var lProcess = Log.Fn($"generate codeDocument from razor", timer: true);
        var codeDocument = projectEngine.Process(projectItem);
        lProcess.Done();

        var lGenerateCode = Log.Fn("generate c# code from codeDocument", timer: true);
        var generatedCode = codeDocument.GetCSharpDocument().GeneratedCode;
        lGenerateCode.Done();

        var lParse = Log.Fn("generate SyntaxTree from c# code", timer: true);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            generatedCode,
            // Add necessary using directives
            new(LanguageVersion.Latest),
            encoding: Encoding.UTF8
        );
        lParse.Done();

        var lRefs = Log.Fn("prepare metadata references", timer: true);
        var refs = _referenceManager.GetMetadataReferences(codeAssembly?.Location, spec);
        lRefs.Done();

        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            .WithGeneralDiagnosticOption(ReportDiagnostic.Suppress); // suppress all warning, to not be treated as compile errors
        //.WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
        //{
        //    { "CS1701", ReportDiagnostic.Suppress }
        //});

        var lCompilation = Log.Fn($"compile: {templatePath}", timer: true);
        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(templatePath))
            .WithOptions(compilationOptions)
            .AddReferences(refs)
            .AddSyntaxTrees(syntaxTree);


        using var peStream = new MemoryStream();
        var result = compilation.Emit(peStream);
        if (!result.Success)
        {
            l.E("Compilation done with error.");
            var errors = new List<string>();

            var failures = result.Diagnostics.Where(diagnostic =>
                /*diagnostic.IsWarningAsError ||*/ diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                //l.A("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            }

            var errorMessage = string.Join("\n", errors);
            lCompilation.E(errorMessage);
            lCompilation.Done();
            l.Done();
            throw new InvalidOperationException(errorMessage);
        }
        lCompilation.Done();

        var lCreate = Log.Fn($"create instance", timer: true);
        peStream.Seek(0, SeekOrigin.Begin);

        // 3. Loading and Execution
        var assemblyLoadContext = new AssemblyLoadContext("UnLoadableAssemblyLoadContext", isCollectible: true);
        var assembly = assemblyLoadContext.LoadFromStream(peStream);

        var viewType = assembly.GetType("Razor.Template");

        var instance = Activator.CreateInstance(viewType);

        assemblyLoadContext.Unload();
        lCreate.Done();

        if (instance is not IRazorPage page)
            return l.ReturnAsError(ViewEngineResult.NotFound(templatePath, new string[] { "View not found" }), "View not found");

        page.Path = templatePath;

        // Create an IView instance from the compiled assembly
        var viewInstance = new RazorView(_viewEngine, _pageActivator, Array.Empty<IRazorPage>(), page, HtmlEncoder.Default, new(templatePath));
        return l.ReturnAsOk(ViewEngineResult.Found(templatePath, viewInstance));
    }

    private string[] DefaultImports => _defaultImports.Get(GetDefaultImports);
    private readonly GetOnce<string[]> _defaultImports = new();
    private static string[] GetDefaultImports()
    {
        var implicitUsings = new List<string>(ImplicitUsings.ForRazor) {

            // based on cshtml decompile POC
            "Microsoft.AspNetCore.Mvc.Rendering" // 'IHtmlHelper<>' is required for '@Html.Partial(...)'

        };

        return implicitUsings.Select(u => $"@using global::{u};").ToArray();
    }

    private ActionContext NewActionContext()
    {
        var l = Log.Fn<ActionContext>();
        var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
        return l.ReturnAsOk(new(httpContext, new(), new()));
    }
}