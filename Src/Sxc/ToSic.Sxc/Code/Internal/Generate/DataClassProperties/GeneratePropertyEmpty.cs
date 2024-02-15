namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyEmpty: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Empty;

    /// <summary>
    /// Empty is - empty - so we don't generate anything
    /// </summary>
    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs) 
        => [];
}