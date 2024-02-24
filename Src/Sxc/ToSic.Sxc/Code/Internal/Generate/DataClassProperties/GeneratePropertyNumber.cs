namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyNumber: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Number;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        var decimals = attribute.Metadata.GetBestValue<int>("Decimals");
        
        return decimals == 0
            ?
            [
                GenPropSnip(tabs, "int", name, $"{ItemAccessor}.Int", summary:
                [
                    $"{name} as int. <br/>",
                    $"To get other types use methods such as .Decimal(\"{name}\")"
                ]),
            ]
            :
            [
                GenPropSnip(tabs, "decimal", name, $"{ItemAccessor}.Decimal", summary:
                [
                    $"{name} as decimal. <br/>",
                    $"To get other types use methods such as .Int(\"{name}\")"
                ]),
            ];
    }
}