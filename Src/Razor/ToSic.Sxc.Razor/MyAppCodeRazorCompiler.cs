using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Razor
{
    internal class MyAppCodeRazorCompiler : ServiceBase, IMyAppCodeRazorCompiler
    {
        // TODO: Copy of code from RazorCompiler.cs - should be refactored to use the same code

        #region Constructor and DI

        private readonly ApplicationPartManager _applicationPartManager;
        private readonly IRazorViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IRazorPageActivator _pageActivator;
        private readonly LazySvc<MyAppCodeLoader> _myAppCodeLoader;
        private readonly LazySvc<IServerPaths> _serverPaths;
        private readonly AssemblyResolver _assemblyResolver;

        public MyAppCodeRazorCompiler(
            ApplicationPartManager applicationPartManager,
            IRazorViewEngine viewEngine,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            IRazorPageActivator pageActivator,
            LazySvc<MyAppCodeLoader> myAppCodeLoader,
            LazySvc<IServerPaths> serverPaths,
            AssemblyResolver assemblyResolver) : base($"{Constants.SxcLogName}.RzrCmp")
        {

            ConnectServices(
                _applicationPartManager = applicationPartManager,
                _viewEngine = viewEngine,
                _serviceProvider = serviceProvider,
                _httpContextAccessor = httpContextAccessor,
                _actionContextAccessor = actionContextAccessor,
                _pageActivator = pageActivator,
                _myAppCodeLoader = myAppCodeLoader,
                _serverPaths = serverPaths,
                _assemblyResolver = assemblyResolver
            );
        }
        #endregion

        public async Task<(IView view, ActionContext context)> CompileView(string templatePath, Action<RazorView> configure = null, IApp app = null)
        {
            var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{templatePath},appCodePath:{app}");
            var actionContext = _actionContextAccessor.ActionContext ?? NewActionContext();
            var partial = await FindViewAsync(actionContext, templatePath, app);
            // do callback to configure the object we received
            if (partial is RazorView rzv) configure?.Invoke(rzv);
            return l.ReturnAsOk((partial, actionContext));
        }

        private static bool _executedAlready = false;
        private async Task<IView> FindViewAsync(ActionContext actionContext, string templatePath, IApp app)
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

                var firstAttempt = await GetViewWithAppCodeAsync(templatePath, app);
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

        private async Task<ViewEngineResult> GetViewWithAppCodeAsync(string templatePath, IApp app)
        {
            // get assembly - try to get from cache, otherwise compile
            var codeAssembly = MyAppCodeLoader.TryGetAssemblyOfCodeFromCache(app.AppId, Log)?.Assembly
                               ?? _myAppCodeLoader.Value.GetAppCodeAssemblyOrNull(app.AppId);

            _assemblyResolver.AddAssembly(codeAssembly, app.RelativePath);

            var assemblyLoadContext = new AssemblyLoadContext("UnLoadableAssemblyLoadContext", isCollectible: true);

            var refs = GetMetadataReferences(codeAssembly.Location);
            var fileSystem = RazorProjectFileSystem.Create(app.PhysicalPath);
            var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, fileSystem, (RazorProjectEngineBuilder builder) =>
            {
                builder.AddDefaultImports(new[]
                {
                    "@using Microsoft.AspNetCore.Mvc;",
                    "@using Microsoft.AspNetCore.Mvc.Razor.Internal;",
                    "@using Microsoft.AspNetCore.Mvc.Rendering;",
                    "@using Microsoft.AspNetCore.Mvc.ViewFeatures;",
                    "@using Microsoft.AspNetCore.Razor.Hosting;",
                    "@using System.Runtime.CompilerServices;",
                    "@using System.Threading.Tasks;",
                    //
                    "@using System.Linq;",
                });
            });
            var projectItem = fileSystem.GetItem(_serverPaths.Value.FullContentPath(templatePath));
            var codeDocument = projectEngine.Process(projectItem);

            var generatedCode = codeDocument.GetCSharpDocument().GeneratedCode;

            // 2. Compilation
            var syntaxTree = CSharpSyntaxTree.ParseText(
                    generatedCode,
                    // Add necessary using directives
                    new CSharpParseOptions(LanguageVersion.Latest),
                    encoding: Encoding.UTF8
                );

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithGeneralDiagnosticOption(ReportDiagnostic.Suppress); // suppress all warning, to not be treated as compile errors
                                                                         //.WithSpecificDiagnosticOptions(new Dictionary<string, ReportDiagnostic>
                                                                         //{
                                                                         //    { "CS1701", ReportDiagnostic.Suppress }
                                                                         //});

            var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(templatePath))
                .WithOptions(compilationOptions)
                .AddReferences(/*GetMetadataReferences(appCodeFullPath)*/refs)
                .AddSyntaxTrees(syntaxTree);

            using var peStream = new MemoryStream();
            var result = compilation.Emit(peStream);
            if (!result.Success)
            {
                //l.E("Compilation done with error.");
                var errors = new List<string>();

                var failures = result.Diagnostics.Where(diagnostic =>
                    /*diagnostic.IsWarningAsError ||*/ diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    //l.A("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                }

                throw new InvalidOperationException(string.Join("\n", errors));
            }

            peStream.Seek(0, SeekOrigin.Begin);

            // 3. Loading and Execution
            var assembly = assemblyLoadContext.LoadFromStream(peStream);

            var viewType = assembly.GetType("Razor.Template");

            var instance = Activator.CreateInstance(viewType);

            assemblyLoadContext.Unload();

            if (instance is not IRazorPage page)
                return ViewEngineResult.NotFound(templatePath, new string[] { "View not found" });

            page.Path = templatePath;

            // Create an IView instance from the compiled assembly
            var viewInstance = new RazorView(_viewEngine, _pageActivator, Array.Empty<IRazorPage>(), page, HtmlEncoder.Default, new DiagnosticListener(templatePath));
            return ViewEngineResult.Found(templatePath, viewInstance);
        }

        private ActionContext NewActionContext()
        {
            var l = Log.Fn<ActionContext>();
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
            return l.ReturnAsOk(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));
        }

        private List<MetadataReference> GetMetadataReferences(string appCodeFullPath)
        {
            var references = new List<MetadataReference>
            {
                // MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // Commented because it solves error when "refs" are referenced.
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.LoadFile(typeof(Microsoft.AspNetCore.Mvc.Razor.RazorPage).Assembly.Location).Location),
            };

            if (File.Exists(appCodeFullPath))
                references.Add(MetadataReference.CreateFromFile(appCodeFullPath));

            RazorReferencedAssemblies().ToList().ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            // Add references to all dll's in bin folder.
            var dllLocation = AppContext.BaseDirectory;
            var dllPath = Path.GetDirectoryName(dllLocation);
            foreach (string dllFile in Directory.GetFiles(dllPath, "*.dll"))
                references.Add(MetadataReference.CreateFromFile(Assembly.LoadFile(dllFile).Location));
            foreach (string dllFile in Directory.GetFiles(Path.Combine(dllPath, "refs"), "*.dll"))
                references.Add(MetadataReference.CreateFromFile(/*Assembly.Load(Path.GetFileNameWithoutExtension(dllFile)).Location*/dllFile));

            return references;
        }

        private static string[] RazorReferencedAssemblies()
        {
            return new string[] {
              "Microsoft.AspNetCore",
              "Microsoft.AspNetCore.Authorization.Policy",
              "Microsoft.AspNetCore.Diagnostics",
              "Microsoft.AspNetCore.Hosting.Abstractions",
              "Microsoft.AspNetCore.Html.Abstractions",
              "Microsoft.AspNetCore.Http.Abstractions",
              "Microsoft.AspNetCore.HttpsPolicy",
              "Microsoft.AspNetCore.Mvc",
              "Microsoft.AspNetCore.Mvc.Abstractions",
              "Microsoft.AspNetCore.Mvc.Core",
              "Microsoft.AspNetCore.Mvc.Razor",
              "Microsoft.AspNetCore.Mvc.RazorPages",
              "Microsoft.AspNetCore.Mvc.TagHelpers",
              "Microsoft.AspNetCore.Mvc.ViewFeatures",
              "Microsoft.AspNetCore.Razor",
              "Microsoft.AspNetCore.Razor.Runtime",
              "Microsoft.AspNetCore.Routing",
              "Microsoft.AspNetCore.StaticFiles",
              "Microsoft.Extensions.DependencyInjection.Abstractions",
              "Microsoft.Extensions.Hosting.Abstractions",
              "Microsoft.Extensions.Logging.Abstractions",
              "System.Diagnostics.DiagnosticSource",
              "System.Linq.Expressions",
              "System.Runtime",
              "System.Runtime.Loader",
              "System.Text.Encodings.Web",
            };
        }
    }
}