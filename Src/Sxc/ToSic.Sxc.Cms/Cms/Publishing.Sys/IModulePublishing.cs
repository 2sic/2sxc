namespace ToSic.Sxc.Cms.Publishing.Sys;

// Note: maybe some day this should go into a .Cms namespace
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModulePublishing: IHasLog
{
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

    int GetLatestVersion(int moduleId);

    int GetPublishedVersion(int moduleId);

    void Publish(int moduleId, int version);
}
