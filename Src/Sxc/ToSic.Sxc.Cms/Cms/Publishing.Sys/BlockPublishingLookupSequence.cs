namespace ToSic.Sxc.Cms.Publishing.Sys;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public enum BlockPublishingLookupSequence
{
    Unknown = 0,

    /// <summary>
    /// Preflight check, to see if disabled / forbidden
    /// </summary>
    PreflightDisabled = 1,

    /// <summary>
    /// Platform specific implementation, like Dnn/Oqtane
    /// </summary>
    Platform = 200,

    /// <summary>
    /// Final step, if nothing else fired.
    /// </summary>
    Final = 998,
    
    /// <summary>
    /// Fallback steps, if nothing found - should be registered.
    /// </summary>
    Fallback = 999,
}