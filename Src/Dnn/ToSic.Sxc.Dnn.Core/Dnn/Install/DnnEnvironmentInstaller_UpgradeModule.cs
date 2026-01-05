using DotNetNuke.Collections;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Hosting;
using ToSic.Eav.Sys;
using ToSic.Sys.Configuration;
using Exception = System.Exception;

namespace ToSic.Sxc.Dnn.Install;

partial class DnnEnvironmentInstaller
{
    internal string UpgradeModule(string version, bool closeWhenDone)
    {
        var l = Log.Fn<string>($"{nameof(version)}: {version}, {nameof(closeWhenDone)}: {closeWhenDone}");

        var logger = new DnnInstallLoggerForVersion(_installLogger, version);

        // if upgrade has to run
        if (!OldFolderExists() && (new Version(version) <= new Version(20, 0, 0)))
        {
            logger.LogAuto("Upgrade skipped because clean Install detected (installation of everything until and including 20.00.00 has been done by 00.00.01.SqlDataProvider)");
            _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
            return version;
        }
        logger.LogUnimportant("UpgradeModule starting");

        // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
        if (/*version != "01.00.00" &&*/ IsUpgradeComplete(version, true, "- Check on Start UpgradeModule"))
        {
            logger.LogAuto("Apparently trying to update this version, but this versions upgrade is apparently completed, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");
        }
        logger.LogAuto("version / upgrade-complete test passed");

        l.A("Will check if IsUpgradeRunning");
        if (IsUpgradeRunning)
        {
            logger.LogAuto("Apparently upgrade is running, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but the upgrade is already running. Aborting upgrade.");
        }
        logger.LogAuto("is-upgrade-running test passed");

        IsUpgradeRunning = true;
        logger.LogAuto("----- Upgrade to " + version + " started -----");

        try
        {
            switch (version)
            {
                case "01.00.00":
                    MigrateUpgradeFolder(logger);
                    AddObsoleteFile(logger);
                    AddObsolete2SxcJs(logger);
                    break;

                // case "15.00.00": // moved to 15.02 because of we accidentally skipped upgrades in 15.01 - see bug #2997
                // case "15.02.00": // originally moved to here, but the Settings.Installation.LastVersionWithServerChanges had not been upgraded
                //                  this results in no file being created for 15.02, so the "IsUpgradeComplete" always fails
                case "15.02.00":    // Set to 15.04 because otherwise the log file is not created if you already have a 15.02+ (necessary to verify UpgradeComplete)
                    MigrateOldDataFoldersAndRelatedV15_02_00(logger);
                    DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00(logger);
                    break;

                case "20.00.00":
                    MigrateSystemCustomFolderV20_00_00(logger);
                    break;

                case "20.00.01":
                    MigrateAppExtensionsFolderV20_00_01(logger);
                    break;

                // Do v21, 22 etc. here before v23

                // In future v23, remove the old /DesktopModules/ToSic_SexyContent folder completely
                case "23.00.00":
                    DeleteFolderToSic_SexyContent_ObsoleteV23Future(logger, version);
                    break;

                    // case "20.xx.xx":
                    // IMPORTANT: when you add a new case, make sure you
                    // 1. upgrade the version number on Settings.Installation.LastVersionWithServerChanges!!!
                    // 2. add it to the Settings.Installation.UpgradeVersionList
                    // If you don't do these two things, various problems will appear

            }
            logger.LogUnimportant("version-list check / switch done");

            // Increase ClientDependency version upon each upgrade (System and all Portals)
            // prevents browsers caching old JS and CSS files for editing, which could cause several errors
            // only set this on the last upgraded version, to prevent crazy updating the client-resource-cache while upgrading
            if (version == EavSystemInfo.VersionString)
            {
                IncreaseClientDependencyVersion(version);

                // Reset Upgrade complete so it's regenerated
                UpgradeCompleteCache.Reset();
                var getNewStatus = UpgradeComplete(true);
                logger.LogAuto($"updated upgrade-complete status to {getNewStatus}");
            }

            _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
            logger.LogAuto("----- Upgrade to " + version + " completed -----");

        }
        catch (Exception e)
        {
            logger.LogAuto("Upgrade failed - " + e.Message);
            throw;
        }
        finally
        {
            IsUpgradeRunning = false;
        }
        logger.LogAuto("UpgradeModule done / returning");
        if (closeWhenDone)
            _installLogger.CloseLogFiles();

        return l.ReturnAndLog(version);
    }

