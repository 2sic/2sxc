using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Oqt.Shared;


namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        #region Constructor / DI

        public OqtPagePublishing() : base($"{OqtConstants.OqtLogPrefix}.Publsh") { }

        #endregion

        public void DoInsidePublishing(IContextOfBlock context, Action<VersioningActionInfo> action)
        {
            var containerId = context.Module.Id;
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

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IBlockDataSource data, string key)
        {
            var cont = data.Out.ContainsKey(key) ? data[key]?.Immutable/*?.ToList()*/ : null;
            Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}");
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
