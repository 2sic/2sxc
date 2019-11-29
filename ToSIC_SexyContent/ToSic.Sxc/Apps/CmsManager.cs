using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public class CmsManager: AppManager, IAppIdentityWithPublishingState
    {
        public bool ShowDrafts { get; }

        public bool EnablePublishing { get; }

        public CmsManager(IAppIdentityWithPublishingState app, ILog parentLog) : base(app, parentLog)
        {
            ShowDrafts = app.ShowDrafts;
            EnablePublishing = app.EnablePublishing;
        }

        [Obsolete("todo: drop this interface asap")]
        public CmsManager(IInAppAndZone app, bool showDrafts, bool enablePublishing, ILog parentLog) : base(app, parentLog)
        {
            ShowDrafts = showDrafts;
            EnablePublishing = enablePublishing;
        }

        public CmsManager(int zoneId, int appId, bool showDrafts, bool enablePublishing, ILog parentLog) : base(zoneId, appId, parentLog)
        {
            ShowDrafts = showDrafts;
            EnablePublishing = enablePublishing;
        }

        // todo: th
        public CmsManager(int appId, ILog parentLog) : base(appId, parentLog)
        {

        }

        public new CmsRuntime Read => _runtime ?? (_runtime = new CmsRuntime(Data, Log, ShowDrafts, EnablePublishing));
        private CmsRuntime _runtime;


        public ViewsManager Views => _views ?? (_views = new ViewsManager(this, Log));
        private ViewsManager _views;

        public BlocksManager Blocks => _blocks ?? (_blocks = new BlocksManager(this, Log));
        private BlocksManager _blocks;


    }
}
