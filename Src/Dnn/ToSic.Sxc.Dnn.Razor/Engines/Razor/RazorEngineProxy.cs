using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Razor;
using System.Web.Razor.Generator;
using Microsoft.CSharp;
using Microsoft.EntityFrameworkCore.Internal;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Engines
{
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
                    //LocalAppDomain.Load(File.ReadAllBytes(dll));
                    GetAssembly(dll);
                    lc++;
                }
                catch
                {
                    // ignored
                    ec.Add(dll);
                }
            }

            var x = $"{lc}-{ec.Join(", ")}";

            if (File.Exists(AppCodeFullPath))
            {
                ReferencedAssemblies.Add(AppCodeFullPath);
                // load referenced Assemblies to LocalAppDomain
                //GetAssembly(AppCodeFullPath);
                //Assembly.LoadFrom(AppCodeFullPath);
                //LocalAppDomain.Load(AssemblyName.GetAssemblyName(AppCodeFullPath));
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
                return LoadAssembly2(assemblyPath, pdbPath);
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

        private Assembly LoadAssembly2(string assemblyPath, string pdbPath = null)
        {
            pdbPath = string.IsNullOrEmpty(pdbPath) ? Path.ChangeExtension(assemblyPath, "pdb") : pdbPath;
            return File.Exists(pdbPath)
                ? LocalAppDomain.Load(File.ReadAllBytes(assemblyPath), File.ReadAllBytes(pdbPath))
                : LocalAppDomain.Load(File.ReadAllBytes(assemblyPath));
        }
    }
}