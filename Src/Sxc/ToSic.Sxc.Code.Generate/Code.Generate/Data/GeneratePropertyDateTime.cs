using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyDateTime(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.DateTime;

    public override List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        return
        [
            GenPropSnip(tabs, "DateTime", name, $"{Specs.ItemAccessor}.DateTime", usings: UsingDateTime, summary:
            [
                $"{name} as DateTime.",
            ]),
        ];
    }

    private List<string> UsingDateTime { get; } = ["System"];
}