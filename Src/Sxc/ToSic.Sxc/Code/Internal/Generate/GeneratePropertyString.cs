namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyString: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Hyperlink;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;
        return [
            new(name, GenerateProperty(tabs, "string", name, "String", parameters: "fallback: \"\"")),
        ];
    }
}