namespace ToSic.Sxc.Apps.Sys.Ui;

[ShowApiWhenReleased(ShowApiMode.Never)]
public struct AppUiInfo
{
    public string Name;
    public int AppId;
    public bool SupportsAjaxReload;
    public string? Thumbnail;
    public string Version;
}