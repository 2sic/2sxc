using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyDateTime(CSharpGeneratorHelper helper) : ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.DateTime;

    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        return
        [
            helper.GenPropSnip(tabs, "DateTime", name, $"{helper.Specs.ItemAccessor}.DateTime", usings: UsingDateTime, summary:
            [
                $"{name} as DateTime.",
            ]),
        ];
    }

    private List<string> UsingDateTime { get; } = ["System"];
}