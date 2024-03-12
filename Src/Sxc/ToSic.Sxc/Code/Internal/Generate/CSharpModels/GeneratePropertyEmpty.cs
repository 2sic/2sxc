namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyEmpty(CodeGenHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Empty;

    /// <summary>
    /// Empty is - empty - so we don't generate anything
    /// </summary>
    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs) 
        => [];
}