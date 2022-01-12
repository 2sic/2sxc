using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;


namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        #region Constructor / DI

        public OqtPagePublishing() : base($"{OqtConstants.OqtLogPrefix}.Publsh") { }

        #endregion

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
        {
            var containerId = (context as IContextOfBlock)?.Module.Id ?? Eav.Constants.IdNotInitialized;
            var userId = 0;
            var enabled = false;// IsEnabled(containerId);
            Log.Add($"DoInsidePublishing(module:{containerId}, user:{userId}, enabled:{enabled})");
            if (enabled)
            {
                /* ignore */
            }

            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
            Log.Add("/DoInsidePublishing");
        }



        public int GetLatestVersion(int instanceId) => 0;

        public int GetPublishedVersion(int instanceId) => 0;


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            Log.Add("publish never happened ");
        }

        //private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IBlockDataSource data, string key)
        //{
        //    var cont = data.GetStream(key, nullIfNotFound: true)?.List; // data.Out.ContainsKey(key) ? data[key]?.List : null;
        //    Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count() ?? 0}");
        //    if (cont != null) list = list.Concat(cont);
        //    return list;
        //}

    }
}
