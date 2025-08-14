using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Render.Sys.Specs;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RenderSpecs
{
    /// <summary>
    /// The data / view model to be used in the Razor file.
    /// </summary>
    public object? Data { get; init; }

    /// <summary>
    /// Info if LightSpeed should be used for rendering - ATM just used for the statistics in the UI.
    /// </summary>
    public bool UseLightspeed { get; init; }

    /// <summary>
    /// Would contain errors from dnn requirements check (like c# 8.0)
    /// </summary>
    public RenderEngineResult? RenderEngineResult { get; init; }

    /// <summary>
    /// Override default behavior in Oqtane
    /// </summary>
    public bool IncludeAllAssetsInOqtane { get; init; }

    /// <summary>
    /// Information about the partial caching, if it is used.
    /// </summary>
    /// <remarks>
    /// Additional data ATM is meant for passing back information after running the view.
    /// Specific example ATM is caching information, which will be used to decide if the result should be cached or not.
    /// 
    /// This is currently done this way because we're not sure how else to get this information back to the engine,
    /// because the Razor View engine is a bit internal and can't just change the return signature.
    /// </remarks>
    public RenderPartialCachingSpecs PartialCaching { get; init; } = new();
}