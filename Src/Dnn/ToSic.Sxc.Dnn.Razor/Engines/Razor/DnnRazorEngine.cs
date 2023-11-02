using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages;
using Microsoft.CSharp;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
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
        AppDomain LocalAppDomain = null;

        public DnnRazorEngine(MyServices helpers, CodeRootFactory codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<DnnRazorSourceAnalyzer> sourceAnalyzer) : base(helpers)
        {
            BaseClassType = typeof(System.Web.WebPages.WebPageBase/*Custom.Hybrid.RazorTyped*/); // TODO: Read from @inherits OR fallback to ???
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
        public Type BaseClassType;

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


                //string outputAssemblyName = "App_Web_" + BuildManager.GenerateRandomAssemblyName(BuildManager.GetGeneratedAssemblyBaseName(virtualPath), false);
                //BuildProvidersCompiler providersCompiler = new System.Web.Compilation.BuildProvidersCompiler(virtualPath, outputAssemblyName);
                //BuildProvider buildProvider = BuildManager.CreateBuildProvider(virtualPath, providersCompiler.CompConfig, providersCompiler.ReferencedAssemblies, true);
                //providersCompiler.SetBuildProviders((ICollection)new SingleObjectCollection((object)buildProvider));
                //BuildResult result1;
                //try
                //{
                //    CompilerResults results = providersCompiler.PerformBuild();
                //    result1 = buildProvider.GetBuildResult(results);
                //}
                //catch (HttpCompileException ex)
                //{
                //    if (ex.DontCache)
                //    {
                //        throw;
                //    }
                //    else
                //    {
                //        BuildResult result2 = (BuildResult)new BuildResultCompileError(virtualPath, ex);
                //        buildProvider.SetBuildResultDependencies(result2);
                //        ex.VirtualPathDependencies = buildProvider.VirtualPathDependencies;
                //        this.CacheVPathBuildResultInternal(virtualPath, result2, utcNow);
                //        ex.DontCache = true;
                //        throw;
                //    }
                //}
                //if (result1 == null)
                //    return (BuildResult)null;
                //this.CacheVPathBuildResultInternal(virtualPath, result1, utcNow);
                //if (!this._precompilingApp && BuildResultCompiledType.UsesDelayLoadType(result1))
                //{
                //    if (cacheKey == null)
                //        cacheKey = BuildManager.GetCacheKeyFromVirtualPath(virtualPath);
                //    result1 = BuildManager.GetBuildResultFromCache(cacheKey);
                //}
                //return result1;

                AssemblyCache = new Dictionary<string, Assembly>();
                ErrorMessage = string.Empty;

                ReferencedNamespaces = new List<string>();
                ReferencedNamespaces.Add("System");
                ReferencedNamespaces.Add("System.Text");
                ReferencedNamespaces.Add("System.Collections.Generic");
                ReferencedNamespaces.Add("System.Linq");
                ReferencedNamespaces.Add("System.IO");
                ReferencedNamespaces.Add("System.Web.WebPages");

                ReferencedAssemblies = new List<string>();
                ReferencedAssemblies.Add("System.dll");
                ReferencedAssemblies.Add("System.Core.dll");
                ReferencedAssemblies.Add(typeof(IHtmlString).Assembly.Location);    // System.Web
                ReferencedAssemblies.Add("Microsoft.CSharp.dll");   // dynamic support!

                // reference all assemblies form bin folder
                foreach (var dll in Directory.GetFiles(HttpRuntime.BinDirectory, "*.dll"))
                    ReferencedAssemblies.Add(dll);

                if (File.Exists(AppCodeFullPath))
                {
                    ReferencedAssemblies.Add(AppCodeFullPath);
                    // load referenced Assemblies to LocalAppDomain
                    //LocalAppDomain.Load(AssemblyName.GetAssemblyName(AppCodeFullPath));
                }

                if (CodeProvider == null) CodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

                //foreach (string Namespace in ReferencedNamespaces)
                //    AddNamespace(Namespace);

                //foreach (string assembly in ReferencedAssemblies)
                //    AddAssembly(assembly);

                SafeClassName = GetSafeClassName(TemplateFullPath);
                var template = File.ReadAllText(TemplateFullPath);
                string assemblyId;
                using (var reader = new StringReader(template))
                {
                    assemblyId = CompileTemplate(reader, SafeClassName, null);
                    if (assemblyId == null)
                        throw new Exception(ErrorMessage);
                }

                GeneratedAssembly = AssemblyCache[assemblyId];

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

                page = Activator.CreateInstance(compiledType) /*LocalAppDomain.CreateInstanceAndUnwrap(GeneratedAssembly.FullName, SafeClassName)*/;
                AppDomain.Unload(LocalAppDomain);
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



        /// <summary>
        /// The code provider used with this instance
        /// </summary>
        internal CSharpCodeProvider CodeProvider { get; set; }

        /// <summary>
        /// A list of default namespaces to include
        /// 
        /// Defaults already included:
        /// System, System.Text, System.IO, System.Collections.Generic, System.Linq
        /// </summary>
        internal List<string> ReferencedNamespaces { get; set; }

        /// <summary>
        /// A list of default assemblies referenced during compilation
        /// 
        /// Defaults already included:
        /// System, System.Text, System.IO, System.Collections.Generic, System.Linq
        /// </summary>
        internal List<string> ReferencedAssemblies { get; set; }

        /// <summary>
        /// Internally cache assemblies loaded with ParseAndCompileTemplate.        
        /// Assemblies are cached in the EngineHost so they don't have
        /// to cross AppDomains for invocation when running in a separate AppDomain
        /// </summary>
        protected Dictionary<string, Assembly> AssemblyCache { get; set; }

        /// <summary>
        /// Any errors that occurred during template execution
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Last generated output
        /// </summary>
        public string LastGeneratedCode
        {
            get => GetTextWithLineNumbers(_LastGeneratedCode);
            set => _LastGeneratedCode = value;
        }
        private string _LastGeneratedCode;
        public Assembly GeneratedAssembly;
        public string SafeClassName;

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
        /// Internally fix ups for templates
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        protected string FixupTemplate(string template)
        {
            // @model fixup
            if (template.Contains("@model "))
            {
                var at = template.IndexOf("@model ");
                var at2 = template.IndexOf("\n", at);
                var line = template.Substring(at, at2 - at);
                var modelClass = line.Replace("@model ", "").Trim();
                var templateType = BaseClassType;
                var newline = "@inherits " +
                              (!string.IsNullOrEmpty(templateType.Namespace) ? templateType.Namespace + "." : "") +
                              templateType.Name + "<" + modelClass + ">";
                template = template.Replace(line, newline);
            }

            return template;
        }



        /// <summary>
        /// Returns the text with a prefix of line numbers
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">Line format used to create the line. 0 is the line number, 1 is the text.</param>
        /// <returns></returns>
        public string GetTextWithLineNumbers(string text, string lineFormat = "{0}.  {1}")
        {
            if (string.IsNullOrEmpty(text)) return text;

            var sb = new StringBuilder();
            var lines = GetLines(text);

            var width = 2;
            if (lines.Length > 9999)
                width = 5;
            else if (lines.Length > 999)
                width = 4;
            else if (lines.Length > 99)
                width = 3;
            else if (lines.Length < 10)
                width = 1;

            lineFormat += "\r\n";
            for (var index = 1; index <= lines.Length; index++)
            {
                var lineNum = index.ToString().PadLeft(width, ' ');
                sb.AppendFormat(lineFormat, lineNum, lines[index - 1]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        public string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null) return null;
            s = s.Replace("\r\n", "\n");
            return maxLines < 1 ? s.Split(new char[] { '\n' }) : s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }

        /// <summary>
        /// Parses and compiles a markup template into an assembly and returns
        /// an assembly name. The name is an ID that can be passed to 
        /// ExecuteTemplateByAssembly which picks up a cached instance of the
        /// loaded assembly.
        /// 
        /// </summary>
        /// <param name="templateReader">Textreader that loads the template</param>
        /// <param name="generatedClassName">The name of the class to generate from the template. null generates name.</param>
        /// <param name="generatedNamespace">The namespace of the class to generate from the template. null generates name.</param>
        /// <remarks>
        /// The actual assembly isn't returned here to allow for cross-AppDomain
        /// operation. If the assembly was returned it would fail for cross-AppDomain
        /// calls.
        /// </remarks>
        /// <returns>An assembly Id. The Assembly is cached in memory and can be used with RenderFromAssembly.</returns>
        public string CompileTemplate(TextReader templateReader,
            string generatedClassName,
            string generatedNamespace = null)
        {
            if (string.IsNullOrEmpty(generatedNamespace)) generatedNamespace = "__RazorHost";

            //generatedClassName = GetSafeClassName(string.IsNullOrEmpty(generatedClassName) ? null : generatedClassName);

            var engine = CreateHost(generatedNamespace, generatedClassName);

            var template = templateReader.ReadToEnd();

            templateReader.Close();

            template = FixupTemplate(template);

            var reader = new StringReader(template);

            // Generate the template class as CodeDom  
            var razorResults = engine.GenerateCode(reader);

            reader.Close();



            var options = new CodeGeneratorOptions();

            // Capture Code Generated as a string for error info
            // and debugging
            LastGeneratedCode = null;
            using (var writer = new StringWriter())
            {
                CodeProvider.GenerateCodeFromCompileUnit(razorResults.GeneratedCode, writer, options);
                LastGeneratedCode = writer.ToString();
            }

            var compilerParameters = new CompilerParameters(ReferencedAssemblies.ToArray());
            //compilerParameters.IncludeDebugInformation = true;           
            compilerParameters.GenerateInMemory = true;


            if (!compilerParameters.GenerateInMemory)
                compilerParameters.OutputAssembly = Path.Combine(Path.GetDirectoryName(TemplateFullPath), /*"_" + Guid.NewGuid().ToString("n")*/ generatedClassName + ".dll");

            var compilerResults = CodeProvider.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
            if (compilerResults.Errors.Count > 0)
            {
                var compileErrors = new StringBuilder();
                foreach (CompilerError compileError in compilerResults.Errors)
                    compileErrors.AppendLine(String.Format("Line: {0}, Column: {1}, Error: {2}",
                                        compileError.Line,
                                        compileError.Column,
                                        compileError.ErrorText));

                ErrorMessage = compileErrors.ToString();
                return null;
            }

            AssemblyCache.Add(compilerResults.CompiledAssembly.FullName, compilerResults.CompiledAssembly);

            return compilerResults.CompiledAssembly.FullName;
        }

        /// <summary>
        /// Creates an instance of the RazorHost with various options applied.
        /// Applies basic namespace imports and the name of the class to generate
        /// </summary>
        /// <param name="generatedNamespace"></param>
        /// <param name="generatedClass"></param>
        /// <param name="baseClassType"></param>
        /// <returns></returns>
        protected RazorTemplateEngine CreateHost(string generatedNamespace, string generatedClass)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = BaseClassType.FullName;
            host.DefaultClassName = generatedClass;
            host.DefaultNamespace = generatedNamespace;

            var context = new GeneratedClassContext("Execute", "Write", "WriteLiteral", "WriteTo", "WriteLiteralTo", typeof(HelperResultPOC).FullName, "DefineSection");
            context.ResolveUrlMethodName = "ResolveUrl";

            host.GeneratedClassContext = context;

            foreach (var ns in ReferencedNamespaces)
            {
                host.NamespaceImports.Add(ns);
            }

            return new RazorTemplateEngine(host);
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

            AppDomainSetup setup = new AppDomainSetup();

            // *** Point at current directory
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain localDomain = AppDomain.CreateDomain(appDomainName, null, setup);

            // *** Need a custom resolver so we can load assembly from non current path
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            return localDomain;
        }

    }

    //public class RazorEngineProxy : MarshalByRefObject
    //{
    //    public string CompileAndRun(string cshtml, object model)
    //    {
    //        try
    //        {
    //            // Compile and run the Razor template
    //            string result = Engine.Razor.RunCompile(cshtml, Guid.NewGuid().ToString(), null, model);
    //            return result;
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle exceptions - you might want to log these or pass them back to the caller
    //            return ex.Message;
    //        }
    //    }
    //}

    /// <summary>
    /// Helped class used by Razor to render generated code.
    /// </summary>
    public class HelperResultPOC : IHtmlString
    {
        private readonly Action<TextWriter> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperResultPOC"/> class,
        /// with the provided <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The action that should be used to produce the result.</param>
        public HelperResultPOC(Action<TextWriter> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;
        }

        /// <summary>
        /// Returns a HTML formatted <see cref="string"/> that represents the current <see cref="HelperResultPOC"/>.
        /// </summary>
        /// <returns>A HTML formatted <see cref="string"/> that represents the current <see cref="HelperResultPOC"/>.</returns>
        public string ToHtmlString()
        {
            return this.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="HelperResultPOC"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="HelperResultPOC"/>.</returns>
        public override string ToString()
        {
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.action(stringWriter);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Writes the output of the <see cref="HelperResultPOC"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> instance that the output should be written to.</param>
        public void WriteTo(TextWriter writer)
        {
            this.action(writer);
        }


        /// <summary>
        /// Writes the output of the <see cref="HelperResultPOC"/> to the provided <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> instance that the output should be written to.</param>
        /// <param name="val"></param>
        public void WriteTo(TextWriter writer, object val)
        {
            writer.Write(val);
        }
    }
}