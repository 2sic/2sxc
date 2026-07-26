using ToSic.Eav.Data.ContentTypes.Fields.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyEntity(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper, "Gen.Entity")
{
    public override ValueTypes ForDataType => ValueTypes.Entity;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;
        var l = Log.Fn<List<CodeFragment>>($"name: {name}");

        var inspector = new WorkFieldEntityInspectType();
        this.ConnectLogs([inspector]);
        var entityType = inspector.PrimaryTypeName(attribute, modeCreate: false, tryOtherModes: true);
        //if (entityType.IsEmpty())
        //{
        //    entityType = inspector.PrimaryTypeName(attribute, modeCreate: true);
        //    l.A($"Entity type was empty, will try for create resulting in: '{entityType}'");
        //}
        //var entityType = attribute.Metadata.Get<string>(AttributeNames.EntityFieldType);
        var allowMulti = attribute.Metadata.Get<bool>(AttributeNames.EntityFieldAllowMulti);

        l.A($"entityType: {entityType}, allowMulti: {allowMulti}");

        var msgPrefix = $"{name} as {(allowMulti ? "list" : "single item")} of";
        // var msgSuffix = "Use methods such as .Children(\"{name}\") or .Child(\"{name}\") to get the actual items.";
        var method = allowMulti ? "Children" : "Child";
        var result = allowMulti ? "IEnumerable<{0}>" : "{0}";
        var resultType = nameof(ITypedItem);
        var usings = UsingTypedItems;

        var msgReturns = allowMulti
            ? "An IEnumerable of specified type, but can be empty."
            : "A single item OR null if nothing found, so you can use ?? to provide alternate objects.";

        var msgRemarks = allowMulti
            ? "Generated to return child-list child because field settings had Multi-Value=true. "
            : "Generated to only return 1 child because field settings had Multi-Value=false. ";

        // If we know the entity type, we can use the actual type instead of ITypedItem
        if (entityType.HasValue())
            Specs.ExportedContentContentTypes
                .FirstOrDefault(t => t.Is(entityType))
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

        return l.Return(
        [
            GenPropSnip(tabs, string.Format(result, resultType), name, $"{Specs.ItemAccessor}." + method, cache: true,
                usings: usings,
                summary: [$"{msgPrefix} {resultType}."],
                remarks: [msgRemarks],
                returns: [msgReturns]
            ),
        ]);
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