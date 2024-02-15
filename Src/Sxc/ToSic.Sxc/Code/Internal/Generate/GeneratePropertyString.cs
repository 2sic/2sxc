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
            new(nameId: name, code: GenerateProperty(tabs: tabs, returnType: "string", name: name, method: "String", parameters: "fallback: \"\"")),
        ];
    }
}