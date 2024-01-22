using ToSic.Sxc.Edit.Toolbar.Internal;

namespace ToSic.Sxc.Edit.Toolbar;

/// <summary>
/// This is a decorator for Content-Types, which allows special configuration of buttons etc. on that type
/// </summary>
/// <remarks>
/// WIP new in v14; as of now only used on NoteDecorator, should have more soon
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ToolbarButtonDecorator(IEntity entity) : EntityBasedType(entity)
{
    public static string TypeName = "ToolbarButtonDecorator";
    public static string TypeNameId = "acc185a7-f300-4468-bce8-d6a64038989d";

    public static string KeyColor = "color";
    public static string KeyIcon = "icon";
    public static string KeyData = "data";
    public static string KeyNote = "note";
    //public static string KeyIconSvgPrefix = "svg:";

    public string Command => GetThis("");

    public bool UiData => GetThis(false);

    public string Ui => GetThis("");

    public string UiIcon => GetThis("");

    public string UiColor=> GetThis("").Trim('#');

    public string AllRules()
    {
        var addOns = new
        {
            icon = UiIcon,
            color = UiColor
        };

        return ToolbarBuilderUtilities.GetUi2Url().SerializeWithChild(Ui, addOns);
    }

}