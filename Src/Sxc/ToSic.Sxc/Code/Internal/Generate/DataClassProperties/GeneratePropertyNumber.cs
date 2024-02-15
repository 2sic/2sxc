namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyNumber: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Number;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return [
            GenPropSnip(tabs, "int", name, "Int", summary:
            [
                $"Get the int of {name}.",
                $"To get other types use methods such as .Decimal(\"{name}\")"
            ]),
        ];
    }
}