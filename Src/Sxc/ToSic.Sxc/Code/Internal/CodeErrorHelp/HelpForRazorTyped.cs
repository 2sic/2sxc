using ToSic.Eav.Code.Help;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;
using static ToSic.Sxc.Code.Internal.CodeErrorHelp.CodeHelpBuilder;


namespace ToSic.Sxc.Code.Internal.CodeErrorHelp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HelpForRazorTyped
{
    #region Methods to help build the data

    internal const string IsNotSupportedIn16Plus = "is not supported in RazorTyped / CodeTyped";

    private static CodeHelp NotExists(string property, params string[] replacement)
        => new GenNotExist(property, replacement) { MsgNotSupportedIn = IsNotSupportedIn16Plus }.Generate();

    #endregion

    /// <summary>
    /// Compile Help for RazorTyped etc.
    /// </summary>
    public static List<CodeHelp> Compile16 => _help ??= BuildListFromDiverseSources(
        // use old `Convert` object
        HelpForRazor14.SystemConvertIncorrectUse,

        // Use Dnn
        HelpForRazor12.DnnObjectNotInHybrid,

        // use `CreateSource(name)
        NotExists("CreateSource", "Kit.Data.CreateSource(...)"),
        NotExists("CreateInstance", "GetCode(...)"),

        // Use AsDynamic(...)
        new GenNotExist("AsDynamic", [
            ("AsItem(...)", "to get a standard ITypedItem"),
            ("AsItemList(...)", "to get a list of ITypedItem"),
            ("AsTyped(...)", "to get a ITyped from an anonymous object"),
            ("AsStack(...)", "to get a typed stack which merges various objects"),
            ("Kit.Json.ToTyped(string)", "to get an ITyped from a json string")
        ])
        {
            MsgNotSupportedIn = IsNotSupportedIn16Plus,
        },

        // Access .List
        // TODO: Resulting API NAMING NOT FINAL
        BuildVariations(
            NotExists("List", "MyItems", "AsItems(MyData.Get())"),
            h => new(h, detect: "does not contain a definition for 'List'"),
            h => new(h, detect: "error CS0305: Using the generic type " +
                                "'System.Collections.Generic.List<T>' requires 1 type arguments")
        ),

        // Core data objects like Content, Presentation, List...
        NotExists("Content", "MyItem"),
        NotExists("Header", "MyHeader"),
        NotExists("Presentation", "MyItem.Presentation"),
        NotExists("ListContent", "MyHeader"),
        NotExists("ListPresentation", "MyHeader.Presentation"),

        // Settings / Resources
        NotExists("Settings", "App.Settings", "AllSettings"),
        BuildVariations(
            NotExists("Resources", "App.Resources", "AllResources"),
            h => new(h, detect: "does not exist in the namespace " +
                                "'Resources' (are you missing an assembly reference?)"),
            h => new(h, detect: "error CS0118: 'Resources' is a " +
                                "'namespace' but is used like a 'variable'"),
            h => new(h, detectRegex: true, detect: "error CS0234: The type or namespace name " +
                                                   "'.*' does not exist in the namespace 'Resources' \\(are you missing an assembly reference\\?\\)")
        ),

        // Edit object
        new GenNotExist("Edit", [
            ("Kit.Toolbar.Default()...", "to build a standard toolbar"),
            ("Kit.Toolbar.Empty()...", "to start with an empty toolbar"),
            ("MyUser.IsContentAdmin", "to find out if edit is enabled"),
            ("Kit.Edit", "to really use the Edit object (not often needed, as the replacements are better)")
        ])
        {
            MsgNotSupportedIn = IsNotSupportedIn16Plus,
        },

        // AsAdam(...)
        new GenNotExist("AsAdam", ("object.Folder(\"FieldName\")", "Use the Folder(...) method on an Item"))
        {
            Comments = "AsAdam isn't needed any more, since there is an easier syntax.",
            MsgNotSupportedIn = IsNotSupportedIn16Plus,
        },
        NotExists("Data", "MyData"),

        // Renamed properties on ITypedItem
        new GenChangeOn("ToSic.Sxc.Data.ITypedItem", "EntityId", alt: "Id"),
        new GenChangeOn("ToSic.Sxc.Data.ITypedItem", "EntityGuid", alt: "Guid"),
        new GenChangeOn("ToSic.Sxc.Data.ITypedItem", "EntityTitle", alt: "Title"),

        // Renamed properties on IAppTyped: Path, Folder
        new GenChangeOn("ToSic.Sxc.Apps.IAppTyped", "Path",
            alt: $".{nameof(IAppTyped.Folder)}.{nameof(IAsset.Url)}"),
        new GenChangeOn("ToSic.Sxc.Apps.IAppTyped", "PhysicalPath",
            alt: $".{nameof(IAppTyped.Folder)}.{nameof(Eav.Apps.Assets.IAsset.PhysicalPath)}"),
        new GenChangeOn("ToSic.Sxc.Apps.IAppTyped", "PathShared",
            alt: $".{nameof(IAppTyped.FolderAdvanced)}(location: \"shared\").{nameof(IAsset.Url)}"),
        new GenChangeOn("ToSic.Sxc.Apps.IAppTyped", "PhysicalPathShared",
            alt: $".{nameof(IAppTyped.FolderAdvanced)}(location: \"shared\").{nameof(Eav.Apps.Assets.IAsset.PhysicalPath)}"),

        new GenChangeOn("ToSic.Sxc.Context.ICmsView", "PathShared",
            alt: $"MyView.{nameof(ICmsView.Folder)}.{nameof(Eav.Apps.Assets.IAsset.PhysicalPath)}"),
        new GenChangeOn("ToSic.Sxc.Context.ICmsView", "PhysicalPath",
            alt: $"MyView.{nameof(ICmsView.Folder)}.{nameof(Eav.Apps.Assets.IAsset.PhysicalPath)}"),
        new GenChangeOn("ToSic.Sxc.Context.ICmsView", "PhysicalPathShared",
            alt: $"MyView.{nameof(ICmsView.Folder)}.{nameof(Eav.Apps.Assets.IAsset.PhysicalPath)}"),

        // razor compile errors
        HelpForRazorCompileErrors.UnknownNamespace,
        HelpForRazorCompileErrors.ProbablySemicolonAfterInherits,
        HelpForRazorCompileErrors.ProbablyCommentAfterInherits
    );
    private static List<CodeHelp> _help;
}