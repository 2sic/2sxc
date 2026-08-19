using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Cms.Publishing.Sys;

/// <summary>
/// Marks a lookup work-step which can provide information about block and therefore page publishing.
/// </summary>
/// <remarks>
/// Various such services can be registered. The first to declare a result will be used.
/// If none declare a result, the default will be used.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IWorkBlockPublishingLookup: IWork<BlockPublishingSettings>, IWorkSequenceOrder;