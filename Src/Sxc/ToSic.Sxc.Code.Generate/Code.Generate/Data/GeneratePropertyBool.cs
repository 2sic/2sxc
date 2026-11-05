using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyBool(CSharpGeneratorHelper helper): ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.Boolean;

    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        return [helper.GenPropSnip(tabs, "bool", name, $"{helper.Specs.ItemAccessor}.Bool", summary:
        [
            $"{name} as bool. <br/>",
            $"To get nullable use .Get(\"{name}\") as bool?;"
        ])];
    }
}