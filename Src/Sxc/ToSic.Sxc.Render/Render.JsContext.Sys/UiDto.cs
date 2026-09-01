namespace ToSic.Sxc.Render.JsContext.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UiDto(bool autoToolbar)
{
    public bool AutoToolbar { get; } = autoToolbar;
}