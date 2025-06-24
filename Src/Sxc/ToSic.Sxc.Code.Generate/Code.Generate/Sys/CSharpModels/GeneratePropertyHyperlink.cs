namespace ToSic.Sxc.Code.Generate.Internal;

internal class GeneratePropertyHyperlink(CSharpGeneratorHelper helper) : GeneratePropertyBase(helper)
{
    public override ValueTypes ForDataType => ValueTypes.Hyperlink;

    public override List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs)
    {
        var name = attribute.Name;

        return
        [
            GenPropSnip(tabs, "string", name, $"{Specs.ItemAccessor}.Url", summary:
            [
                $"{name} as link (url). <br/>",
                $"To get the underlying value like 'file:72' use String(\"{name}\")"
            ]),
            GenPropSnip(tabs, "IFile", name + "File", $"{Specs.ItemAccessor}.File", sourceName: name, priority: false, usings: UsingAdam, jsonIgnore: true ,summary:
            [
                $"Get the file object for {name} - or null if it's empty or not referencing a file."
            ]),
            GenPropSnip(tabs, "IFolder", name + "Folder", $"{Specs.ItemAccessor}.Folder", sourceName: name, priority: false, usings: UsingAdam, jsonIgnore: true ,summary:
            [
                $"Get the folder object for {name}."
            ]),
        ];
    }

    private List<string> UsingAdam { get; } = ["ToSic.Sxc.Adam"];
}