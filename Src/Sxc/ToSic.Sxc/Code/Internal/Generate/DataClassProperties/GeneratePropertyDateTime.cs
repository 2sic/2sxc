namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyDateTime: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.DateTime;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "DateTime", name, "DateTime", summary:
            [
                $"Get the DateTime of {name}.",
            ]),
        ];
    }
}