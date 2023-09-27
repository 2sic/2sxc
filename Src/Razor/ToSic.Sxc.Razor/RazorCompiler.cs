using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Html5;

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
        private readonly IRazorPageActivator _pageActivator;

        public RazorCompiler(
            ApplicationPartManager applicationPartManager,
            IRazorViewEngine viewEngine,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            IRazorPageActivator pageActivator) : base($"{Constants.SxcLogName}.RzrCmp")
        {
            ConnectServices(
                _applicationPartManager = applicationPartManager,
                _viewEngine = viewEngine,
                _serviceProvider = serviceProvider,
                _httpContextAccessor = httpContextAccessor,
                _actionContextAccessor = actionContextAccessor,
                _pageActivator = pageActivator
            );
        }
        #endregion

        public async Task<(IView view, ActionContext context)> CompileView(string templatePath, Action<RazorView> configure = null,
            string appCodeFullPath = null, string templateFullPath = null)
        {
            var l = Log.Fn<(IView view, ActionContext context)>($"partialName:{templatePath},appCodePath:{appCodeFullPath}");
            var actionContext = _actionContextAccessor.ActionContext ?? NewActionContext();
            var partial = await FindViewAsync(actionContext, templatePath, appCodeFullPath, templateFullPath);
            // do callback to configure the object we received
            if (partial is RazorView rzv) configure?.Invoke(rzv);
            return l.ReturnAsOk((partial, actionContext));
        }

        private static bool _executedAlready = false;
        private async Task<IView> FindViewAsync(ActionContext actionContext, string templatePath, string appCodePath = null, string templateFullPath = null)
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

                //AssemblyPart applicationPart = null;
                //if (!string.IsNullOrEmpty(appCodePath) && File.Exists(appCodePath))
                //{
                //    Assembly.LoadFrom(appCodePath);
                //    applicationPart = new AssemblyPart(Assembly.LoadFrom(appCodePath));
                //    var appCodePathWithoutExtension = appCodePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? appCodePath.Substring(0, appCodePath.Length - 4) : appCodePath;
                //    applicationPart = new CompilationReferencesProvider(_assemblyLoadContext.LoadFromAssemblyPath(appCodePath));
                //}


                var firstAttempt = await GetViewWithAppCodeAsync(/*applicationPart*/null, templatePath, appCodePath, templateFullPath);
                l.A($"firstAttempt: {firstAttempt}");

                //if (applicationPart != null)
                //    _applicationPartManager.ApplicationParts.Add(applicationPart);

                //var firstAttempt = _viewEngine.GetView(null, partialName, false);
                //l.A($"firstAttempt: {firstAttempt}");

                //if (applicationPart != null)
                //    _applicationPartManager.ApplicationParts.Remove(applicationPart);

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

        //private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private AssemblyLoadContext _assemblyLoadContext;
        private Dictionary<string, Task<CompiledViewDescriptor>>? _compiledViews;

        public async Task<ViewEngineResult> GetViewWithAppCodeAsync(ApplicationPart applicationPart, string templatePath, string appCodeFullPath, string templateFullPath = null)
        {
            _assemblyLoadContext = new AssemblyLoadContext("UnLoadableAssemblyLoadContext", isCollectible: true);
            // _assemblyLoadContext = AssemblyLoadContext.Default;
            var refs = GetMetadataReferences(appCodeFullPath);
            //await _semaphore.WaitAsync();  // acquire the semaphore
            //try
            //{
            //    if (applicationPart != null)
            //        _applicationPartManager.ApplicationParts.Add(applicationPart);

            //var firstAttempt = _viewEngine.GetView(null, templatePath, false);

            //_assemblyLoadContext.Unload();

            //return firstAttempt;

            //    if (applicationPart != null)
            //        _applicationPartManager.ApplicationParts.Remove(applicationPart);

            //    return firstAttempt;
            //}
            //finally
            //{
            //    _semaphore.Release();  // release the semaphore
            //}



            //var viewsFeature = new ViewsFeature();
            //_applicationPartManager.PopulateFeature(viewsFeature);

            //EnsureCompiledViews();

            //return Task.FromResult(new CompiledViewDescriptor
            //{
            //    RelativePath = normalizedPath,
            //    ExpirationTokens = Array.Empty<IChangeToken>(),
            //});






            var appProjectRoot = new DirectoryInfo(appCodeFullPath)?.Parent?.Parent?.FullName ?? Path.GetDirectoryName(templateFullPath ?? templatePath); ;
            var fileSystem = RazorProjectFileSystem.Create(appProjectRoot);
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
            var projectItem = fileSystem.GetItem(templateFullPath);
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
            var assembly = _assemblyLoadContext.LoadFromStream(peStream);

            var viewType = assembly.GetType("Razor.Template");

            var instance = Activator.CreateInstance(viewType);
            _assemblyLoadContext.Unload();

            //var compileTask = Compiler.CompileAsync(relativePath);
            //var viewDescriptor = compileTask.GetAwaiter().GetResult();

            //var viewType = viewDescriptor.Type;
            //if (viewType != null)
            //{
            //    var newExpression = Expression.New(viewType);
            //    var pathProperty = viewType.GetProperty(nameof(IRazorPage.Path))!;

            //    // Generate: page.Path = relativePath;
            //    // Use the normalized path specified from the result.
            //    var propertyBindExpression = Expression.Bind(pathProperty, Expression.Constant(viewDescriptor.RelativePath));
            //    var objectInitializeExpression = Expression.MemberInit(newExpression, propertyBindExpression);
            //    var pageFactory = Expression
            //        .Lambda<Func<IRazorPage>>(objectInitializeExpression)
            //        .Compile();
            //    return new RazorPageFactoryResult(viewDescriptor, pageFactory);
            //}
            //else
            //{
            //    return new RazorPageFactoryResult(viewDescriptor, razorPageFactory: null);
            //}

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
            //var dllLocation = AppContext.BaseDirectory; // typeof(ToSic.Sxc.Razor.StartupRazor).Assembly.Location;
            //var bin = Path.GetDirectoryName(dllLocation);
            //var references = Directory.GetFiles(bin, "*.dll").Select(dllFile => MetadataReference.CreateFromFile(dllFile)).ToArray();
            //var referencesRefs = Directory.GetFiles(Path.Combine(bin, "refs"), "*.dll").Select(dllFile => MetadataReference.CreateFromFile(dllFile)).ToArray();

            var references = new List<MetadataReference>
            {
                // MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // Commented because it solves error when "refs" are referenced.
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                MetadataReference.CreateFromFile(Assembly.LoadFile(typeof(Microsoft.AspNetCore.Mvc.Razor.RazorPage).Assembly.Location).Location),
            };

            if (File.Exists(appCodeFullPath))
                references.Add(MetadataReference.CreateFromFile(_assemblyLoadContext.LoadFromAssemblyPath(appCodeFullPath).Location));

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

            //var razorAssemblyNames = new string[] { };
            //// add the assembly containing the RazorViewEngine
            //razorAssemblyNames[0] = typeof(Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine).Assembly.Location;
            //// add the assembly containing the IHostingEnvironment
            //razorAssemblyNames[1] = typeof(Microsoft.AspNetCore.Hosting.IHostingEnvironment).Assembly.Location;
            //// add the assembly containing the IHtmlHelper
            //razorAssemblyNames[2] = typeof(Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper).Assembly.Location;
            //return razorAssemblyNames;
        }

        private void EnsureCompiledViews(/*ILogger logger*/)
        {
            if (_compiledViews is not null)
            {
                return;
            }

            var viewsFeature = new ViewsFeature();
            _applicationPartManager.PopulateFeature(viewsFeature);

            // We need to validate that the all compiled views are unique by path (case-insensitive).
            // We do this because there's no good way to canonicalize paths on windows, and it will create
            // problems when deploying to linux. Rather than deal with these issues, we just don't support
            // views that differ only by case.
            var compiledViews = new Dictionary<string, Task<CompiledViewDescriptor>>(
                viewsFeature.ViewDescriptors.Count,
                StringComparer.OrdinalIgnoreCase);

            foreach (var compiledView in viewsFeature.ViewDescriptors)
            {
                // Log.ViewCompilerLocatedCompiledView(logger, compiledView.RelativePath);

                if (!compiledViews.ContainsKey(compiledView.RelativePath))
                {
                    // View ordering has precedence semantics, a view with a higher precedence was not
                    // already added to the list.
                    compiledViews.TryAdd(compiledView.RelativePath, Task.FromResult(compiledView));
                }
            }

            //if (compiledViews.Count == 0)
            //{
            //    Log.ViewCompilerNoCompiledViewsFound(logger);
            //}

            // Safe races should be ok. We would end up logging multiple times
            // if this is invoked concurrently, but since this is primarily a dev-scenario, we don't think
            // this will happen often. We could always re-consider the logging if we get feedback.
            _compiledViews = compiledViews;
        }

    }
}