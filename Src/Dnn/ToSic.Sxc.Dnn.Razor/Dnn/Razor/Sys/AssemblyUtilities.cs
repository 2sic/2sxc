using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Helper utilities for working with assemblies and class names.
/// </summary>
[PrivateApi]
public class AssemblyUtilities() : ServiceBase("Dnn.RzAmUt")
{
    internal const string DefaultNamespace = "RazorHost";

    /// <summary>
    /// Find the main type in a generated assembly.
    /// </summary>
    internal Type FindMainType(Assembly generatedAssembly, string className, bool isCshtml)
    {
        var l = Log.Fn<Type>($"className: '{className}'; isCshtml: {isCshtml}", timer: true);
        
        if (generatedAssembly == null) 
            return l.ReturnAsError(null, "generatedAssembly is null");

        var mainType = generatedAssembly.GetType(
            isCshtml ? $"{DefaultNamespace}.{className}" : className, 
            false, 
            true);
        
        if (mainType != null) 
            return l.ReturnAsOk(mainType);

        l.A("can't find MainType in standard way, fallback #1 - search by classname, ignoring namespace");
        foreach (var mainTypeFallback1 in generatedAssembly.GetTypes())
            if (mainTypeFallback1.Name.Equals(className, StringComparison.OrdinalIgnoreCase))
                return l.ReturnAsOk(mainTypeFallback1);

        l.A("can't find mainTypeFallback1, fallback #2 - just return first type");
        var mainTypeFallback2 = generatedAssembly.GetTypes().FirstOrDefault();
        return l.ReturnAsOk(mainTypeFallback2);
    }

    /// <summary>
    /// Generate a safe class name from a template file path.
    /// </summary>
    internal string GetSafeClassName(string templateFullPath)
    {
        if (!string.IsNullOrWhiteSpace(templateFullPath))
            return "RazorView" + GetSafeString(Path.GetFileNameWithoutExtension(templateFullPath));

        return "RazorView" + Guid.NewGuid().ToString("N");
    }

    private string GetSafeString(string input)
    {
        var safeChars = input.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray();
        var safeString = new string(safeChars);

        if (!char.IsLetter(safeString.FirstOrDefault()) && safeString.FirstOrDefault() != '_')
            safeString = "_" + safeString;

        return safeString;
    }

    /// <summary>
    /// Format compiler errors into a readable error message.
    /// </summary>
    internal string FormatCompilerErrors(List<CompilerError> errors)
    {
        var compileErrors = new StringBuilder();
        foreach (var error in errors)
            compileErrors.AppendLine($"Line: {error.Line}, Column: {error.Column}, Error: {error.ErrorText}");
        return compileErrors.ToString();
    }
}
