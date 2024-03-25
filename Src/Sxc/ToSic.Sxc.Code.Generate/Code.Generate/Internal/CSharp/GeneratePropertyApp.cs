using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Generate.Internal;

internal class GeneratePropertyApp(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Undefined;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {




        var name = attribute.Name;

        var entityType = attribute.Metadata.GetBestValue<string>("EntityType");
        var allowMulti = attribute.Metadata.GetBestValue<bool>("AllowMultiValue");

        var msgPrefix = $"{name} as " + (allowMulti ? "list" : "single item") + " of";
        // var msgSuffix = "Use methods such as .Children(\"{name}\") or .Child(\"{name}\") to get the actual items.";
        var method = allowMulti ? "Children" : "Child";
        var result = allowMulti ? "IEnumerable<{0}>" : "{0}";
        var resultType = nameof(ITypedItem);
        var usings = UsingTypedItems;

        // TODO: CONSIDER GOING TO lIST - REQUIRES A ToList at the end.

        var msgReturns = allowMulti
            ? "An IEnumerable of specified type, but can be empty."
            : "A single item OR null if nothing found, so you can use ?? to provide alternate objects.";

        var msgRemarks = allowMulti
            ? "Generated to return child-list child because field settings had Multi-Value=true. "
            : "Generated to only return 1 child because field settings had Multi-Value=false. ";

        // If we know the entity type, we can use the actual type instead of ITypedItem
        if (entityType.HasValue())
            Specs.ExportedContentContentTypes
                .FirstOrDefault(t => entityType.EqualsInsensitive(t.Name))
                .DoIfNotNull(ct =>
                {
                    // Switch the result type
                    resultType = ct.Name;
                    // Make the method generic returning the expected type
                    method += $"<{resultType}>";
                    // No usings needed, as the type is already in the namespace
                    usings = allowMulti ? UsingList : [];
                    msgRemarks += $"The type {resultType} was specified in the field settings.";
                });

        return
        [
            GenPropSnip(tabs, string.Format(result, resultType), name, $"{Specs.ItemAccessor}." + method, cache: true, usings: usings,
                summary: ["Typed App with typed Settings & Resources"]
            ),
        ];
    }

    private List<string> UsingList { get; } =
    [
        "System.Collections.Generic",
    ];

    private List<string> UsingTypedItems { get; } =
    [
        "System.Collections.Generic",
        "ToSic.Sxc.Data"
    ];

}