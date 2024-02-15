namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyBool: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Boolean;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs) 
        => [new(attribute.Name, GenerateProperty(tabs, "bool", attribute.Name, "Bool"))];
}