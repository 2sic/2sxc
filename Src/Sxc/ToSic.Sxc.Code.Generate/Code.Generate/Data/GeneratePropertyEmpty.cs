using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyEmpty(CSharpGeneratorHelper helper) : ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.Empty;

    /// <summary>
    /// Empty is - empty - so we don't generate anything
    /// </summary>
    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
        => [];
}