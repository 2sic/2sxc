using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.SexyContent.Installer;
using ToSic.SexyContent.Search;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnBusinessController : ModuleSearchBase, IUpgradeable, IVersionable
    {
        private Log Log { get; }

        #region DNN Interface Members - search, upgrade, versionable

        private IPagePublishing Publishing { get; }

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            Log = new Log("DNN.BusCon", null, "starting the business controller");
            Publishing = new PagePublishing(Log);
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
        {
            Log.Add("delete version is not supported");
            //versioning.DoInsideDeleteLatestVersion(moduleId, (args) => {
            //    // NOTE for 2dm: If we want to support delete, reset any item in draft state of the content-block
            //});
        }

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
                return new SearchController(Log).GetModifiedSearchDocuments(moduleInfo, beginDate);
            }
            catch (Exception e)
            {
                throw new SearchIndexException(moduleInfo, e);
            }
        }

        #endregion
    }
}