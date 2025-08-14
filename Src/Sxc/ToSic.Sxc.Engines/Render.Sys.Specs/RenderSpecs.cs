using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Render.Sys.Specs;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RenderSpecs
{
    public object? Data { get; init; }
    public bool UseLightspeed { get; init; }

    /// <summary>
    /// Would contain errors from dnn requirements check (like c# 8.0)
    /// </summary>
    public RenderEngineResult? RenderEngineResult { get; init; }

    /// <summary>
    /// Override default behavior in Oqtane
    /// </summary>
    public bool IncludeAllAssetsInOqtane { get; init; }
}