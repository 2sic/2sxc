using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Razor;
using System.Web.Razor.Generator;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service responsible for compiling Razor templates (.cshtml) into assemblies.
/// </summary>
public class RazorCompilerService(
    MemoryCacheService memoryCacheService,
    IAssemblyDiskCacheService diskCacheService)
    : ServiceBase("Dnn.RzrCmpSvc", connect: [memoryCacheService, diskCacheService])
{
    private const string FallbackBaseClass = "System.Web.WebPages.WebPageBase";
    private const string CSharpCodeProviderCacheKey = "Sxc-Dnn-CSharpCodeProvider";
    private const int CSharpCodeProviderCacheMinutes = 5;

    internal const string DefaultNamespace = "RazorHost";

    /// <summary>
    /// Compiles Razor source code into an assembly.
    /// </summary>
    public (Assembly Assembly, List<CompilerError> Errors) Compile(
        string sourceCode, 
        List<string> referencedAssemblies, 
        string className, 
        string sourceFileName, 
        string outputAssemblyPath = null)
    {
        var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"sourceCode: {sourceCode.Length} chars");

        var baseClass = FindBaseClass(sourceCode);
        l.A($"Base class: {baseClass}");

        var engine = CreateRazorTemplateEngine(className, baseClass, DefaultNamespace);

        // Generate C# code from Razor template
        var lTimer = Log.Fn("Generate Code", timer: true);
        using var reader = new StringReader(sourceCode);
        var razorResults = engine.GenerateCode(reader, className, DefaultNamespace, sourceFileName);
        lTimer.Done();

        // Compile the template into an assembly
        var compiler = GetCSharpCodeProvider();
        lTimer = Log.Fn("Compile", timer: true);
        var compilerParameters = CreateCompilerParameters(referencedAssemblies, outputAssemblyPath);
        var compilerResults = compiler.CompileAssemblyFromDom(compilerParameters, razorResults.GeneratedCode);
        lTimer.Done();

        if (compilerResults.Errors.Count <= 0)
            return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

        var errorList = compilerResults.Errors.Cast<CompilerError>().Where(e => !e.IsWarning).ToList();

        return (!errorList.Any())
            ? l.ReturnAsOk((compilerResults.CompiledAssembly, null))
            : l.ReturnAsError((null, errorList), "error");
    }

    private string FindBaseClass(string template)
    {
        var l = Log.Fn<string>($"template: {template.Length} chars");
        
        try
        {
            var inheritsMatch = Regex.Match(template, @"@inherits\s+(?<BaseName>[\w\.]+)", RegexOptions.Multiline);

            if (!inheritsMatch.Success)
                return l.Return(FallbackBaseClass, $"no @inherits found, fallback to '{FallbackBaseClass}'");

            var baseClass = inheritsMatch.Groups["BaseName"].Value;
            if (baseClass.IsEmptyOrWs())
                return l.Return(FallbackBaseClass, $"@inherits empty string, fallback to '{FallbackBaseClass}'");

            return l.ReturnAsOk(baseClass);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(FallbackBaseClass, "error");
        }
    }

    private RazorTemplateEngine CreateRazorTemplateEngine(string className, string baseClass, string defaultNamespace)
    {
        var l = Log.Fn<RazorTemplateEngine>($"className: '{className}'; baseClass: '{baseClass}'", timer: true);

        var host = new RazorEngineHost(new CSharpRazorCodeLanguage())
        {
            DefaultBaseClass = baseClass,
            DefaultClassName = className,
            DefaultNamespace = defaultNamespace
        };

        var context = new GeneratedClassContext(
            "Execute",
            "Write",
            "WriteLiteral",
            "WriteTo",
            "WriteLiteralTo",
            typeof(HelperResult).FullName,
            "DefineSection")
        {
            ResolveUrlMethodName = "ResolveUrl"
        };

        host.GeneratedClassContext = context;

        // add implicit usings
        foreach (var ns in ImplicitUsings.ForRazor) 
            host.NamespaceImports.Add(ns);

        var engine = new RazorTemplateEngine(host);
        return l.ReturnAsOk(engine);
    }

    private CSharpCodeProvider GetCSharpCodeProvider()
    {
        var l = Log.Fn<CSharpCodeProvider>(timer: true);
        
        if (memoryCacheService.TryGet<CSharpCodeProvider>(CSharpCodeProviderCacheKey, out var fromCache))
            return l.Return(fromCache, "from cached");

        var codeProvider = new CSharpCodeProvider();
        memoryCacheService.Set(CSharpCodeProviderCacheKey, codeProvider, p => p.SetSlidingExpiration(CSharpCodeProviderCacheMinutes * 60));

        return l.Return(codeProvider, "created new and cached");
    }

    private CompilerParameters CreateCompilerParameters(List<string> referencedAssemblies, string outputAssemblyPath)
    {
        var compilerParameters = new CompilerParameters([.. referencedAssemblies])
        {
            GenerateInMemory = !diskCacheService.IsEnabled() && string.IsNullOrEmpty(outputAssemblyPath),
            IncludeDebugInformation = true,
            TreatWarningsAsErrors = false,
            CompilerOptions = DnnRoslynConstants.CompilerOptions,
        };

        if (diskCacheService.IsEnabled() && !string.IsNullOrEmpty(outputAssemblyPath))
        {
            var outDir = Path.GetDirectoryName(outputAssemblyPath);
            if (outDir.HasValue()) 
                Directory.CreateDirectory(outDir);
            
            compilerParameters.OutputAssembly = outputAssemblyPath;
            compilerParameters.TempFiles = new TempFileCollection(EnsureTempDir());
        }

        return compilerParameters;
    }

    private string EnsureTempDir()
    {
        var cacheRoot = diskCacheService.GetCacheDirectoryPath();
        var tempDir = Path.Combine(cacheRoot, "temp");
        if (!Directory.Exists(tempDir))
            Directory.CreateDirectory(tempDir);
        return tempDir;
    }
}
