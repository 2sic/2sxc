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

        // if upgrade has to run
        if (!OldFolderExists() && (new Version(version) <= new Version(20, 0, 0)))
        {
            _installLogger.LogStep(version, "Upgrade skipped because clean Install detected (installation of everything until and including 20.00.00 has been done by 00.00.01.SqlDataProvider)");
            _installLogger.LogVersionCompletedToPreventRerunningTheUpgrade(version);
            return version;
        }
        _installLogger.LogStep(version, "UpgradeModule starting", false);

        // Abort upgrade if it's already done - if version is 01.00.00, the module has probably been uninstalled - continue in this case.
        if (/*version != "01.00.00" &&*/ IsUpgradeComplete(version, true, "- Check on Start UpgradeModule"))
        {
            _installLogger.LogStep(version, "Apparently trying to update this version, but this versions upgrade is apparently completed, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but it looks like the upgrade for this version is already complete. Aborting upgrade.");
        }
        _installLogger.LogStep(version, "version / upgrade-complete test passed");

        l.A("Will check if IsUpgradeRunning");
        if (IsUpgradeRunning)
        {
            _installLogger.LogStep(version, "Apparently upgrade is running, will abort");
            throw new("2sxc upgrade for version " + version +
                      " started, but the upgrade is already running. Aborting upgrade.");
        }
        _installLogger.LogStep(version, "is-upgrade-running test passed");

        IsUpgradeRunning = true;
        _installLogger.LogStep(version, "----- Upgrade to " + version + " started -----");

        try
        {
            switch (version)
            {
                case "01.00.00":
                    MigrateUpgradeFolder(version);
                    AddObsoleteFile(version);
                    AddObsolete2SxcJs(version);
                    break;

                // case "15.00.00": // moved to 15.02 because of we accidentally skipped upgrades in 15.01 - see bug #2997
                // case "15.02.00": // originally moved to here, but the Settings.Installation.LastVersionWithServerChanges had not been upgraded
                //                  this results in no file being created for 15.02, so the "IsUpgradeComplete" always fails
                case "15.02.00":    // Set to 15.04 because otherwise the log file is not created if you already have a 15.02+ (necessary to verify UpgradeComplete)
                    MigrateOldDataFoldersAndRelatedV15_02_00(version);
                    DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00(version);
                    break;

                case "20.00.00":
                    MigrateSystemCustomFolderV20_00_00(version);
                    break;

                case "20.00.01":
                    MigrateAppExtensionsFolderV20_00_01(version);
                    break;

                case "21.00.00":
                    DeleteObsoleteFolder(version);
                    break;

                    // case "20.xx.xx":
                    // IMPORTANT: when you add a new case, make sure you
                    // 1. upgrade the version number on Settings.Installation.LastVersionWithServerChanges!!!
                    // 2. add it to the Settings.Installation.UpgradeVersionList
                    // If you don't do these two things, various problems will appear

            }
            _installLogger.LogStep(version, "version-list check / switch done", false);

            // Increase ClientDependency version upon each upgrade (System and all Portals)
            // prevents browsers caching old JS and CSS files for editing, which could cause several errors
            // only set this on the last upgraded version, to prevent crazy updating the client-resource-cache while upgrading
            if (version == EavSystemInfo.VersionString)
            {
                IncreaseClientDependencyVersion(version);

                // Reset Upgrade complete so it's regenerated
                UpgradeCompleteCache.Reset();
                var getNewStatus = UpgradeComplete(true);
                _installLogger.LogStep(version, $"updated upgrade-complete status to {getNewStatus}");
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
        if (closeWhenDone)
            _installLogger.CloseLogFiles();

        return l.ReturnAndLog(version);
    }

    private bool OldFolderExists() => _isUpgrade ??= Directory.Exists(HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash());
    private bool? _isUpgrade;

    private string OldSysFolderRootFullPath(string version) => _oldSysFolderRootFullPath ??= GetOldSysFolder(version);
    private static string _oldSysFolderRootFullPath;

    private void DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00(string version)
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

            _installLogger.LogStep(version, $"{nameof(DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00)} - Data cleaning and schema change for CJson completed successfully.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(DataTimelineCleaningDataAndChangeSchemaForCJsonV15_02_00)} - Error during timeline data cleaning - {e.Message}");
        }
    }

    private void MigrateOldDataFoldersAndRelatedV15_02_00(string version)
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
                _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelatedV15_02_00)} - Old .data-custom folder migrated to new location: '{dataCustomFolder}'");
            }

            // migrate old .databeta folder
            var oldDataBetaFolder = Path.Combine(Path.Combine(globalFolder, ".databeta"));
            var dataBetaFolder = _globalConfiguration.Value.DataBetaFolder();
            if (Directory.Exists(oldDataBetaFolder) && !Directory.Exists(dataBetaFolder))
            {
                Directory.Move(oldDataBetaFolder, dataBetaFolder);
                _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelatedV15_02_00)} - Old .databeta folder migrated to new location: '{dataBetaFolder}'");
            }
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelatedV15_02_00)} - Error during migration of old data folders - {e.Message}");
        }
    }

    private string GetOldSysFolder(string version)
    {
        var oldSysFolderRootFullPath = HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash();
        try
        {
            if (Directory.Exists(oldSysFolderRootFullPath))
            {
                _installLogger.LogStep(version, $"{nameof(GetOldSysFolder)} - Old system folder.");
                return oldSysFolderRootFullPath;
            }
            _installLogger.LogStep(version, $"{nameof(GetOldSysFolder)} - Old system folder does not exist.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(GetOldSysFolder)} - Error during migration - {e.Message}\n" +
                $"{e.Source}\n" +
                $"{e.InnerException?.Message}");
        }
        return oldSysFolderRootFullPath;
    }

    private void AddObsoleteFile(string version)
    {
        var obsoleteFilename = "OBSOLETE.txt";

        try
        {
            // add OBSOLETE.txt with appropriate text - remove folder in v21
            var obsoleteFilePath = Path.Combine(OldSysFolderRootFullPath(version), obsoleteFilename);
            if (!File.Exists(obsoleteFilePath))
            {
                var obsoleteText = $"This folder is OBSOLETE as of 2sxc v20.\n" +
                                   $"It will be removed in v21.\n" +
                                   $"The folder was marked as obsolete on {DateTime.Now:yyyy-MM-dd HH:mm:ss} for backup purposes, but it is no longer in use.\n";

                File.WriteAllText(obsoleteFilePath, obsoleteText);

                _installLogger.LogStep(version, $"{nameof(AddObsoleteFile)} - {obsoleteFilename} created with text:\n{obsoleteText}");
            }
            else
                _installLogger.LogStep(version, $"{nameof(AddObsoleteFile)} - {obsoleteFilename} already exists, nothing to do.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(AddObsoleteFile)} - Error during migration - {e.Message}.");
        }
    }

    private void AddObsolete2SxcJs(string version)
    {
        var obsoleteFilename = @"js\2sxc.api.min.js";

        try
        {
            // add 2sxc.api.min.js appropriate text - remove folder in v21
            var obsoleteFilePath = Path.Combine(OldSysFolderRootFullPath(version), obsoleteFilename);
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

                _installLogger.LogStep(version, $"{nameof(AddObsolete2SxcJs)} - '{obsoleteFilename}' overwrite existing content with text:\n{obsoleteText}");
            }
            else
                _installLogger.LogStep(version, $"{nameof(AddObsolete2SxcJs)} - '{obsoleteFilename}' not exists, nothing to do.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(AddObsolete2SxcJs)} - Error during migration - {e.Message}.");
        }
    }
    private void MigrateSystemCustomFolderV20_00_00(string version)
    {
        try
        {
            var oldSystemCustomFolder = Path.Combine(
                OldSysFolderRootFullPath(version),
                FolderConstants.DataFolderProtected,
                FolderConstants.DataSubFolderSystemCustom
            );

            var systemCustomFolder = _globalConfiguration.Value.DataCustomFolder();

            if (Directory.Exists(oldSystemCustomFolder))
            {
                Helpers.DirectoryCopy(oldSystemCustomFolder, systemCustomFolder, true);
                _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolderV20_00_00)} - Old '{FolderConstants.DataSubFolderSystemCustom}' folder migrated to new location: " + systemCustomFolder);
            }
            else
                _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolderV20_00_00)} - Old '{FolderConstants.DataSubFolderSystemCustom}' folder does not exist.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolderV20_00_00)} - Error during migration - " + e.Message);
        }
    }

    // Migrate the 'system' (AppExtensions) folder
    private void MigrateAppExtensionsFolderV20_00_01(string version)
    {
        try
        {
            var oldAppExtensionsFolder = Path.Combine(
                OldSysFolderRootFullPath(version),
                FolderConstants.AppExtensionsFolder
            );

            var newAppExtensionsFolder = Path.Combine(
                _globalConfiguration.Value.GlobalFolder(),
                FolderConstants.AppExtensionsFolder
            );

            if (Directory.Exists(oldAppExtensionsFolder))
            {
                Helpers.DirectoryCopy(oldAppExtensionsFolder, newAppExtensionsFolder, true);
                _installLogger.LogStep(version, $"{nameof(MigrateAppExtensionsFolderV20_00_01)} - Old '{FolderConstants.AppExtensionsFolder}' folder migrated to new location: " + newAppExtensionsFolder);

                RenameAllOldDataSubfolders(version, newAppExtensionsFolder);
            }
            else
                _installLogger.LogStep(version, $"{nameof(MigrateAppExtensionsFolderV20_00_01)} - Old '{FolderConstants.AppExtensionsFolder}' folder does not exist.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateAppExtensionsFolderV20_00_01)} - Error during migration - {e.Message}");
        }
    }

    // Rename all old '.data' subfolders to 'App_Data' that are in appExtension subfolders in 'system' folder
    private void RenameAllOldDataSubfolders(string version, string newAppExtensionsFolder)
    {
        _installLogger.LogStep(version, $"{nameof(RenameAllOldDataSubfolders)} - Start renaming in app extensions subfolders from '{FolderConstants.DataFolderProtected}' to '{FolderConstants.DataFolderProtected}'");

        new DirectoryInfo(newAppExtensionsFolder).GetDirectories()
            .SelectMany(appExtensionDirectory => appExtensionDirectory.GetDirectories(FolderConstants.DataFolderOld))
            .ForEach(oldDotDataDirectory => RenameOldDotDataToAppData(version, oldDotDataDirectory));

        _installLogger.LogStep(version, $"{nameof(RenameAllOldDataSubfolders)} - Finish renaming in app extensions subfolders from '{FolderConstants.DataFolderProtected}' to '{FolderConstants.DataFolderProtected}'");
    }

    // Rename old '.data' folder to 'App_Data'
    private void RenameOldDotDataToAppData(string version, DirectoryInfo oldDotDataDirectory)
    {
        var destDirName = Path.Combine(oldDotDataDirectory.Parent!.FullName, FolderConstants.DataFolderProtected);
        try
        {
            if (!Directory.Exists(destDirName))
            {
                oldDotDataDirectory.MoveTo(destDirName);
                _installLogger.LogStep(version, $"{nameof(RenameOldDotDataToAppData)} - Renamed: '{oldDotDataDirectory.FullName}'");
            }
            else
            {
                // fallback strategy if directory already exists
                Helpers.DirectoryCopy(oldDotDataDirectory.FullName, destDirName, true);
                oldDotDataDirectory.Delete(true);
                _installLogger.LogStep(version, $"{nameof(RenameOldDotDataToAppData)} - Coped and deleted: '{oldDotDataDirectory.FullName}'");
            }
            
        }
        catch (Exception ex)
        {
            _installLogger.LogStep(version, $"{nameof(RenameOldDotDataToAppData)} - Error during renaming: '{oldDotDataDirectory.FullName}' - {ex.Message}");
        }
    }

    private void MigrateUpgradeFolder(string version)
    {
        try
        {
            const string upgrade = "Upgrade";
            var oldUpgradeFolder = Path.Combine(
                OldSysFolderRootFullPath(version),
                upgrade
                );

            var upgradeFolder = Path.Combine(
                _globalConfiguration.Value.GlobalFolder(),
                upgrade
                );

            if (Directory.Exists(oldUpgradeFolder))
            {
                Helpers.DirectoryCopy(oldUpgradeFolder, upgradeFolder, true);
                _installLogger.LogStep(version, $"{nameof(MigrateUpgradeFolder)} - Old system '{upgrade}' folder migrated to new location: " + upgradeFolder);
            }
            else
                _installLogger.LogStep(version, $"{nameof(MigrateUpgradeFolder)} - Old system '{upgrade}' folder does not exist.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateUpgradeFolder)} - Error during migration - {e.Message}");
        }
    }

    private void DeleteObsoleteFolder(string version)
    {
        var oldSysFolderRootFullPath = HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash() + "_OBSOLETE";
        _installLogger.LogStep(version, $"{nameof(DeleteObsoleteFolder)} - Attempting to delete old system folder: '{oldSysFolderRootFullPath}'.");
        try
        {
            if (Directory.Exists(oldSysFolderRootFullPath))
            {
                Directory.Delete(oldSysFolderRootFullPath, true);
                _installLogger.LogStep(version, $"{nameof(DeleteObsoleteFolder)} - Old system folder deleted successfully.");
            }
            else
            {
                _installLogger.LogStep(version, $"{nameof(DeleteObsoleteFolder)} - Old system folder does not exist, nothing to delete.");
            }
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(DeleteObsoleteFolder)} - Error during deletion of old system folder - {e.Message}");
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