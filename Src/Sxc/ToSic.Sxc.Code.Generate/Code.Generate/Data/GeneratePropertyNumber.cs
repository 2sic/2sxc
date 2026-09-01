using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyNumber(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Number;

    public override List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        var decimals = fieldDef.Metadata.Get<int>("Decimals");

        return decimals == 0
            ?
            [
                GenPropSnip(tabs, "int", name, $"{Specs.ItemAccessor}.Int", summary:
                [
                    $"{name} as int. <br/>",
                    $"To get other types use methods such as .Decimal(\"{name}\")"
                ]),
            ]
            :
            [
                GenPropSnip(tabs, "decimal", name, $"{Specs.ItemAccessor}.Decimal", summary:
                [
                    $"{name} as decimal. <br/>",
                    $"To get other types use methods such as .Int(\"{name}\")"
                ]),
            ];
    }
}