using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Blocks.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RenderSpecs
{
    public object Data { get; set; }
    public bool UseLightspeed { get; set; }
    public RenderEngineResult RenderEngineResult { get; set; } // errors from dnn requirements check (like c# 8.0)
    public bool IncludeAllAssetsInOqtane { get; set; } // override default behavior in Oqtane
}