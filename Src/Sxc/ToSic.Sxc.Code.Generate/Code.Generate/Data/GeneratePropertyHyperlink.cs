using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class GeneratePropertyHyperlink(CSharpGeneratorHelper helper) : ICodeGenProperty
{
    public ValueTypes ForDataType => ValueTypes.Hyperlink;

    public List<CodeFragment> Generate(IContentTypeField fieldDef, int tabs)
    {
        var name = fieldDef.Name;

        return
        [
            helper.GenPropSnip(tabs, "string", name, $"{helper.Specs.ItemAccessor}.Url", summary:
            [
                $"{name} as link (url). <br/>",
                $"To get the underlying value like 'file:72' use String(\"{name}\")"
            ]),
            helper.GenPropSnip(tabs, "IFile", name + "File", $"{helper.Specs.ItemAccessor}.File", sourceName: name, priority: false, usings: UsingAdam, jsonIgnore: true ,summary:
            [
                $"Get the file object for {name} - or null if it's empty or not referencing a file."
            ]),
            helper.GenPropSnip(tabs, "IFolder", name + "Folder", $"{helper.Specs.ItemAccessor}.Folder", sourceName: name, priority: false, usings: UsingAdam, jsonIgnore: true ,summary:
            [
                $"Get the folder object for {name}."
            ]),
        ];
    }

    private List<string> UsingAdam { get; } = ["ToSic.Sxc.Adam"];
}