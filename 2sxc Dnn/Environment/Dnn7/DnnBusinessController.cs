using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.SexyContent.Search;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Environment.Dnn7.Installation;
using ToSic.SexyContent.Environment.Dnn7.Search;

namespace ToSic.SexyContent.Environment.Dnn7
{
    /// <summary>
    /// This is the connector-class which DNN consults when it needs to know things about a module
    /// It's used in the background, not when the page is loading
    /// </summary>
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {
        private Log Log { get; }

        #region DNN Interface Members - search, upgrade, versionable

        private IPagePublishing Publishing
        {
            get
            {
                if (_publishing != null) return Publishing;

                // if publishing is used, make sure it's in the log-history
                _publishing = new PagePublishing(Log);
                History.Add("dnn-publishing", Log);
                return _publishing;
            }
        }

        private IPagePublishing _publishing;

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            Log = new Log("DNN.BusCon", null, "starting the business controller");
        }

        public int GetLatestVersion(int moduleId) => Publishing.GetLatestVersion(moduleId);

        public int GetPublishedVersion(int moduleId) => Publishing.GetPublishedVersion(moduleId);

        public void PublishVersion(int moduleId, int version)
        {
            Log.Add("publish m#{moduleId}, v:{version}");
            Publishing.Publish(moduleId, version);

            try
            {
                Logging.LogToDnn("Publishing", "ok", Log, force:true);
            }
            catch
            {
                // ignore
            }
        }



        public void DeleteVersion(int moduleId, int version) 
            => Log.Add("delete version is not supported");

        public int RollBackVersion(int moduleId, int version)
        {
            Log.Add("DNN tried to rollback version " + version + ", but 2sxc does not support this.");

            // Return the currently published version, because this is what the module's state is after this operation
            return Publishing.GetPublishedVersion(moduleId);
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
            Logging.LogToDnn("Upgrade", "ok", Log, force:true); // always log, this often causes hidden problems
            return res;
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                return new SearchController(Log).GetModifiedSearchDocuments(new DnnInstanceInfo(moduleInfo), beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion
    }
}