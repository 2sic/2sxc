using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Search;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is the connector-class which DNN consults when it needs to know things about a module
    /// It's used in the background, not when the page is loading
    /// </summary>
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {
        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            Log = new Log("DNN.BusCon", null, "starting");
        }

        private ILog Log { get; }

        #region DNN Interface Members - search, upgrade, versionable

        private IPagePublishing Publishing
        {
            get
            {
                if (_publishing != null) return Publishing;

                // if publishing is used, make sure it's in the log-history
                _publishing = new Sxc.Dnn.Cms.DnnPagePublishing().Init(Log);
                History.Add("dnn-publishing", Log);
                return _publishing;
            }
        }
        private IPagePublishing _publishing;


        public int GetLatestVersion(int instanceId) => Publishing.GetLatestVersion(instanceId);

        public int GetPublishedVersion(int instanceId) => Publishing.GetPublishedVersion(instanceId);

        public void PublishVersion(int instanceId, int version)
        {
            Log.Add($"publish m#{instanceId}, v:{version}");
            Publishing.Publish(instanceId, version);

            try
            {
                DnnLogging.LogToDnn("Publishing", "ok", Log, force:true);
            }
            catch
            {
                // ignore
            }
        }



        public void DeleteVersion(int instanceId, int version) 
            => Log.Add("delete version is not supported");

        public int RollBackVersion(int instanceId, int version)
        {
            Log.Add("DNN tried to rollback version " + version + ", but 2sxc does not support this.");

            // Return the currently published version, because this is what the module's state is after this operation
            return Publishing.GetPublishedVersion(instanceId);
        }

        /// <summary>
        /// This is part of the IUpgradeable of DNN
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string UpgradeModule(string version)
        {
            Log.Add($"upgrade module - start for v:{version}");
            var res = new InstallationController().UpgradeModule(version);
            Log.Add($"result:{res}");
            DnnLogging.LogToDnn("Upgrade", "ok", Log, force:true); // always log, this often causes hidden problems
            return res;
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                return new SearchController(Log).GetModifiedSearchDocuments(
                    new DnnContainer().Init(moduleInfo, Log), beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion
    }
}