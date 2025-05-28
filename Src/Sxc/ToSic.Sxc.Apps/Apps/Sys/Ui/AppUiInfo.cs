namespace ToSic.Eav.Apps.Internal.Ui;

[ShowApiWhenReleased(ShowApiMode.Never)]
public struct AppUiInfo
{
    public string Name;
    public int AppId;
    public bool SupportsAjaxReload;
    public string Thumbnail;
    public string Version;
}