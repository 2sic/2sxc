﻿using System;
using ToSic.Eav;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;


namespace ToSic.Sxc.Cms.Publishing
{
    internal class BasicPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        public BasicPagePublishing() : base($"{LogNames.NotImplemented}.Publsh") { }

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
        {
            var wrapLog = Log.Call();
            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
            wrapLog(null);
        }



        public int GetLatestVersion(int instanceId) => 0;

        public int GetPublishedVersion(int instanceId) => 0;


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            Log.Add("publish never happened ");
        }

    }
}
