namespace ToSic.Sxc.Code.Generate.Internal;

internal class GeneratePropertyDateTime(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.DateTime;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

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