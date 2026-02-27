using ToSic.Eav.Models;

namespace ToSic.Sxc.Cms.Settings.InputFields;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record StringWysiwyg() : ModelFromEntityBasic
{
    public const string ContentTypeNameId = "f8a6ed24-11c0-42c4-9022-409989489563";

    public const string SettingsPath = "Settings.InputFields.StringWysiwyg";

    public string EditorPlugin
    {
        get => field ??= GetThis("tinymce");
        set;
    }

    public string ButtonSource
    {
        get => field ??= GetThis("");
        set;
    }

    public string ButtonSourceInDebugMode
    {
        get => field ??= GetThis("");
        set;
    }

    internal static StringWysiwyg Defaults = new()
    {
        EditorPlugin = "",
        ButtonSource = "",
        ButtonSourceInDebugMode = "",
    };
}