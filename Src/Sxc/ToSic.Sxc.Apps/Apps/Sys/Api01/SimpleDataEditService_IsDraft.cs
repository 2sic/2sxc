using ToSic.Eav.Data.Build;
using ToSic.Sys.Utils;
using static ToSic.Eav.Apps.Internal.Api01.SaveApiAttributes;

namespace ToSic.Eav.Apps.Internal.Api01;

partial class SimpleDataEditService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="publishedState"></param>
    /// <param name="defaultPublished">Either the existing published state, or a definition if allowed</param>
    /// <param name="writePublishAllowed"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    internal static EntitySavePublishing GetPublishSpecs(object publishedState, bool? defaultPublished, bool writePublishAllowed, ILog log)
    {
        var l = log.Fn<EntitySavePublishing>($"{nameof(publishedState)}: {publishedState}; {nameof(defaultPublished)}: {defaultPublished}; {nameof(writePublishAllowed)}: {writePublishAllowed}");
        // If it already has a published original
        // Then we want to keep it that way, unless it's not allowed,
        // in which case we must branch (if we will create a draft, to be determined later on)
        var shouldBranchDrafts = defaultPublished == true && !writePublishAllowed;
        l.A($"shouldBranchDrafts: {shouldBranchDrafts}");

        switch (publishedState)
        {
            // Case No change, not specified
            case null:
            case string emptyString when string.IsNullOrEmpty(emptyString):
            case string nullWritten when nullWritten.EqualsInsensitive(PublishModeNull):
                var published = defaultPublished != false && writePublishAllowed;
                return l.ReturnAndLog(new() { ShouldPublish = published, ShouldBranchDrafts = shouldBranchDrafts}, "null/empty");

            // Case "draft"
            case string draftString when draftString.EqualsInsensitive(PublishModeDraft):
                return l.ReturnAndLog(new() { ShouldPublish = false, ShouldBranchDrafts = defaultPublished ?? false }, "draft");

            // case boolean or truthy string
            default:
                var isPublished = publishedState.ConvertOrDefault<bool>(numeric: false, truthy: true);
                return l.ReturnAndLog(new() { ShouldPublish = isPublished && writePublishAllowed, ShouldBranchDrafts = shouldBranchDrafts}, "default");
        }
    }
    
}