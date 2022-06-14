using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Cms.Publishing
{
    /// <summary>
    /// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
    /// NOTE: It is currently not in use, and that's ok. 
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class NoPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        #region Constructors

        public NoPagePublishing() : base("Eav.NoPubl") { }

        #endregion

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
        {
            var info = new VersioningActionInfo();
            action.Invoke(info);
        }

        //public void DoInsidePublishLatestVersion(int instanceId, Action<VersioningActionInfo> action)
        //{
        //    // NOTE: Do nothing!
        //}

        //public void DoInsideDeleteLatestVersion(int instanceId, Action<VersioningActionInfo> action)
        //{
        //    // NOTE: Do nothing!
        //}

        public int GetLatestVersion(int instanceId)
        {
            return 0;
        }

        public int GetPublishedVersion(int instanceId)
        {
            return 0;
        }

        public void Publish(int instanceId, int version)
        {
            // ignore
        }


    }
}
