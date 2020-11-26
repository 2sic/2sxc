using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    /// <summary>
    /// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
    /// NOTE: It is currently not in use, and that's ok. 
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class NoPagePublishingResolver : HasLog<IPagePublishingResolver>, IPagePublishingResolver
    {
        #region Constructors

        public NoPagePublishingResolver() : base("Cms.PubNone") { }

        #endregion

        public BlockPublishingState GetPublishingState(int instanceId) 
            => new BlockPublishingState {ForceDraft = false, Mode = PublishingMode.DraftOptional};
    }
}
