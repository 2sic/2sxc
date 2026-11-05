using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyNumber(CSharpGeneratorHelper helper) : ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.Number;

    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        var decimals = fieldDef.Metadata.Get<int>("Decimals");

        return decimals == 0
            ?
            [
                helper.GenPropSnip(tabs, "int", name, $"{helper.Specs.ItemAccessor}.Int", summary:
                [
                    $"{name} as int. <br/>",
                    $"To get other types use methods such as .Decimal(\"{name}\")"
                ]),
            ]
            :
            [
                helper.GenPropSnip(tabs, "decimal", name, $"{helper.Specs.ItemAccessor}.Decimal", summary:
                [
                    $"{name} as decimal. <br/>",
                    $"To get other types use methods such as .Int(\"{name}\")"
                ]),
            ];
    }
}