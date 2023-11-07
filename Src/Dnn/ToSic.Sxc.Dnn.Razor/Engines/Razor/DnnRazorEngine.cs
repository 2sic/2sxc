using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Apps.Paths;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]
    // ReSharper disable once UnusedMember.Global
    public partial class DnnRazorEngine : EngineBase, IRazorEngine
    {
        #region Constructor / DI

        private readonly LazySvc<CodeErrorHelpService> _errorHelp;
        private readonly CodeRootFactory _codeRootFactory;
        private readonly LazySvc<DnnRazorSourceAnalyzer> _sourceAnalyzer;

        /// <summary>
        /// Internal instance of the AppDomain to hang onto when
        /// running in a separate AppDomain. Ensures the AppDomain
        /// stays alive.
        /// </summary>
        public AppDomain LocalAppDomain = null;
        public Assembly GeneratedAssembly;
        public string SafeClassName;

        public DnnRazorEngine(MyServices helpers, CodeRootFactory codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<DnnRazorSourceAnalyzer> sourceAnalyzer) : base(helpers)
        {
            ConnectServices(
                _codeRootFactory = codeRootFactory,
                _errorHelp = errorHelp,
                _sourceAnalyzer = sourceAnalyzer
            );
        }


        #endregion


        [PrivateApi]
        protected RazorComponentBase Webpage
        {
            get
            {
                Log.A($"Webpage get: {_webpage}");
                return _webpage;
            }
            set
            {
                Log.A($"Webpage set: {value}");
                _webpage = value;
            }

        }
        private RazorComponentBase _webpage;

        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            var l = Log.Fn();
            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error. Your web.config seems to be wrong in the 2sxc folder.", exc);
                //old till 2023-05-11 " Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                // see https://web.archive.org/web/20131201093234/http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7
                throw l.Done(e);
            }
            l.Done();
        }

        [PrivateApi]
        protected HttpContextBase HttpContextCurrent =>
            _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
        private readonly GetOnce<HttpContextBase> _httpContext = new GetOnce<HttpContextBase>();


        [PrivateApi]
        private (TextWriter writer, List<Exception> exception) Render(TextWriter writer, object data)
        {
            var l = Log.Fn<(TextWriter writer, List<Exception> exception)>(message: "will render into TextWriter");
            var page = Webpage; // access the property once only
            try
            {
                if (data != null && page is ISetDynamicModel setDyn)
                    setDyn.SetDynamicModel(data);
            }
            catch (Exception e)
            {
                l.Ex("Problem with setting dynamic model, error will be ignored.", e);
            }

            try
            {
                page.ExecutePageHierarchy(new WebPageContext(HttpContextCurrent, page, data), writer, page);
            }
            catch (Exception maybeIEntityCast)
            {
                var ex = l.Ex(_errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
                // Special form of throw to preserve details about the call stack
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw; // fake throw, just so the code shows what happens
            }
            return l.Return((writer, page.SysHlp.ExceptionsOrNull));
        }

        [PrivateApi]
        protected override (string, List<Exception>) RenderTemplate(object data)
        {
            var l = Log.Fn<(string, List<Exception>)>();
            var writer = new StringWriter();
            var result = Render(writer, data);
            return l.ReturnAsOk((result.writer.ToString(), result.exception));
        }

        private object CreateWebPageInstance()
        {
            // create new application domain
            LocalAppDomain = CreateAppDomain(null);

            var l = Log.Fn<object>();
            object page = null;
            Type compiledType;
            try
            {
                //compiledType = BuildManager.GetCompiledType(TemplatePath);

                var razorEngineProxy = new RazorEngineProxy(AppCodeFullPath, TemplateFullPath, LocalAppDomain);

                SafeClassName = GetSafeClassName(TemplateFullPath);
                var template = File.ReadAllText(TemplateFullPath);
                string assemblyId;
                using (var reader = new StringReader(template))
                {
                    assemblyId = razorEngineProxy.CompileTemplate(reader, SafeClassName, null);
                    if (assemblyId == null)
                        throw new Exception(razorEngineProxy.ErrorMessage);
                }

                //GeneratedAssembly = razorEngineProxy.GetAssembly(razorEngineProxy.AssemblyCache[assemblyId].Location);
                GeneratedAssembly = razorEngineProxy.AssemblyCache[assemblyId];

                // find the generated type to instantiate
                compiledType = GeneratedAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(SafeClassName));

            }
            catch (Exception compileEx)
            {
                // TODO: ADD MORE compile error help
                // 1. Read file
                // 2. Try to find base type - or warn if not found
                // 3. ...
                var razorType = _sourceAnalyzer.Value.TypeOfVirtualPath(TemplatePath);
                l.A($"Razor Type: {razorType}");
                var ex = l.Ex(_errorHelp.Value.AddHelpForCompileProblems(compileEx, razorType));
                // Special form of throw to preserve details about the call stack
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw; // fake throw, just so the code shows what happens
            }

            try
            {
                if (compiledType == null)
                    return l.ReturnNull("type not found");

                page = Activator.CreateInstance(compiledType);
                //LocalAppDomain.CreateInstanceAndUnwrap(GeneratedAssembly.FullName, SafeClassName);
                //LocalAppDomain.CreateInstanceFrom(GeneratedAssembly.Location, $"Razor.{SafeClassName}", false, BindingFlags.Default, null,
                //        new object[] { null }, CultureInfo.CurrentCulture, null)
                //LocalAppDomain.CreateInstanceFromAndUnwrap(GeneratedAssembly.Location, $"Razor.{SafeClassName}", null);


                var pageObjectValue = RuntimeHelpers.GetObjectValue(page);
                return l.ReturnAsOk(pageObjectValue);
            }
            catch (Exception createInstanceException)
            {
                var ex = l.Ex(_errorHelp.Value.AddHelpIfKnownError(createInstanceException, page));
                // Special form of throw to preserve details about the call stack
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw; // fake throw, just so the code shows what happens
            }
            finally
            {
                AppDomain.Unload(LocalAppDomain);
                //DeleteAssembly(GeneratedAssembly.Location);
            }
        }

        private bool InitWebpage()
        {
            var l = Log.Fn<bool>();
            if (string.IsNullOrEmpty(TemplatePath)) return l.ReturnFalse("null path");

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException($"The webpage found at '{TemplatePath}' was not created.");

            if (!(objectValue is RazorComponentBase pageToInit))
                throw new InvalidOperationException($"The webpage at '{TemplatePath}' must derive from RazorComponentBase.");
            Webpage = pageToInit;

            pageToInit.Context = HttpContextCurrent;
            pageToInit.VirtualPath = TemplatePath;
            //var compatibility = Constants.CompatibilityLevel9Old;
            if (pageToInit is RazorComponent rzrPage)
            {
#pragma warning disable CS0618
                rzrPage.Purpose = Purpose;
#pragma warning restore CS0618
                //compatibility = Constants.CompatibilityLevel10;
            }

            //var compatibility = (pageToInit as ICompatibilityLevel)?.CompatibilityLevel ?? Constants.CompatibilityLevel9Old;
            //if (pageToInit is IDynamicCode16)
            //    compatibility = Constants.CompatibilityLevel16;
            //else if (pageToInit is ICompatibleToCode12)
            //    compatibility = Constants.CompatibilityLevel12;

            if (pageToInit is SexyContentWebPage oldPage)
#pragma warning disable 618, CS0612
                oldPage.InstancePurpose = (InstancePurposes)Purpose;
#pragma warning restore 618, CS0612
            InitHelpers(pageToInit);
            return l.ReturnTrue("ok");
        }

        private void InitHelpers(RazorComponentBase webPage)
        {
            var l = Log.Fn();
            var dynCode = _codeRootFactory.BuildCodeRoot(webPage, Block, Log, compatibilityFallback: Constants.CompatibilityLevel9Old);
            //dynCode.InitDynCodeRoot(Block, Log); //, compatibility)
            //.SetCompatibility(compatibility);
            webPage.ConnectToRoot(dynCode);

            // New in 10.25 - ensure jquery is not included by default
            if (dynCode.Cdf.CompatibilityLevel > Constants.MaxLevelForAutoJQuery)
            {
                l.A("Compatibility is new, don't need AutoJQuery");
                CompatibilityAutoLoadJQueryAndRvt = false;
            }
            l.Done();
        }



        // ------------ POC -----

        public void DeleteAssembly(string assemblyPath)
        {
            try
            {

                var pdbPath = Path.ChangeExtension(assemblyPath, "pdb");
                if (File.Exists(pdbPath)) File.Delete(pdbPath);
            }
            catch
            {
                // ignored
            }

            try
            {
                if (File.Exists(assemblyPath)) File.Delete(assemblyPath);
            }
            catch
            {
                // ignored
            }
        }


        /// <summary>
        /// Last generated output
        /// </summary>



        ///// <summary>
        ///// Method to add namespaces to the compiled code.
        ///// Add namespaces to minimize explicit namespace
        ///// requirements in your Razor template code.
        ///// 
        ///// Make sure that any required assemblies are
        ///// loaded first.
        ///// </summary>
        ///// <param name="ns">First namespace</param>
        ///// <param name="additionalNamespaces">additional namespaces</param>
        //public void AddNamespace(string ns, params string[] additionalNamespaces)
        //{
        //    ReferencedNamespaces.Add(ns);
        //    if (additionalNamespaces != null)
        //    {
        //        foreach (string ans in additionalNamespaces)
        //        {
        //            ReferencedNamespaces.AddRange(additionalNamespaces);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Method to add assemblies to the referenced assembly list.
        ///// Use the DLL name or strongly typed name. Assembly added HAS 
        ///// to be accessible via GAC or in bin/privatebin path
        ///// </summary>
        ///// <param name="assemblyPath">
        ///// Path to the assembly. GAC'd assemblies or assemblies in current path
        ///// can be provided without a path. All others should contain a fully qualified OS path. 
        ///// Note that Razor does not look in the PrivateBin path for the AppDomain.
        ///// </param>
        ///// <param name="additionalAssemblies">Additional assembly paths to add </param>
        //public void AddAssembly(string assemblyPath, params string[] additionalAssemblies)
        //{
        //    ReferencedAssemblies.Add(assemblyPath);
        //    if (additionalAssemblies != null)
        //        ReferencedAssemblies.AddRange(additionalAssemblies);
        //}

        /// <summary>
        /// Returns a unique ClassName for a template to execute
        /// Optionally pass in an objectId on which the code is based
        /// or null to get default behavior.
        /// 
        /// Default implementation just returns Guid as string
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        protected string GetSafeClassName(object objectId)
        {
            return "_" + Guid.NewGuid().ToString().Replace("-", "_");
        }


        /// <summary>
        /// Internally creates a new AppDomain in which Razor templates can
        /// be run.
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <returns></returns>
        private AppDomain CreateAppDomain(string appDomainName)
        {
            if (appDomainName == null)
                appDomainName = "RazorHost_" + Guid.NewGuid().ToString("n");


            var evidence = AppDomain.CurrentDomain.Evidence;

            var setup = new AppDomainSetup();

            var appCodeFolder = Path.GetDirectoryName(AppCodeFullPath);
            var appFolder = Path.GetDirectoryName(appCodeFolder);
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory/*appFolder*/;
            setup.PrivateBinPath = $"{AppDomain.CurrentDomain.BaseDirectory};{HttpRuntime.BinDirectory};{appFolder};{appCodeFolder}";
            //setup.ShadowCopyFiles = "true";
            setup.LoaderOptimization = LoaderOptimization.SingleDomain;

            var ps = new PermissionSet(PermissionState.Unrestricted);

            var localDomain = AppDomain.CreateDomain(appDomainName, evidence, setup, ps);

            // *** Need a custom resolver so we can load assembly from non current path
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            return localDomain;
        }

    }
}