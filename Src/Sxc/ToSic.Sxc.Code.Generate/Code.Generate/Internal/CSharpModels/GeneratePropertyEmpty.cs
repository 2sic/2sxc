namespace ToSic.Sxc.Code.Generate.Internal;

internal class GeneratePropertyEmpty(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Empty;

    /// <summary>
    /// Empty is - empty - so we don't generate anything
    /// </summary>
    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
        => [];
}