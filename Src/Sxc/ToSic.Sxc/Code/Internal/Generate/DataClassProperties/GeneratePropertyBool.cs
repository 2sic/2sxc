namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyBool: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Boolean;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return [GenPropSnip(tabs, "bool", name, "Bool", summary:
        [
            $"Get the bool of {name}.",
            $"To get nullable use .Get(\"{name}\") as bool?;"
        ])];
    }
}