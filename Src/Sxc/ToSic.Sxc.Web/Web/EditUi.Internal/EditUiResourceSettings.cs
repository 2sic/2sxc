namespace ToSic.Sxc.Web.Internal.EditUi;

/// <summary>
/// Configuration which icons/fonts are needed by the various edit-UIs
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public struct EditUiResourceSettings
{
    public bool IconsMaterial { get; set; }

    public bool FontRoboto { get; set; }

    public static EditUiResourceSettings EditUi => new()
    {
        IconsMaterial = true,
        FontRoboto = true,
    };

    public static EditUiResourceSettings QuickDialog => new()
    {
        IconsMaterial = true,
        FontRoboto = true,
    };
}