using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Render.Sys.Specs;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RenderSpecs
{
    public object? Data { get; init; }
    public bool UseLightspeed { get; init; }
    public RenderEngineResult? RenderEngineResult { get; init; } // errors from dnn requirements check (like c# 8.0)
    public bool IncludeAllAssetsInOqtane { get; init; } // override default behavior in Oqtane
}