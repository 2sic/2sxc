namespace ToSic.Sxc.Apps.Sys.Ui;

[ShowApiWhenReleased(ShowApiMode.Never)]
public struct TemplateUiInfo
{
    public int TemplateId;
    public string Name;
    public string ContentTypeStaticName;
    public bool IsHidden;
    public string? Thumbnail;
    public bool IsDefault; // new, v13
}