using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting;
using System.Web;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]
    [Serializable]
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
        private AppDomain _customAppDomain = null;

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
        private RazorComponentBase Webpage
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

        [PrivateApi]
        protected HttpContextBase HttpContextCurrent =>
            _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
        private readonly GetOnce<HttpContextBase> _httpContext = new GetOnce<HttpContextBase>();

        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            var l = Log.Fn();
            try
            {
                //CreateWebPageInstance();
                //InitWebpage();
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

        protected override (string, List<Exception>) RenderTemplate(object data)
        {
            var l = Log.Fn<object>();
            object page = null;





            // create new application domain
            _customAppDomain = CreateNewAppDomain("RazorEngineAssemblyAppDomain");
            //_customAppDomain = AppDomain.CurrentDomain;

            try
            {
                //compiledType = BuildManager.GetCompiledType(TemplatePath);
                var proxyType = typeof(RazorEngineProxy);
                var razorEngineProxy = (RazorEngineProxy)_customAppDomain.CreateInstanceAndUnwrap(proxyType.Assembly.FullName, proxyType.FullName);
                razorEngineProxy.Init(AppCodeFullPath, TemplateFullPath, TemplatePath, Purpose/*, InitHelpers*/, HttpRuntime.BinDirectory);

                // create an instance in the original application domain

                return razorEngineProxy.RenderTemplate(data, proxy, Current);
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
                Unload();
            }
        }

        public static Func<HttpContextBase> Current = () => new HttpContextWrapper(HttpContext.Current);

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
        /// Internally creates a new AppDomain in which Razor templates can
        /// be run.
        /// </summary>
        /// <param name="appDomainName"></param>
        /// <returns></returns>
        //private AppDomain CreateAppDomainOld(string appDomainName)
        //{
        //    if (appDomainName == null)
        //        appDomainName = "RazorHost_" + Guid.NewGuid().ToString("n");


        //    var evidence = AppDomain.CurrentDomain.Evidence;

        //    var setup = new AppDomainSetup();

        //    var appCodeFolder = Path.GetDirectoryName(AppCodeFullPath);
        //    var appFolder = Path.GetDirectoryName(appCodeFolder);
        //    setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory/*appFolder*/;
        //    setup.PrivateBinPath = $"{AppDomain.CurrentDomain.BaseDirectory};{HttpRuntime.BinDirectory};{appFolder};{appCodeFolder}";
        //    //setup.ShadowCopyFiles = "true";
        //    setup.LoaderOptimization = LoaderOptimization.SingleDomain;

        //    var ps = new PermissionSet(PermissionState.Unrestricted);

        //    var localDomain = AppDomain.CreateDomain(appDomainName, evidence, setup, ps);

        //    // *** Need a custom resolver so we can load assembly from non current path
        //    //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

        //    return localDomain;
        //}
        private AppDomain CreateNewAppDomain(string appDomain)
        {
            var appCodeFolder = Path.GetDirectoryName(AppCodeFullPath);
            var appFolder = Path.GetDirectoryName(appCodeFolder);

            var domainSetup = new AppDomainSetup
            {
                ApplicationName = appDomain,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = $"{AppDomain.CurrentDomain.BaseDirectory};{HttpRuntime.BinDirectory};{appFolder};{appCodeFolder}",
                ShadowCopyFiles = "true",

            };
            return AppDomain.CreateDomain(appDomain, null, domainSetup);
        }
        private void Unload()
        {
            if (_customAppDomain == null || _customAppDomain == AppDomain.CurrentDomain) return;
            AppDomain.Unload(_customAppDomain);
            _customAppDomain = null;
        }
    }
}