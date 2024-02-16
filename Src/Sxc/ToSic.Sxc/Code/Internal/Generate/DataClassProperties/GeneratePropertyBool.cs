namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyBool: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Boolean;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return [GenPropSnip(tabs, "bool", name, "Bool", summary:
        [
            $"{name} as bool. <br/>",
            $"To get nullable use .Get(\"{name}\") as bool?;"
        ])];
    }
}