namespace ToSic.Sxc.Render.Sys.Specs;

/// <summary>
/// Class to forward to rendering partials.
/// It's empty - if any platform (like Razor) support it, it must inherit it and implement more properties.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record RenderPartialSpecs;
