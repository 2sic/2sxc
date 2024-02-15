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

        //var fileComment = CodeHelper.XmlComment(tabs, summary:
        //[
        //    $"Get the file object for {name} - or null if it's empty or not referencing a file."
        //]);

        //var folderComment = CodeHelper.XmlComment(tabs, summary:
        //[
        //    $"Get the folder object for {name}."
        //]);

        return
        [
            GenPropSnip(tabs, "string", name, "Url", summary:
            [
                $"Get the link on {name}.",
                $"To get the underlying value like 'file:72' use String(\"{name}\")"
            ]),
            GenPropSnip(tabs, "IFile", name + "File", "File", priority: false, usings: UsingAdam, summary:
            [
                $"Get the file object for {name} - or null if it's empty or not referencing a file."
            ]),
            GenPropSnip(tabs, "IFolder", name + "Folder", "Folder", priority: false, usings: UsingAdam, summary:
            [
                $"Get the folder object for {name}."
            ]),
        ];
    }

    private List<string> UsingAdam { get; }  = ["ToSic.Sxc.Adam"];
}