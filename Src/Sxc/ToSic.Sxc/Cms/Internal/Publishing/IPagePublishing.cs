using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Cms.Internal.Publishing;

// Note: maybe some day this should go into a .Cms namespace
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPagePublishing: IHasLog
{
    /// <summary>
    /// Wraps an action and performs pre/post processing related to versioning of the environment.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action);

    ///// <summary>
    ///// Wraps an action inside publish of latest version.
    ///// NOTE: Should be called by the business-controller of the module. The controller must implement the IVersionable.
    ///// </summary>
    ///// <param name="moduleId"></param>
    //void DoInsidePublishLatestVersion(int moduleId, Action<VersioningActionInfo> action);

    ///// <summary>
    ///// Wraps an action inside delete/discard of latest version.
    ///// NOTE: Should be called by the business-controller of the module. The controller must implement the IVersionable.
    ///// </summary>
    ///// <param name="moduleId"></param>
    //void DoInsideDeleteLatestVersion(int moduleId, Action<VersioningActionInfo> action);

    int GetLatestVersion(int instanceId);

    int GetPublishedVersion(int instanceId);

    void Publish(int instanceId, int version);
}