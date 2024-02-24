namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyDateTime: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.DateTime;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "DateTime", name, $"{ItemAccessor}.DateTime", usings: UsingDateTime, summary:
            [
                $"{name} as DateTime.",
            ]),
        ];
    }

    private List<string> UsingDateTime { get; } = ["System"];
}