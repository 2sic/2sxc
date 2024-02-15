namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyHyperlink: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Hyperlink;

    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;
        return [
            new(name, GenerateProperty(tabs, "string", name, "Url")),
            new(name + "File", GenerateProperty(tabs, "IFile", name + "File", "File"), false, Usings),
            new(name + "Folder", GenerateProperty(tabs, "IFolder", name + "Folder", "Folder"), false, Usings),
        ];
    }

    private List<string> Usings { get; }  = ["ToSic.Sxc.Adam"];
}