using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages;
using Microsoft.CSharp;
using Microsoft.EntityFrameworkCore.Internal;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
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

                page = /*Activator.CreateInstance(compiledType);*/
                    //LocalAppDomain.CreateInstanceAndUnwrap(GeneratedAssembly.FullName, SafeClassName);
                    //LocalAppDomain.CreateInstanceFrom(GeneratedAssembly.Location, $"Razor.{SafeClassName}", false, BindingFlags.Default, null,
                    //        new object[] { null }, CultureInfo.CurrentCulture, null)
                    LocalAppDomain.CreateInstanceFromAndUnwrap(GeneratedAssembly.Location, $"Razor.{SafeClassName}", null);


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
                DeleteAssembly(GeneratedAssembly.Location);
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

    public class RazorEngineProxy : MarshalByRefObject
    {
        public string AppCodeFullPath { get; }
        public string TemplateFullPath { get; }
        public AppDomain LocalAppDomain { get; }
        public Type BaseClassType;

        public string LastGeneratedCode
        {
            get => GetTextWithLineNumbers(_LastGeneratedCode);
            set => _LastGeneratedCode = value;
        }
        private string _LastGeneratedCode;

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
        /// The code provider used with this instance
        /// </summary>
        internal CSharpCodeProvider CodeProvider { get; set; }



        /// <summary>
        /// Internally cache assemblies loaded with ParseAndCompileTemplate.        
        /// Assemblies are cached in the EngineHost so they don't have
        /// to cross AppDomains for invocation when running in a separate AppDomain
        /// </summary>
        public Dictionary<string, Assembly> AssemblyCache { get; set; }

        /// <summary>
        /// Any errors that occurred during template execution
        /// </summary>
        public string ErrorMessage { get; set; }

        public RazorEngineProxy(string appCodeFullPath, string templateFullPath, AppDomain localAppDomain)
        {
            AppCodeFullPath = appCodeFullPath;
            TemplateFullPath = templateFullPath;
            LocalAppDomain = localAppDomain;
            BaseClassType = typeof(System.Web.WebPages.WebPageBase/*Custom.Hybrid.RazorTyped*/); // TODO: Read from @inherits OR fallback to ???
            CodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

            AssemblyCache = new Dictionary<string, Assembly>();
            ErrorMessage = string.Empty;

            ReferencedNamespaces = new List<string>
            {
                "System",
                "System.Text",
                "System.Collections.Generic",
                "System.Linq",
                "System.IO",
                "System.Web.WebPages"
            };

            ReferencedAssemblies = new List<string>
            {
                "System.dll",
                "System.Core.dll",
                typeof(IHtmlString).Assembly.Location,    // System.Web
                "Microsoft.CSharp.dll"   // dynamic support!
            };

            // reference all assemblies form bin folder
            foreach (var dll in Directory.GetFiles(HttpRuntime.BinDirectory, "*.dll"))
                ReferencedAssemblies.Add(dll);

            var lc = 0;
            var ec = new List<string>();
            foreach (var dll in ReferencedAssemblies)
            {
                try
                {
                    LocalAppDomain.Load(File.ReadAllBytes(dll));
                    lc++;
                }
                catch
                {
                    // ignored
                    ec.Add(dll);
                }
            }

            var x = $"{lc}-{ec.Join(", ")}";

            //if (File.Exists(AppCodeFullPath))
            //{
            //    ReferencedAssemblies.Add(AppCodeFullPath);
            //    // load referenced Assemblies to LocalAppDomain
            //    LocalAppDomain.Load(AssemblyName.GetAssemblyName(AppCodeFullPath));
            //}


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
            //if (string.IsNullOrEmpty(generatedNamespace)) generatedNamespace = "__RazorHost";

            //generatedClassName = GetSafeClassName(string.IsNullOrEmpty(generatedClassName) ? null : generatedClassName);

            var engine = CreateHost(generatedClassName, generatedNamespace);

            var template = templateReader.ReadToEnd();

            templateReader.Close();

            template = FixupTemplate(template);

            var reader = new StringReader(template);

            // Generate the template class as CodeDom  
            var razorResults = engine.GenerateCode(reader);

            reader.Close();

            // Make the class serializable so we can pass it across AppDomains
            razorResults.GeneratedCode.Namespaces[0].Types[0].CustomAttributes
                .Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));

            //// implement ISerializable so we can pass it across AppDomains
            //razorResults.GeneratedCode.Namespaces[0].Types[0].BaseTypes.Add(typeof(ISerializable));

            //// add public void GetObjectData(SerializationInfo info, StreamingContext context)
            //var method = new CodeMemberMethod();
            //method.Name = "GetObjectData";
            //method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //method.ReturnType = new CodeTypeReference(typeof(void));
            //method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(SerializationInfo), "info"));
            //method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(StreamingContext), "context"));
            //method.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "GetObjectData", new CodeExpression[] { new CodeVariableReferenceExpression("info"), new CodeVariableReferenceExpression("context") }));
            //razorResults.GeneratedCode.Namespaces[0].Types[0].Members.Add(method);


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
            compilerParameters.IncludeDebugInformation = true;
            compilerParameters.GenerateInMemory = false;

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
        /// <param name="generatedClass"></param>
        /// <param name="generatedNamespace"></param>
        /// <param name="baseClassType"></param>
        /// <returns></returns>
        protected RazorTemplateEngine CreateHost(string generatedClass, string generatedNamespace = null)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = BaseClassType.FullName;
            host.DefaultClassName = generatedClass;
            if (generatedNamespace.HasValue()) host.DefaultNamespace = generatedNamespace;

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

        public Assembly GetAssembly(string assemblyPath, string pdbPath = null)
        {
            try
            {
                return LoadAssembly(assemblyPath, pdbPath);
            }
            catch (Exception)
            {
                return null;
                // throw new InvalidOperationException(ex);
            }
        }

        private Assembly LoadAssembly(string assemblyPath, string pdbPath = null)
        {
            pdbPath = string.IsNullOrEmpty(pdbPath) ? Path.ChangeExtension(assemblyPath, "pdb") : pdbPath;
            return File.Exists(pdbPath)
                ? Assembly.Load(File.ReadAllBytes(assemblyPath), File.ReadAllBytes(pdbPath))
                : Assembly.Load(File.ReadAllBytes(assemblyPath));
        }
    }

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