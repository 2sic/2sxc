using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public bool ShowDraftsInData { get; }

        public bool VersioningEnabled { get; }

        public CmsManager(IAppIdentityWithPublishingState app, ILog parentLog) : base(app, parentLog)
        {
            ShowDraftsInData = app.ShowDraftsInData;
            VersioningEnabled = app.VersioningEnabled;
        }

        // todo: drop this interface asap.
        public CmsManager(IAppIdentity app, bool showDrafts, bool enablePublishing, ILog parentLog) : base(app, parentLog)
        {
            ShowDraftsInData = showDrafts;
            VersioningEnabled = enablePublishing;
        }

        public CmsManager(int zoneId, int appId, bool showDrafts, bool enablePublishing, ILog parentLog) : base(zoneId, appId, parentLog)
        {
            ShowDraftsInData = showDrafts;
            VersioningEnabled = enablePublishing;
        }

        // todo: th
        public CmsManager(int appId, ILog parentLog) : base(appId, parentLog)
        {

        }

        public new CmsRuntime Read => _runtime ?? (_runtime = new CmsRuntime(Data, Log, ShowDraftsInData, VersioningEnabled));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager(this, Log));
        private BlocksManager _blocks;


    }
}
