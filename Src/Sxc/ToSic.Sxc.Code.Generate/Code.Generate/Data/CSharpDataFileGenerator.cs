using System.Text;
using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class CSharpDataFileGenerator(CSharpModelsGeneratorBase setGenerator, ILog parentLog): HelperBase(parentLog, "Gen.CSMHlp")
{
    /// <summary>
    /// Generates a file model using a helper generator (for different types of target code).
    /// </summary>
    /// <param name="singleGenerator"></param>
    /// <param name="specs"></param>
    /// <returns></returns>
    internal GeneratedDataModel? Generate(ICSharpCodeFragmentGenerator singleGenerator, CSharpFileSpecs specs)
    {
        var fileSetSpecs = specs.FileSetSpecs;
        var setCodeGenHelper = setGenerator.CodeGenHelper;
        var finalClassName = specs.ClassName;
        var l = Log.Fn<GeneratedDataModel>($"ClassName: {finalClassName}; {nameof(specs.Type)}: {specs.Type?.Name} ({specs.Type?.NameId})");

        if (specs.Type == null)
            return l.ReturnNull("No content type provided");

        // Generate main partial class with optional suffix
        // TODO: WHY are we using 2 prefixes here?
        var autoGenClassName = $"{fileSetSpecs.BaseClassPrefix}{specs.ClassName}{fileSetSpecs.BaseClassSuffix}";
        var mainClass = setCodeGenHelper.ClassWrapper(finalClassName, false, true, fileSetSpecs.NamespaceAutoGen + "." + autoGenClassName);

        // Generate AutoGen class with properties
        var classAutoGen = setCodeGenHelper.ClassWrapper(autoGenClassName, true, false, fileSetSpecs.DataInherits);
        var (_, propsSb, usings, firstProperty) = GenerateProperties(specs);
        var classCode = classAutoGen.ToString(propsSb);

        var autoGenClass =
            singleGenerator.GenerateAutoGenClassComment(specs)
            + classCode;

        var fullBody =
            singleGenerator.GenerateMainClassComment(specs, firstProperty)
            + mainClass;

        var fileContents =
            setCodeGenHelper.GenerateUsings(usings)
            + setCodeGenHelper.NamespaceWrapper(fileSetSpecs.DataNamespace)
                .ToString(fullBody)
            + "\n\n"
            + setCodeGenHelper.NamespaceWrapper(fileSetSpecs.BaseClassNamespace)
                .ToString(autoGenClass);

        return l.Return(new($"{finalClassName}{fileSetSpecs.FileGeneratedSuffix}", fileContents, singleGenerator.GenerateFileIntroComment(specs)), $"File size: {fileContents.Length}");
    }

    /// <summary>
    /// Generate all the properties, based on the attributes of the type.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    private (bool HasProps, string? Code, List<string>? Usings, string? FirstProperty) GenerateProperties(CSharpFileSpecs specs)
    {
        var attributes = specs.Type.Attributes.ToList();
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
                Generators = GenDataProperties.Generators(setGenerator.CodeGenHelper)
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

}
