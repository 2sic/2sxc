using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyEntity: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Entity;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        var entityType = attribute.Metadata.GetBestValue<string>("EntityType");
        var allowMulti = attribute.Metadata.GetBestValue<bool>("AllowMultiValue");

        var msgPrefix = $"{name} as " + (allowMulti ? "list" : "single item") + "of";
        // var msgSuffix = "Use methods such as .Children(\"{name}\") or .Child(\"{name}\") to get the actual items.";
        var method = allowMulti ? "Children" : "Child";
        var result = allowMulti ? "IEnumerable<{0}>" : "{0}";
        var resultType = nameof(ITypedItem);
        var usings = UsingTypedItems;

        var msgReturns = allowMulti
            ? "Will always return a IEnumerable, but can be empty."
            : "Will always return a single item, but can be null.";

        // ReSharper disable once InvertIf
        if (entityType.HasValue())
            specs.ExportedContentContentTypes
                .FirstOrDefault(t => entityType.EqualsInsensitive(t.Name))
                .UseIfNotNull(ct =>
                {
                    // Switch the result type
                    resultType = ct.Name;
                    // Make the method generic returning the expected type
                    method += $"<{resultType}>";
                    // No usings needed, as the type is already in the namespace
                    usings = null;
                });

        return
        [
            GenPropSnip(tabs, string.Format(result, resultType), name, method, usings: usings, summary:
            [
                $"{msgPrefix} {resultType}.",
            ]),
        ];
    }

    private List<string> UsingTypedItems { get; } =
    [
        "System.Collections.Generic",
        "ToSic.Sxc.Data"
    ];

}