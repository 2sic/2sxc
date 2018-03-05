using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ToSic.Eav;
using System.Data.SqlClient;
using System.Configuration;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.Installer
{
    internal class InstallationController
    {
        public bool SaveUnimportantDetails = true;

        private readonly Logger _installLogger;

        private Log Log = new Log("Ist.InstCo");

        /// <summary>
        /// This static initializer will do a one-time check to see if everything is ready,
        /// so subsequent access to this property will not need to do anything any more
        /// </summary>
        static InstallationController()
        {
            UpdateUpgradeCompleteStatus();
        }

        private static void UpdateUpgradeCompleteStatus()
        {
            UpgradeComplete = new InstallationController().IsUpgradeComplete(Settings.Installation.LastVersionWithServerChanges, "- static check");
        }

        /// <summary>
        /// Instance initializers...
        /// </summary>
        public InstallationController()
        {
            _installLogger = new Logger(SaveUnimportantDetails);
        }

        internal string UpgradeModule(string version)
        {
            // Check if table "ToSIC_SexyContent_Templates" exists. 
            // If it's gone, then PROBABLY skip all upgrade-codes incl. 8.11!
            var sql = @"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToSIC_SexyContent_Templates]') AND TYPE IN(N'U')";
            var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
            sqlConnection.Open();
            var sqlCommand = new SqlCommand(sql, sqlConnection);
            var runDbChangesUntil811 = (Int32)sqlCommand.ExecuteScalar() == 1; // if there is one result row, this means the templates table still exists, we need to run changes before 08.11
            sqlConnection.Close();

            // if version is 01.00.00, the upgrade has to run because log files should be cleared
            if (!runDbChangesUntil811 && version != "01.00.00" && (new Version(version) <= new Version(8,11,0)))
            {
                _installLogger.LogStep(version, "Upgrade skipped because 00.99.00 install detected (installation of everything until and including 08.11 has been done by 00.99.00.SqlDataProvider)", true);
                _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
                return version;
            }

            _installLogger.LogStep(version, "UpgradeModule starting", false);

            // Configure Unity / eav, etc.
            Settings.EnsureSystemIsInitialized();
            // new UnityConfig().Configure();

            // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
            if (version != "01.00.00" && IsUpgradeComplete(version, "- Check on Start UpgradeModule"))
            {
                _installLogger.LogStep(version, "Apparently trying to update this version, but this versions upgrade is apparently compeleted, will abort");
                throw new Exception("2sxc upgrade for version " + version +
                                    " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");
            }
            _installLogger.LogStep(version, "version / upgrade-complete test passed");

            if (IsUpgradeRunning)
            {
                _installLogger.LogStep(version, "Apparently upgrade is running, will abort");
                throw new Exception("2sxc upgrade for version " + version +
                                    " started, but the upgrade is already running. Aborting upgrade.");
            }
            _installLogger.LogStep(version, "is-upgrade-running test passed");

            IsUpgradeRunning = true;
            _installLogger.LogStep(version, "----- Upgrade to " + version + " started -----");

            try
            {

                switch (version)
                {
                    case "01.00.00": // Make sure that log folder empty on new installations (could happen if 2sxc was already installed on a system)
                        MaybeResetUpgradeLogsToStartAgainFromV1();
                        break;
                    case "07.02.00":
                        new V7(version, _installLogger, Log).Version070200();
                        break;
                    case "07.02.02":
                        // Make sure upgrades FROM BETWEEN 07.00.00 and 07.02.02 do not run again (create log files for each of them)
                        _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade("07.00.00", false);
                        _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade("07.00.03", false);
                        _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade("07.02.00", false);
                        break;
                    case "07.03.01":
                        new V7(version, _installLogger, Log).EnsurePipelineDesignerAttributeSets(); // Need to ensure this again because of upgrade problems
                        break;
                    case "07.03.03":
                        new V7(version, _installLogger, Log).Version070303();
                        break;
                    case "08.00.02":
                        new V8(version, _installLogger, Log).Version080002();
                        break;
                    case "08.00.04":
                        new V8(version, _installLogger, Log).Version080004();
                        break;
                    case "08.00.07":
                        new V8(version, _installLogger, Log).Version080007();
                        break;
                    case "08.01.00":
                        new V8(version, _installLogger, Log).Version080100();
                        break;
                    case "08.03.00":
                        break;
                    case "08.03.02":
                        new V8(version, _installLogger, Log).Version080302();
                        break;
                    case "08.03.03":
                        new V8(version, _installLogger, Log).Version080303();
                        break;
                    case "08.03.05":
                        Helpers.ImportXmlSchemaOfVersion("08.03.05", false);
                        break;
                    case "08.04.00":
                        Helpers.ImportXmlSchemaOfVersion("08.04.00", false);
                        break;
                    case "08.04.03":
                        Helpers.ImportXmlSchemaOfVersion("08.04.03", false);
                        break;
                    case "08.04.05":
                        Helpers.ImportXmlSchemaOfVersion("08.04.03", false); // this is correct, RE-RUN 08.04.03 as there was a problem in that version number before
                        break;
                    case "08.05.00":
                        Helpers.ImportXmlSchemaOfVersion("08.05.00", false);
                        break;
                    case "08.05.01":
                        new V8(version, _installLogger, Log).Version080501();
                        break;
                    case "08.05.02":
                        Helpers.ImportXmlSchemaOfVersion("08.05.02", false);
                        break;
                    case "08.05.03":
                        Helpers.ImportXmlSchemaOfVersion("08.05.03", false);
                        break;
                    case "08.05.05":
                        Helpers.ImportXmlSchemaOfVersion("08.05.05", false);
                        break;
                    case "08.11.00":
                        new V8(version, _installLogger, Log).Version081100();
                        break;

                        // warning!!! when you add a new case, make sure you upgrade the version number on Settings.Installation.LastVersionWithServerChanges!!!
                }
                _installLogger.LogStep(version, "version-list check / switch done", false);

                // Increase ClientDependency version upon each upgrade (System and all Portals)
                // prevents browsers caching old JS and CSS files for editing, which could cause several errors
                // only set this on the last upgraded version, to prevent crazy updating the client-resource-cache while upgrading
                if (version == Settings.Installation.UpgradeVersionList.Last())
                {
                    _installLogger.LogStep(version, "ClientResourceManager- seems to be last item in version-list, will clear");
                    try
                    {
                        var envInstaller = Factory.Resolve<IInstallerEnvironment>();
                        envInstaller.UpgradeCompleted();
                    }
                    catch { /* ignore */ }
                    //ClientResourceManager.UpdateVersion();
                    _installLogger.LogStep(version, "ClientResourceManager- done clearing");

                    UpdateUpgradeCompleteStatus();
                    _installLogger.LogStep(version, "updated upgrade-complete status");
                }

                _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
                _installLogger.LogStep(version, "----- Upgrade to " + version + " completed -----");

            }
            catch (Exception e)
            {
                _installLogger.LogStep(version, "Upgrade failed - " + e.Message);
                throw;
            }
            finally
            {
                IsUpgradeRunning = false;
            }
            _installLogger.LogStep(version, "UpgradeModule done / returning");
            return version;
        }

        internal void MaybeResetUpgradeLogsToStartAgainFromV1()
        {
            _installLogger.LogStep("", "Maybe reset logs start");
            // this condition only applies, if 2sxc upgrade 7 didn't happen yet
            if (DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)
                    .GetContentType("2SexyContent-Template") != null) return;

            _installLogger.LogStep("", "Will reset all logs now");
            _installLogger.DeleteAllLogFiles();
            _installLogger.LogStep("", "Maybe Reset logs done");
        }


        internal void FinishAbortedUpgrade()
        {
            _installLogger.LogStep("", "FinishAbortedUpgrade starting", false);
            _installLogger.LogStep("", "Will handle " + Settings.Installation.UpgradeVersionList.Length + " versions");
            // Run upgrade again for all versions that do not have a corresponding logfile
            foreach (var upgradeVersion in Settings.Installation.UpgradeVersionList)
            {
                var complete = IsUpgradeComplete(upgradeVersion, "- check for FinishAbortedUpgrade");
                _installLogger.LogStep("", "Status for version " + upgradeVersion + " is " + complete);
                if (!complete)
                    UpgradeModule(upgradeVersion);
            }

            _installLogger.LogStep("", "FinishAbortedUpgrade done", false);

            //_logger.SaveDetailedLog();
            // Restart application
            HttpRuntime.UnloadAppDomain();
        }




        internal string CheckUpgradeMessage(bool isSuperUser)
        {
            // Upgrade success check - show message if upgrade did not run successfully
            if (UpgradeComplete) return null;

            return IsUpgradeRunning
                ? "It looks like a 2sxc upgrade is currently running.Please wait for the operation to complete(the upgrade may take a few minutes)."
                : isSuperUser
                    ? "Module upgrade did not complete (<a href='http://2sxc.org/en/help/tag/install' target='_blank'>read more</a>). Click to complete: <br><a class='dnnPrimaryAction' onclick='$2sxc.system.finishUpgrade(this)'>complete upgrade</a>"
                    : "Module upgrade did not complete successfully. Please login as host user to finish the upgrade.";
        }

        #region Status Stuff
        internal static bool UpgradeComplete;

        internal bool IsUpgradeComplete(string version, string note = "")
        {
            _installLogger.LogStep(version, "IsUgradeComplete checking " + note, false);
            var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + version + ".resources");
            var complete = File.Exists(logFilePath);
            _installLogger.LogStep(version, "IsUgradeComplete: " + complete, false);
            return complete;
        }

        // cache the status
        private static bool? _running;
        /// <summary>
        /// Set / Check if it's running, by storing the static info but also creating/releasing a lock-file
        /// We need the lock file in case another system would try to read the status, which doesn't share
        /// this running static instance
        /// </summary>
        internal bool IsUpgradeRunning
        {
            get
            {
                return _running ?? (_running = new Lock().IsSet).Value;
            }
            private set
            {
                try
                {
                    _installLogger.LogStep("", "set upgrade running - " + value);

                    if (value)
                        new Lock().Set();
                    else
                        new Lock().Release();
                    _installLogger.LogStep("", "set upgrade running - " + value + " - done");
                }
                catch
                {
                    _installLogger.LogStep("", "set upgrade running - " + value + " - error!");
                }
                finally
                {
                    _running = value;
                }
            }
        }
        #endregion


    }
}