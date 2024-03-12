namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyBool(CodeGenHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Boolean;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return [GenPropSnip(tabs, "bool", name, $"{Specs.ItemAccessor}.Bool", summary:
        [
            $"{name} as bool. <br/>",
            $"To get nullable use .Get(\"{name}\") as bool?;"
        ])];
    }
}