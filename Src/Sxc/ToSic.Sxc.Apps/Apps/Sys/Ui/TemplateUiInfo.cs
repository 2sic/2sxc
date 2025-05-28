namespace ToSic.Eav.Apps.Internal.Ui;

[ShowApiWhenReleased(ShowApiMode.Never)]
public struct TemplateUiInfo
{
    public int TemplateId;
    public string Name;
    public string ContentTypeStaticName;
    public bool IsHidden;
    public string Thumbnail;
    public bool IsDefault; // new, v13
}