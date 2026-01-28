using System.Text;
using ToSic.Eav.Data.Sys;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// Abstract base class for C# model generators (both DataModel and CustomModel)
/// Contains common logic for generating model classes with properties
/// </summary>
internal abstract class CSharpModelGeneratorBase(CSharpModelsGeneratorBase generator, IContentType type, string baseName, ILog parentLog, string logName) 
    : HelperBase(parentLog, logName)
{
    protected readonly CSharpModelsGeneratorBase Generator = generator;
    protected readonly IContentType Type = type;

    /// <summary>
    /// The "true" name such as "Tag" without prefix/suffix
    /// </summary>
    protected readonly string BaseName = baseName;

    ///// <summary>
    ///// Prefix to use for the class name
    ///// </summary>
    //protected string Prefix => Specs.Prefix ?? "";

    ///// <summary>
    ///// Gets the suffix associated with the class.
    ///// </summary>
    //protected string Suffix => ClassSuffix;

    protected string ClassName => $"{Specs.Prefix}{BaseName}{ClassSuffix}";

    internal CSharpCodeSpecs Specs => Generator.Specs;
    protected CSharpGeneratorHelper CodeGenHelper => Generator.CodeGenHelper;

    #region Abstract Members

    /// <summary>
    /// Gets the username for file generation
    /// </summary>
    protected string UserName => Generator.User.Name;

    /// <summary>
    /// Gets the class suffix to append to the main class name (e.g., "Model" for CustomModel)
    /// </summary>
    protected abstract string ClassSuffix { get; }

    /// <summary>
    /// Generates the file introduction comment with extension instructions
    /// </summary>
    /// <param name="userName">The name of the user generating the file</param>
    /// <returns>The file introduction comment</returns>
    protected abstract string GenerateFileIntroComment(string userName);

    /// <summary>
    /// Generates the main class XML documentation comment
    /// </summary>
    /// <param name="firstPropertyName">The name of the first property for examples</param>
    /// <returns>The main class comment</returns>
    protected abstract string GenerateMainClassComment(string? firstPropertyName);

    /// <summary>
    /// Generates the auto-generated base class XML documentation comment
    /// </summary>
    /// <returns>The auto-generated class comment</returns>
    protected abstract string GenerateAutoGenClassComment();

    #endregion

    #region Common Implementation

    internal GeneratedDataModel? PrepareFile()
    {
        var finalClassName = ClassName;
        var l = Log.Fn<GeneratedDataModel>($"ClassName: {finalClassName}; {nameof(Type)}: {Type?.Name} ({Type?.NameId})");

        if (Type == null)
            return l.ReturnNull("No content type provided");

        // Generate main partial class with optional suffix
        // TODO: WHY are we using 2 prefixes here?
        var autoGenClassName = $"{Specs.BaseClassPrefix}{ClassName}{Specs.BaseClassSuffix}";
        var mainClass = CodeGenHelper.ClassWrapper(finalClassName, false, true, Specs.NamespaceAutoGen + "." + autoGenClassName);

        // Generate AutoGen class with properties
        var classAutoGen = CodeGenHelper.ClassWrapper(autoGenClassName, true, false, Specs.DataInherits);
        var (_, propsSb, usings, firstProperty) = ClassProperties(Type.Attributes.ToList());
        var classCode = classAutoGen.ToString(propsSb);

        var autoGenClass =
            GenerateAutoGenClassComment()
            + classCode;

        var fullBody =
            GenerateMainClassComment(firstProperty)
            + mainClass;

        var fileContents =
            CodeGenHelper.GenerateUsings(usings)
            + CodeGenHelper.NamespaceWrapper(Specs.DataNamespace)
                .ToString(fullBody)
            + "\n\n"
            + CodeGenHelper.NamespaceWrapper(Specs.BaseClassNamespace)
                .ToString(autoGenClass);

        return l.Return(new($"{finalClassName}{Specs.FileGeneratedSuffix}", fileContents, GenerateFileIntroComment(UserName)), $"File size: {fileContents.Length}");
    }

    private (bool HasProps, string? Code, List<string>? Usings, string? FirstProperty) ClassProperties(List<IContentTypeAttribute> attributes)
    {
        var l = Log.Fn<(bool, string?, List<string>?, string?)>($"{nameof(attributes)}: {attributes.Count}");

        // Generate all properties with the helpers
        var propsSnippets = attributes
            .Select(a => new
            {
                Attribute = a,
                Generators = GenDataProperties.Generators(CodeGenHelper)
                    .Where(p => p.ForDataType == a.Type)
                    .ToList()
            })
            .Where(a => a.Generators.Any())
            .SelectMany(set =>
            {
                return set.Generators
                    .SelectMany(p => p.Generate(set.Attribute, Specs.TabsProperty));
            })
            .ToList();

        if (!propsSnippets.Any())
            return l.Return((false, null, null, null), "no snippets found");

        // Detect duplicate names as this would fail
        // If we have duplicates, keep the first with a real priority
        var deduplicated = propsSnippets
            .GroupBy(ps => ps.NameId)
            .SelectMany(g => g.OrderBy(ps => ps.Priority ? 0 : 1).Take(1))
            .OrderBy(ps => ps.NameId)
            .ToList();

        var sb = new StringBuilder();
        foreach (var genCode in deduplicated)
            sb.AppendLine(genCode.ToString());

        var usings = deduplicated
            .SelectMany(ps => ps.Usings)
            .Distinct()
            .OrderBy(u => u)
            .ToList();

        l.A($"Snippets: {propsSnippets.Count}; Deduplicated: {deduplicated.Count}; Usings: {usings.Count}");
        var properties = sb.ToString();

        return l.Return((true, properties, usings, deduplicated.First().NameId), $"props string len: {properties.Length}");
    }

    #endregion

    #region Helper Methods for Derived Classes

    protected string? GenerateCommonScopeRemarks()
    {
        var scope = Type.Scope;
        var scopeIsSpecial = scope != ScopeConstants.Default;
        return scopeIsSpecial ? $"This Content-Type is NOT in the default scope, so you may not see it in the Admin UI. It's in the scope {scope}." : null;
    }

    protected string GetScopeDescription()
    {
        var scope = Type.Scope;
        var scopeIsSpecial = scope != ScopeConstants.Default;
        return scopeIsSpecial ? $"(scope: {scope})" : "";
    }

    #endregion
}
