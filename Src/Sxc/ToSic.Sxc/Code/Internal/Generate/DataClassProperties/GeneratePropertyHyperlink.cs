namespace ToSic.Sxc.Code.Internal.Generate;

internal class GeneratePropertyHyperlink: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Hyperlink;

    public override List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "string", name, "Url", summary:
            [
                $"{name} as link (url). <br/>",
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