    private bool OldFolderExists() => _isUpgrade
        ??= Directory.Exists(HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash());
    private bool? _isUpgrade;

    private string OldSysFolderRootFullPath(DnnInstallLoggerForVersion logger) => _oldSysFolderRootFullPath
        ??= GetOldSysFolder(logger);
    private static string _oldSysFolderRootFullPath;

    private void DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00(DnnInstallLoggerForVersion logger)
    {
        // ToSic_EAV_DataTimeline cleaning data and change schema for CJson
        try
        {
            const string sql150000 = @"                         
            -- remove trigger generated data from 'ToSIC_EAV_DataTimeline' in batches

            IF OBJECT_ID('dbo.ToSIC_EAV_DataTimeline', 'U') IS NOT NULL
            BEGIN
                WHILE (SELECT COUNT(*) FROM [dbo].[ToSIC_EAV_DataTimeline] WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')) > 0
                BEGIN
                    ;WITH CTE AS
                    (
	                    SELECT TOP 100000 * 
	                    FROM [dbo].[ToSIC_EAV_DataTimeline] 
	                    WHERE [SourceTable] IN ('ToSIC_EAV_Values', 'ToSIC_EAV_EntityRelationships', 'ToSIC_EAV_ValuesDimensions')
	                    )
                    DELETE FROM CTE;
                END;

                -- drop NewData column from 'ToSIC_EAV_DataTimeline'
                IF EXISTS (SELECT * FROM sys.columns WHERE Name = 'NewData' AND Object_ID = OBJECT_ID('ToSIC_EAV_DataTimeline'))
                BEGIN
                    ALTER TABLE ToSIC_EAV_DataTimeline DROP COLUMN NewData;
                END;

                -- add CJson column to 'ToSIC_EAV_DataTimeline'
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = 'CJson' AND Object_ID = OBJECT_ID('ToSIC_EAV_DataTimeline'))
                BEGIN
                    ALTER TABLE ToSIC_EAV_DataTimeline ADD CJson varbinary(max) NULL;
                END;
            END;";
            using var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString);
            sqlConnection.Open();
            using var sqlCommand150000 = new SqlCommand(sql150000, sqlConnection);
            sqlCommand150000.CommandTimeout = 0; // disable sql execution command timeout on sql server
            sqlCommand150000.ExecuteNonQuery();

            logger.LogAuto("Data cleaning and schema change for CJson completed successfully.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during timeline data cleaning - {e.Message}");
        }
    }

    private void MigrateOldDataFoldersAndRelatedV15_02_00(DnnInstallLoggerForVersion logger)
    {
        try
        {
            // move app.json template from old to new location
            _appJsonService.Value.MoveAppJsonTemplateFromOldToNewLocation();

            // migrate old .data-custom folder
            var globalFolder = _globalConfiguration.Value.GlobalFolder();
            var oldDataCustomFolder = Path.Combine(Path.Combine(globalFolder, ".data-custom"));
            var dataCustomFolder = _globalConfiguration.Value.DataCustomFolder();
            if (Directory.Exists(oldDataCustomFolder) && !Directory.Exists(dataCustomFolder))
            {
                Directory.Move(oldDataCustomFolder, dataCustomFolder);
                logger.LogAuto($"Old .data-custom folder migrated to new location: '{dataCustomFolder}'");
            }

            // migrate old .databeta folder
            var oldDataBetaFolder = Path.Combine(Path.Combine(globalFolder, ".databeta"));
            var dataBetaFolder = _globalConfiguration.Value.DataBetaFolder();
            if (Directory.Exists(oldDataBetaFolder) && !Directory.Exists(dataBetaFolder))
            {
                Directory.Move(oldDataBetaFolder, dataBetaFolder);
                logger.LogAuto($"Old .databeta folder migrated to new location: '{dataBetaFolder}'");
            }
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration of old data folders - {e.Message}");
        }
    }

    private string GetOldSysFolder(DnnInstallLoggerForVersion logger)
    {
        var oldSysFolderRootFullPath = HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash();
        try
        {
            if (Directory.Exists(oldSysFolderRootFullPath))
            {
                logger.LogAuto("Old system folder.");
                return oldSysFolderRootFullPath;
            }
            logger.LogAuto("Old system folder does not exist.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}\n" +
                $"{e.Source}\n" +
                $"{e.InnerException?.Message}");
        }
        return oldSysFolderRootFullPath;
    }

    private void AddObsoleteFile(DnnInstallLoggerForVersion logger)
    {
        var obsoleteFilename = "OBSOLETE.txt";

        try
        {
            // add OBSOLETE.txt with appropriate text - remove folder in v21
            var obsoleteFilePath = Path.Combine(OldSysFolderRootFullPath(logger), obsoleteFilename);
            if (!File.Exists(obsoleteFilePath))
            {
                var obsoleteText = $"This folder is OBSOLETE as of 2sxc v20.\n" +
                                   $"It will be removed in v23.\n" +
                                   $"The folder was marked as obsolete on {DateTime.Now:yyyy-MM-dd HH:mm:ss} for backup purposes, but it is no longer in use.\n";

                File.WriteAllText(obsoleteFilePath, obsoleteText);

                logger.LogAuto($"{obsoleteFilename} created with text:\n{obsoleteText}");
            }
            else
                logger.LogAuto($"{obsoleteFilename} already exists, nothing to do.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}.");
        }
    }

    private void AddObsolete2SxcJs(DnnInstallLoggerForVersion logger)
    {
        var obsoleteFilename = @"js\2sxc.api.min.js";

        try
        {
            // add 2sxc.api.min.js appropriate text - remove folder in v21
            var obsoleteFilePath = Path.Combine(OldSysFolderRootFullPath(logger), obsoleteFilename);
            if (File.Exists(obsoleteFilePath))
            {
                var obsoleteText =
                    $@"// IMPORTANT: This file is obsolete and will be deleted in a future 2sxc release.
// You are using '2sxc.api.min.js' from the old, obsolete folder:
//   'DesktopModules/ToSIC_SexyContent/js/2sxc.api.min.js'
// Please update your code to use the new path:
//   'DesktopModules/ToSic.Sxc/js/2sxc.api.min.js'
// For more info, see: https://docs.2sxc.org/abyss/releases/history/v20/breaking.html
// This notice was created on {DateTime.Now:yyyy-MM-dd HH:mm:ss}.

(function() {{
    alert('''2sxc.api.min.js'' is obsolete and will be removed soon.\n\nUpdate your code to use:\n''DesktopModules/ToSic.Sxc/js/2sxc.api.min.js''');
}})();
";
                File.WriteAllText(obsoleteFilePath, obsoleteText); // overwrite existing content

                logger.LogAuto($"'{obsoleteFilename}' overwrite existing content with text:\n{obsoleteText}");
            }
            else
                logger.LogAuto($"'{obsoleteFilename}' not exists, nothing to do.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}.");
        }
    }
    private void MigrateSystemCustomFolderV20_00_00(DnnInstallLoggerForVersion logger)
    {
        try
        {
            var oldSystemCustomFolder = Path.Combine(
                OldSysFolderRootFullPath(logger),
                FolderConstants.DataFolderProtected,
                FolderConstants.DataSubFolderSystemCustom
            );

            var systemCustomFolder = _globalConfiguration.Value.DataCustomFolder();

            if (Directory.Exists(oldSystemCustomFolder))
            {
                Helpers.DirectoryCopy(oldSystemCustomFolder, systemCustomFolder, true);
                logger.LogAuto($"Old '{FolderConstants.DataSubFolderSystemCustom}' folder migrated to new location: " + systemCustomFolder);
            }
            else
                logger.LogAuto($"Old '{FolderConstants.DataSubFolderSystemCustom}' folder does not exist.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}");
        }
    }

    // Migrate the 'system' (AppExtensions) folder
    private void MigrateAppExtensionsFolderV20_00_01(DnnInstallLoggerForVersion logger)
    {
        try
        {
            var oldAppExtensionsFolder = Path.Combine(
                OldSysFolderRootFullPath(logger),
                FolderConstants.AppExtensionsLegacyFolder
            );

            var newAppExtensionsFolder = Path.Combine(
                _globalConfiguration.Value.GlobalFolder(),
                FolderConstants.AppExtensionsLegacyFolder
            );

            if (Directory.Exists(oldAppExtensionsFolder))
            {
                Helpers.DirectoryCopy(oldAppExtensionsFolder, newAppExtensionsFolder, true);
                logger.LogAuto($"Old '{FolderConstants.AppExtensionsLegacyFolder}' folder migrated to new location: {newAppExtensionsFolder}");

                RenameAllOldDataSubfolders(logger, newAppExtensionsFolder);
            }
            else
                logger.LogAuto($"Old '{FolderConstants.AppExtensionsLegacyFolder}' folder does not exist.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}");
        }
    }

    // Rename all old '.data' subfolders to 'App_Data' that are in appExtension subfolders in 'system' folder
    private void RenameAllOldDataSubfolders(DnnInstallLoggerForVersion logger, string newAppExtensionsFolder)
    {
        logger.LogAuto($"Start renaming in app extensions subfolders from '{FolderConstants.DataFolderProtected}' to '{FolderConstants.DataFolderProtected}'");

        new DirectoryInfo(newAppExtensionsFolder).GetDirectories()
            .SelectMany(appExtensionDirectory => appExtensionDirectory.GetDirectories(FolderConstants.DataFolderOld))
            .ForEach(oldDotDataDirectory => RenameOldDotDataToAppData(logger, oldDotDataDirectory));

        logger.LogAuto($"Finish renaming in app extensions subfolders from '{FolderConstants.DataFolderProtected}' to '{FolderConstants.DataFolderProtected}'");
    }

    // Rename old '.data' folder to 'App_Data'
    private void RenameOldDotDataToAppData(DnnInstallLoggerForVersion logger, DirectoryInfo oldDotDataDirectory)
    {
        var destDirName = Path.Combine(oldDotDataDirectory.Parent!.FullName, FolderConstants.DataFolderProtected);
        try
        {
            if (!Directory.Exists(destDirName))
            {
                oldDotDataDirectory.MoveTo(destDirName);
                logger.LogAuto($"Renamed: '{oldDotDataDirectory.FullName}'");
            }
            else
            {
                // fallback strategy if directory already exists
                Helpers.DirectoryCopy(oldDotDataDirectory.FullName, destDirName, true);
                oldDotDataDirectory.Delete(true);
                logger.LogAuto($"Coped and deleted: '{oldDotDataDirectory.FullName}'");
            }
            
        }
        catch (Exception ex)
        {
            logger.LogAuto($"Error during renaming: '{oldDotDataDirectory.FullName}' - {ex.Message}");
        }
    }

    private void MigrateUpgradeFolder(DnnInstallLoggerForVersion logger)
    {
        try
        {
            const string upgrade = "Upgrade";
            var oldUpgradeFolder = Path.Combine(
                OldSysFolderRootFullPath(logger),
                upgrade
                );

            var upgradeFolder = Path.Combine(
                _globalConfiguration.Value.GlobalFolder(),
                upgrade
                );

            if (Directory.Exists(oldUpgradeFolder))
            {
                Helpers.DirectoryCopy(oldUpgradeFolder, upgradeFolder, true);
                logger.LogAuto($"Old system '{upgrade}' folder migrated to new location: " + upgradeFolder);
            }
            else
                logger.LogAuto($"Old system '{upgrade}' folder does not exist.");
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during migration - {e.Message}");
        }
    }

    /// <summary>
    /// Clean up special /system folder which was moved in v20 and is obsolete now.
    /// At v20 we don't remove everything, so people can still find their code if they had anything custom.
    /// But we will do a clean-up in v23 or later.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="version"></param>
    private void DeleteFolderToSic_SexyContent_ObsoleteV23Future(DnnInstallLoggerForVersion logger, string version)
    {
        var oldSysFolderRootFullPath = HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash();
        logger.LogAuto($"Attempting to delete old system folder: '{oldSysFolderRootFullPath}'.");
        try
        {
            if (Directory.Exists(oldSysFolderRootFullPath))
            {
                Directory.Delete(oldSysFolderRootFullPath, true);
                logger.LogAuto($"Old system folder deleted successfully.");
            }
            else
            {
                logger.LogAuto($"Old system folder does not exist, nothing to delete.");
            }
        }
        catch (Exception e)
        {
            logger.LogAuto($"Error during deletion of old system folder - {e.Message}");
        }
    }

    private void IncreaseClientDependencyVersion(string version)
    {
        _installLogger.LogStep(version, "ClientResourceManager- seems to be last item in version-list, will clear");

        HostController.Instance.IncrementCrmVersion(true);
        DataCache.ClearCache();

        _installLogger.LogStep(version, "ClientResourceManager- done clearing");
    }
}