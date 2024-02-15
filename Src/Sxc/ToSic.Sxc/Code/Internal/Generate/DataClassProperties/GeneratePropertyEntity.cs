namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyEntity: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Entity;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "IEnumerable<ITypedItem>", name, "Children", usings: UsingTypedItems, summary:
            [
                $"{name} as list of ITypedItem.",
            ]),
        ];
    }

    private List<string> UsingTypedItems { get; } =
    [
        "System.Collections.Generic",
        "ToSic.Sxc.Data"
    ];

}