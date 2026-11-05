using System.Text;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

///// <summary>
///// Abstract base class for C# model generators (both DataModel and CustomModel)
///// Contains common logic for generating model classes with properties
///// </summary>
//internal abstract class CSharpModelGeneratorBase(CSharpModelsGeneratorBase multiGenerator, /*IContentType type, string baseName,*/ ILog parentLog, string logName) 
//    : HelperBase(parentLog, logName)
//{
    //protected readonly CSharpModelsGeneratorBase Generator = multiGenerator;
    //protected readonly IContentType Type = type;

    /// <summary>
    /// The "true" name such as "Tag" without prefix/suffix
    /// </summary>
    //protected readonly string BaseName = baseName;

    //protected string ClassName => $"{FileSetSpecs.Prefix}{BaseName}{ClassSuffix}";

    //internal CSharpCodeSpecs FileSetSpecs => multiGenerator.Specs;
    //protected CSharpGeneratorHelper CodeGenHelper => multiGenerator.CodeGenHelper;

    //#region Abstract Members

    ///// <summary>
    ///// Gets the class suffix to append to the main class name (e.g., "Model" for CustomModel)
    ///// </summary>
    //protected abstract string ClassSuffix { get; }

    ///// <summary>
    ///// Generates the file introduction comment with extension instructions
    ///// </summary>
    ///// <param name="userName">The name of the user generating the file</param>
    ///// <returns>The file introduction comment</returns>
    //protected abstract string GenerateFileIntroComment(string userName);

    ///// <summary>
    ///// Generates the main class XML documentation comment
    ///// </summary>
    ///// <param name="firstPropertyName">The name of the first property for examples</param>
    ///// <returns>The main class comment</returns>
    //protected abstract string GenerateMainClassComment(string? firstPropertyName);

    ///// <summary>
    ///// Generates the auto-generated base class XML documentation comment
    ///// </summary>
    ///// <returns>The auto-generated class comment</returns>
    //protected abstract string GenerateAutoGenClassComment();

//    #endregion

//}

internal class CSharpModelFileGenerator(CSharpModelsGeneratorBase multiGenerator, ILog parentLog): HelperBase(parentLog, "Gen.CSMHlp")
{
    private CSharpGeneratorHelper CodeGenHelper => multiGenerator.CodeGenHelper;

    #region Common Implementation

    internal GeneratedDataModel? PrepareFile(IGenCodeFragments singleGenerator, CodeFragmentSpecs specs)
    {
        var fileSetSpecs = specs.FileSetSpecs;
        var finalClassName = specs.ClassName;
        var l = Log.Fn<GeneratedDataModel>($"ClassName: {finalClassName}; {nameof(specs.Type)}: {specs.Type?.Name} ({specs.Type?.NameId})");

        if (specs.Type == null)
            return l.ReturnNull("No content type provided");

        // Generate main partial class with optional suffix
        // TODO: WHY are we using 2 prefixes here?
        var autoGenClassName = $"{fileSetSpecs.BaseClassPrefix}{specs.ClassName}{fileSetSpecs.BaseClassSuffix}";
        var mainClass = CodeGenHelper.ClassWrapper(finalClassName, false, true, fileSetSpecs.NamespaceAutoGen + "." + autoGenClassName);

        // Generate AutoGen class with properties
        var classAutoGen = CodeGenHelper.ClassWrapper(autoGenClassName, true, false, fileSetSpecs.DataInherits);
        var (_, propsSb, usings, firstProperty) = ClassProperties(specs, [..specs.Type.Attributes]);
        var classCode = classAutoGen.ToString(propsSb);

        var autoGenClass =
            singleGenerator.GenerateAutoGenClassComment(specs)
            + classCode;

        var fullBody =
            singleGenerator.GenerateMainClassComment(specs, firstProperty)
            + mainClass;

        var fileContents =
            CodeGenHelper.GenerateUsings(usings)
            + CodeGenHelper.NamespaceWrapper(fileSetSpecs.DataNamespace)
                .ToString(fullBody)
            + "\n\n"
            + CodeGenHelper.NamespaceWrapper(fileSetSpecs.BaseClassNamespace)
                .ToString(autoGenClass);

        return l.Return(new($"{finalClassName}{fileSetSpecs.FileGeneratedSuffix}", fileContents, singleGenerator.GenerateFileIntroComment(specs)), $"File size: {fileContents.Length}");
    }

    private (bool HasProps, string? Code, List<string>? Usings, string? FirstProperty) ClassProperties(CodeFragmentSpecs specs, List<IContentTypeField> attributes)
    {
        var l = Log.Fn<(bool, string?, List<string>?, string?)>($"{nameof(attributes)}: {attributes.Count}");

        // Ephemeral fields only exist in the edit UI and are not persisted.
        // Generated properties would access missing data and fail during serialization.
        var persistedAttributes = attributes
            .Where(a => !a.IsEphemeral())
            .ToList();

        // Generate all properties with the helpers
        var propsSnippets = persistedAttributes
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
                    .SelectMany(p => p.Generate(set.Attribute, specs.FileSetSpecs.TabsProperty));
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

}

/// <summary>
/// 
/// </summary>
/// <param name="UserName">Gets the username for file generation</param>
internal record CodeFragmentSpecs(
    CSharpCodeSpecs FileSetSpecs,
    IContentType Type,
    string UserName,
    string BaseName
)
{
    public string ClassName => $"{FileSetSpecs.Prefix}{BaseName}{ClassSuffix}";

    /// <summary>
    /// Gets the class suffix to append to the main class name (e.g., "Model" for CustomModel)
    /// </summary>
    public string ClassSuffix => FileSetSpecs.Suffix ?? "";
}

internal interface IGenCodeFragments
{
    /// <summary>
    /// Generates the file introduction comment with extension instructions
    /// </summary>
    /// <param name="fileSpecs">The specifications for the file being generated</param>
    /// <returns>The file introduction comment</returns>
    string GenerateFileIntroComment(CodeFragmentSpecs fileSpecs);

    /// <summary>
    /// Generates the main class XML documentation comment
    /// </summary>
    /// <param name="fileSpecs">The specifications for the file being generated</param>
    /// <param name="firstPropertyName">The name of the first property for examples</param>
    /// <returns>The main class comment</returns>
    string GenerateMainClassComment(CodeFragmentSpecs fileSpecs, string? firstPropertyName);

    /// <summary>
    /// Generates the auto-generated base class XML documentation comment
    /// </summary>
    /// <returns>The auto-generated class comment</returns>
    string GenerateAutoGenClassComment(CodeFragmentSpecs fileSpecs);

}