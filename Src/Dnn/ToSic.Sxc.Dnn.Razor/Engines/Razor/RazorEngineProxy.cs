using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using System.Runtime.Remoting;
using System.Text;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.WebPages;
using Microsoft.CSharp;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    public class RazorEngineProxy : MarshalByRefObject
    {
        private string AppCodeFullPath { get; set; }
        private string TemplateFullPath { get; set; }
        private string TemplatePath { get; set; }
        private Purpose Purpose { get; set; }

        public delegate void InitHelpersDelegate(RazorComponentBase webpage);

        //private InitHelpersDelegate InitHelpers { get; set; }
        private Type BaseClassType { get; set; }

        private string LastGeneratedCode
        {
            get => GetTextWithLineNumbers(_lastGeneratedCode);
            set => _lastGeneratedCode = value;
        }
        private string _lastGeneratedCode;

        /// <summary>
        /// A list of default namespaces to include
        /// 
        /// Defaults already included:
        /// System, System.Text, System.IO, System.Collections.Generic, System.Linq
        /// </summary>
        private List<string> ReferencedNamespaces { get; set; }

        /// <summary>
        /// A list of default assemblies referenced during compilation
        /// 
        /// Defaults already included:
        /// System, System.Text, System.IO, System.Collections.Generic, System.Linq
        /// </summary>
        private List<string> ReferencedAssemblies { get; set; }

        /// <summary>
        /// The code provider used with this instance
        /// </summary>
        private CSharpCodeProvider CodeProvider { get; set; }

        private string CompiledClassName { get; set; }
        public Type CompiledType { get; private set; }
        private Assembly CompiledAssembly { get; set; }
        private RazorComponentBase Webpage { get; set; }

        /// <summary>
        /// Internally cache assemblies loaded with ParseAndCompileTemplate.        
        /// Assemblies are cached in the EngineHost so they don't have
        /// to cross AppDomains for invocation when running in a separate AppDomain
        /// </summary>
        private Dictionary<string, Assembly> AssemblyCache { get; set; }

        /// <summary>
        /// Any errors that occurred during template execution
        /// </summary>
        public string ErrorMessage { get; private set; }

        public RazorEngineProxy()
        {

        }

        public void Init(string appCodeFullPath, string templateFullPath, string templatePath, Purpose purpose/*, InitHelpersDelegate initHelpers*/, string binDirectory)
        {
            AppCodeFullPath = appCodeFullPath;
            TemplateFullPath = templateFullPath;
            TemplatePath = templatePath;
            Purpose = purpose;
            //InitHelpers = initHelpers;
            BaseClassType = typeof(System.Web.WebPages.WebPageBase/*Custom.Hybrid.RazorTyped*/); // TODO: Read from @inherits OR fallback to ???
            BinDirectory = binDirectory;
            CompiledClassName = GetSafeClassName(TemplateFullPath);
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
            foreach (var dll in Directory.GetFiles(BinDirectory, "*.dll"))
                ReferencedAssemblies.Add(dll);

            var lc = 0;
            var ec = new List<string>();
            foreach (var dll in ReferencedAssemblies)
            {
                try
                {
                    GetAssembly(dll);
                    lc++;
                }
                catch
                {
                    // ignored
                    ec.Add(dll);
                }
            }

            //var x = $"{lc}-{ec.Join(", ")}";

            if (File.Exists(AppCodeFullPath))
            {
                ReferencedAssemblies.Add(AppCodeFullPath);
                GetAssembly(AppCodeFullPath);
            }
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
        private string CompileTemplate(TextReader templateReader, string generatedNamespace = null)
        {
            //if (string.IsNullOrEmpty(generatedNamespace)) generatedNamespace = "__RazorHost";

            //generatedClassName = GetSafeClassName(string.IsNullOrEmpty(generatedClassName) ? null : generatedClassName);

            var engine = CreateHost(CompiledClassName, generatedNamespace);

            var template = templateReader.ReadToEnd();

            templateReader.Close();

            template = FixupTemplate(template);

            var reader = new StringReader(template);

            // Generate the template class as CodeDom  
            var razorResults = engine.GenerateCode(reader);

            reader.Close();

            //// Make the class serializable so we can pass it across AppDomains
            //razorResults.GeneratedCode.Namespaces[0].Types[0].CustomAttributes
            //    .Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));

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

            CodeProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
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
            compilerParameters.GenerateInMemory = true;

            if (!compilerParameters.GenerateInMemory)
                compilerParameters.OutputAssembly = Path.Combine(Path.GetDirectoryName(TemplateFullPath), CompiledClassName + ".dll");

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

            CompiledAssembly = compilerResults.CompiledAssembly;
            AssemblyCache.Add(CompiledAssembly.FullName, CompiledAssembly);

            // find the generated type to instantiate
            CompiledType = CompiledAssembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(CompiledClassName));

            return CompiledAssembly.FullName;
        }

        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]


        /// <summary>
        /// Creates an instance of the RazorHost with various options applied.
        /// Applies basic namespace imports and the name of the class to generate
        /// </summary>
        /// <param name="generatedClass"></param>
        /// <param name="generatedNamespace"></param>
        /// <param name="baseClassType"></param>
        /// <returns></returns>
        private RazorTemplateEngine CreateHost(string generatedClass, string generatedNamespace = null)
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
        private string FixupTemplate(string template)
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
        private string GetTextWithLineNumbers(string text, string lineFormat = "{0}.  {1}")
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
        private string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null) return null;
            s = s.Replace("\r\n", "\n");
            return maxLines < 1 ? s.Split(new char[] { '\n' }) : s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }

        private Assembly GetAssembly(string assemblyPath, string pdbPath = null)
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

        /// <summary>
        /// Returns a unique ClassName for a template to execute
        /// Optionally pass in an objectId on which the code is based
        /// or null to get default behavior.
        /// 
        /// Default implementation just returns Guid as string
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        private string GetSafeClassName(object objectId)
        {
            return "_" + Guid.NewGuid().ToString().Replace("-", "_");
        }

        private string BinDirectory { get; set; }
        private HttpContextBase HttpContextWrapper { get; set; }

        public (string, List<Exception>) RenderTemplate(object data, HttpContextProxy proxyInNewDomain, Func<HttpContextBase> current)
        {
            // Use the proxy object to access HttpContext.Current from the new application domain
            //var context = (HttpContext)proxyInNewDomain.GetCurrentHttpContext();
            proxyInNewDomain.SyncContext();
            //var c = HttpContext.Current;
            //var w = new HttpContextWrapper(c);
            //var z = current.Invoke();
            try
            {
                using (var reader = new StringReader(File.ReadAllText(TemplateFullPath)))
                {
                    var assemblyId = CompileTemplate(reader, null);
                    if (assemblyId == null)
                        throw new Exception(ErrorMessage);
                }
            }
            catch (Exception compileEx)
            {
                // TODO: ADD MORE compile error help
                // 1. Read file
                // 2. Try to find base type - or warn if not found
                // 3. ...
                //var razorType = _sourceAnalyzer.Value.TypeOfVirtualPath(TemplatePath);
                //l.A($"Razor Type: {razorType}");
                //var ex = l.Ex(_errorHelp.Value.AddHelpForCompileProblems(compileEx, razorType));
                // Special form of throw to preserve details about the call stack
                ExceptionDispatchInfo.Capture(compileEx).Throw();
                throw; // fake throw, just so the code shows what happens
            }

            if (CompiledType == null)
                return (null, new List<Exception>()); //  l.ReturnNull("type not found");

            var writer = new StringWriter();
            var result = Render(writer, data);
            return (result.writer.ToString(), result.exception);
        }

        private (TextWriter writer, List<Exception> exception) Render(TextWriter writer, object data)
        {
            var page = InitWebpage(); // access the property once only
            try
            {
                if (data != null && page is ISetDynamicModel setDyn)
                    setDyn.SetDynamicModel(data);
            }
            catch (Exception e)
            {
                // l.Ex("Problem with setting dynamic model, error will be ignored.", e);
            }

            try
            {
                page.ExecutePageHierarchy(new WebPageContext(HttpContextWrapper, page, data), writer, page);
            }
            catch (Exception ex)
            {
                // Special form of throw to preserve details about the call stack
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw; // fake throw, just so the code shows what happens
            }
            return (writer, page.SysHlp.ExceptionsOrNull);
        }

        private RazorComponentBase InitWebpage()
        {
            var objectValue = RuntimeHelpers.GetObjectValue(GetPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException($"The webpage found at '{TemplatePath}' was not created.");

            if (!(objectValue is RazorComponentBase pageToInit))
                throw new InvalidOperationException($"The webpage at '{TemplatePath}' must derive from RazorComponentBase.");
            Webpage = pageToInit;

            pageToInit.Context = HttpContextWrapper;
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
            //InitHelpers(pageToInit); // HACK: stv commented
            //return l.ReturnTrue("ok");
            return Webpage;
        }

        private object GetPageInstance(/*string assemblyId*/)
        {
            //var assembly = GetAssembly(assemblyId);
            //if (assembly == null) return null;

            //var type = assembly.GetTypes().FirstOrDefault(t => t.Name.StartsWith(CompiledClassName));
            //if (type == null) return null;

            var instance = new ObjectHandle(Activator.CreateInstance(CompiledType)).Unwrap();

            //page = new ObjectHandle(Activator.CreateInstance(razorEngineProxy.CompiledType)).Unwrap();
            //page = Activator.CreateInstance(razorEngineProxy.CompiledAssembly.FullName, razorEngineProxy.CompiledClassName).Unwrap();
            //page = CustomAppDomain.CreateInstanceAndUnwrap(razorEngineProxy.CompiledAssembly.FullName, razorEngineProxy.CompiledClassName);
            //CustomAppDomain.CreateInstanceFrom(GeneratedAssembly.Location, $"Razor.{SafeClassName}", false, BindingFlags.Default, null,
            //        new object[] { null }, CultureInfo.CurrentCulture, null)
            //CustomAppDomain.CreateInstanceFromAndUnwrap(GeneratedAssembly.Location, $"Razor.{SafeClassName}", null);



            return instance;
        }
    }
}