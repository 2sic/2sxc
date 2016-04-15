using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using DotNetNuke.Web.Client.ClientResourceManagement;
using ToSic.Eav;

namespace ToSic.SexyContent.Installer
{
    public class InstallationController
    {
        public static bool SaveUnimportantDetails = true;

        private static Logger logger = new Logger(SaveUnimportantDetails);

        /// <summary>
        /// This static initializer will do a one-time check to see if everything is ready,
        /// so subsequent access to this property will not need to do anything any more
        /// </summary>
        static InstallationController()
        {
            UpgradeComplete = IsUpgradeComplete(Settings.ModuleVersion);
        }

        internal string UpgradeModule(string version)
        {
            logger.LogStep(version, "UpgradeModule starting", false);

            // Configure Unity
            new UnityConfig().Configure();

            if (version != "01.00.00" && IsUpgradeComplete(version)) // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
                throw new Exception("2sxc upgrade for version " + version + " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");

            if (IsUpgradeRunning)
                throw new Exception("2sxc upgrade for version " + version + " started, but the upgrade is already running. Aborting upgrade.");

            IsUpgradeRunning = true;
            logger.LogStep(version, "----- Upgrade to " + version + " started -----");

            try
            {

                switch (version)
                {
                    case "01.00.00": // Make sure that log folder empty on new installations (could happen if 2sxc was already installed on a system)
                        MaybeResetUpgradeLogsToStartAgainFromV1();
                        break;
                    case "05.05.00":
                        new V5().Version050500();
                        break;
                    case "06.06.00":
                    case "06.06.04":
                        new V6().EnsurePipelineDesignerAttributeSets();
                        break;
                    case "07.00.00":
                        new V7(logger).Version070000();
                        break;
                    case "07.00.03":
                        new V7(logger).Version070003();
                        break;
                    case "07.02.00":
                        new V7(logger).Version070200();
                        break;
                    case "07.02.02":
                        // Make sure upgrades FROM BETWEEN 07.00.00 and 07.02.02 do not run again (create log files for each of them)
                        logger.LogSuccessfulUpgrade("07.00.00", false);
                        logger.LogSuccessfulUpgrade("07.00.03", false);
                        logger.LogSuccessfulUpgrade("07.02.00", false);
                        break;
                    case "07.03.01":
                        new V6().EnsurePipelineDesignerAttributeSets(); // Need to ensure this again because of upgrade problems
                        break;
                    case "07.03.03":
                        new V7(logger).Version070303();
                        break;
                    case "08.00.02":
                        new V8(logger).Version080002();
                        break;
                    case "08.00.04":
                        new V8(logger).Version080004();
                        break;
                    case "08.00.07":
                        new V8(logger).Version080007();
                        break;
                    case "08.01.00":
                        new V8(logger).Version080100();
                        break;
                    case "08.03.00":
                        break;
                    case "08.03.02":
                        new V8(logger).Version080302();
                        break;
                    case "08.03.03":
                        new V8(logger).Version080303();
                        break;
                    case "08.03.05":
                        Helpers.ImportXmlSchemaOfVersion("08.03.05", false);
                        break;
                    case "08.04.00":
                        Helpers.ImportXmlSchemaOfVersion("08.04.00", false);
                        break;
                }

                // Increase ClientDependency version upon each upgrade (System and all Portals)
                // prevents browsers caching old JS and CSS files for editing, which could cause several errors
                ClientResourceManager.UpdateVersion();

                logger.LogSuccessfulUpgrade(version);
                logger.LogStep(version, "----- Upgrade to " + version + " completed -----");

            }
            catch (Exception e)
            {
                logger.LogStep(version, "Upgrade failed - " + e.Message);
                throw;
            }
            finally
            {
                IsUpgradeRunning = false;
            }

            return version;
        }

        internal void MaybeResetUpgradeLogsToStartAgainFromV1()
        {
            // this condition only applies, if 2sxc upgrade 7 didn't happen yet
            if (
                DataSource.GetCache(Constants.DefaultZoneId, Constants.MetaDataAppId)
                    .GetContentType("2SexyContent-Template") != null) return;

            if (Directory.Exists(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)))
            {
                var files = new List<string>(Directory.GetFiles(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)));
                files.ForEach(x =>
                {
                    try
                    {
                        File.Delete(x);
                    }
                    catch
                    {
                    }
                });
            }
            return;
        }

        internal void FinishAbortedUpgrade()
        {
            logger.LogStep("", "FinishAbortedUpgrade starting", false);

            // Run upgrade again for all versions that do not have a corresponding logfile
            foreach (var upgradeVersion in Settings.Installation.UpgradeVersionList)
            {
                if (!IsUpgradeComplete(upgradeVersion))
                    UpgradeModule(upgradeVersion);
            }

            logger.LogStep("", "FinishAbortedUpgrade done", false);

            logger.SaveDetailedLog();
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
        internal static readonly bool UpgradeComplete;

        internal static bool IsUpgradeComplete(string version)
        {
            logger.LogStep(version, "IsUgradeComplete checking", false);
            var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + version + ".resources");
            var complete = File.Exists(logFilePath);
            logger.LogStep(version, "IsUgradeComplete: " + complete, false);
            return complete;
        }

        // cache the status
        private static bool? _running;
        internal static bool IsUpgradeRunning
        {
            get
            {
                return _running ?? (_running = new Lock().IsSet).Value;
            }
            private set
            {
                if (value)
                    new Lock().Set();
                else
                    new Lock().Release();
                _running = value;
            }
        }
        #endregion


    }
}