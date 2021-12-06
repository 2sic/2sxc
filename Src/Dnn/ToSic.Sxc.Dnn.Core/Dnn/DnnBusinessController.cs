using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search.Entities;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Search;

namespace ToSic.Sxc.Dnn
{
    /// <summary>
    /// This is the connector-class which DNN consults when it needs to know things about a module
    /// It's used in the background, not when the page is loading
    /// </summary>
    public class DnnBusinessController : ModuleSearchBase
    {
        #region Constructor (not DI)

        /// <summary>
        /// Constructor overload for DotNetNuke
        /// (BusinessController needs a parameterless constructor)
        /// </summary>
        public DnnBusinessController()
        {
            Log = new Log("DNN.BusCon", null, "starting");
        }
        
        private ILog Log { get; }

        #endregion

        #region Diagnostics stuff

        public static int SearchErrorsMax = 10;

        public static int SearchErrorsCount { get; set; }

        #endregion

        #region DNN Interface Members - search, upgrade, versionable

        private IPagePublishing Publishing
        {
            get
            {
                if (_publishing != null) return Publishing;

                // if publishing is used, make sure it's in the log-history
                _publishing = Eav.Factory.StaticBuild<Cms.DnnPagePublishing>().Init(Log);
                Eav.Factory.StaticBuild<LogHistory>().Add("dnn-publishing", Log);
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
            var res = new DnnInstallationController().UpgradeModule(version);
            Log.Add($"result:{res}");
            DnnLogging.LogToDnn("Upgrade", "ok", Log, force:true); // always log, this often causes hidden problems
            return res;
        }

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            try
            {
                // changed 2021-08-28 / 2dm / 2sxc 12.04 - must verify that it works without issues
                // TODO: STV pls verify, then remove this comment
                return Eav.Factory.StaticBuild<SearchController>().Init(Log)
                    .GetModifiedSearchDocuments(Eav.Factory.StaticBuild<DnnModule>().Init(moduleInfo, Log), beginDate);
                //var sp = Eav.Factory.GetServiceProvider();
                //return new SearchController(sp, Log).Init(Log).GetModifiedSearchDocuments(
                //    Eav.Factory.StaticBuild<DnnModule>().Init(moduleInfo, Log), beginDate);
            }
            catch (Exception e)
            {
                AddSearchExceptionToLog(moduleInfo, e, nameof(DnnBusinessController));
                return new List<SearchDocument>();
            }
        }

        public static void AddSearchExceptionToLog(ModuleInfo moduleInfo, Exception e, string nameOfSource)
        {
            var errCount = SearchErrorsCount++;
            // ignore errors after 10
            if (errCount > SearchErrorsMax) return;
            
            if (errCount == SearchErrorsMax)
            {
                Exceptions.LogException(new SearchIndexException(moduleInfo,
                    new Exception(
                        $"Hit {SearchErrorsMax} SearchIndex exceptions in 2sxc modules, will stop reporting them to not flood the error log. \n" +
                        $"To start reporting again up to {SearchErrorsMax} just restart the application. \n" +
                        $"To show more errors change 'ToSic.Sxc.Dnn.{nameof(DnnBusinessController)}.{nameof(SearchErrorsMax)}' to a higher number in some code of yours like in a temporary razor view. " +
                        $"Note that in the meantime, the count may already be higher. You can always get that from {nameof(SearchErrorsCount)}."),
                    nameOfSource, errCount, SearchErrorsMax));
                return;
            }

            Exceptions.LogException(new SearchIndexException(moduleInfo, e, nameOfSource, errCount, SearchErrorsMax));
        }

        #endregion

    }
}