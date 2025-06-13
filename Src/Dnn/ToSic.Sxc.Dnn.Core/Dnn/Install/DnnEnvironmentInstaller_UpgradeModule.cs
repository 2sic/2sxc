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
            _installLogger.LogStep(version, "Upgrade skipped because clean Install detected (installation of everything until and including 20.00.00 has been done by 00.00.01.SqlDataProvider)", true);
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
                    MigrateUpgradeFolder(version, OldSysFolderRootFullPath(version));
                    break;

                // All versions before v15 should trigger this
                case "15.00.00":
                    throw new("Trying to upgrade pre v15 version - which isn't supported in v20+. Please first upgrade with older v15-v19 LTS, before running this upgrade to a v20+");

                // case "15.00.00": // moved to 15.02 because of we accidentally skipped upgrades in 15.01 - see bug #2997
                // case "15.02.00": // originally moved to here, but the Settings.Installation.LastVersionWithServerChanges had not been upgraded
                //                  this results in no file being created for 15.02, so the "IsUpgradeComplete" always fails
                case "15.02.00":    // Set to 15.04 because otherwise the log file is not created if you already have a 15.02+ (necessary to verify UpgradeComplete)
                    MigrateOldDataFoldersAndRelated(version);
                    DataTimelineCleaningDataAndChangeSchemaForCJson(version);
                    break;

                case "19.99.00":
                    AddObsoleteFile(version, _oldSysFolderRootFullPath);
                    MigrateSystemCustomFolder(version, _oldSysFolderRootFullPath);
                    MigrateUpgradeFolder(version, _oldSysFolderRootFullPath);
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

    private string OldSysFolderRootFullPath(string version) => _oldSysFolderRootFullPath ??= GetOldSysFolderAndRenameIt(version);
    private string _oldSysFolderRootFullPath;

    private void DataTimelineCleaningDataAndChangeSchemaForCJson(string version)
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

            _installLogger.LogStep(version, $"{nameof(DataTimelineCleaningDataAndChangeSchemaForCJson)} - Data cleaning and schema change for CJson completed successfully.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(DataTimelineCleaningDataAndChangeSchemaForCJson)} - Error during timeline data cleaning - {e.Message}");
        }
    }

    private void MigrateOldDataFoldersAndRelated(string version)
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
                _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelated)} - Old .data-custom folder migrated to new location: '{dataCustomFolder}'");
            }

            // migrate old .databeta folder
            var oldDataBetaFolder = Path.Combine(Path.Combine(globalFolder, ".databeta"));
            var dataBetaFolder = _globalConfiguration.Value.DataBetaFolder();
            if (Directory.Exists(oldDataBetaFolder) && !Directory.Exists(dataBetaFolder))
            {
                Directory.Move(oldDataBetaFolder, dataBetaFolder);
                _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelated)} - Old .databeta folder migrated to new location: '{dataBetaFolder}'");
            }
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateOldDataFoldersAndRelated)} - Error during migration of old data folders - {e.Message}");
        }
    }

    private string GetOldSysFolderAndRenameIt(string version)
    {
        var oldSysFolderRootFullPath = HostingEnvironment.MapPath(DnnConstants.OldSysFolderRootVirtual).TrimLastSlash();
        try
        {
            var obsoleteFolderName = oldSysFolderRootFullPath + "_OBSOLETE";
            _installLogger.LogStep(version, $"{nameof(GetOldSysFolderAndRenameIt)} - Attempting to rename from '{oldSysFolderRootFullPath}' to '{obsoleteFolderName}'.");
            if (Directory.Exists(oldSysFolderRootFullPath) && !Directory.Exists(obsoleteFolderName))
            {
                Directory.Move(oldSysFolderRootFullPath, obsoleteFolderName);
                _installLogger.LogStep(version, $"{nameof(GetOldSysFolderAndRenameIt)} - Old system folder moved to OBSOLETE.");
                return obsoleteFolderName;
            }
            if (Directory.Exists(obsoleteFolderName))
            {
                _installLogger.LogStep(version, $"{nameof(GetOldSysFolderAndRenameIt)} - Old system folder already moved to OBSOLETE.");
                return obsoleteFolderName;
            }
            _installLogger.LogStep(version, $"{nameof(GetOldSysFolderAndRenameIt)} - Old system folder does not exist, nothing to rename.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(GetOldSysFolderAndRenameIt)} - Error during migration - {e.Message}\n" +
                $"{e.Source}\n" +
                $"{e.InnerException?.Message}");
        }
        return oldSysFolderRootFullPath;
    }

    private void AddObsoleteFile(string version, string oldSysFolderRootFullPath)
    {
        var obsoleteFilename = "OBSOLETE.txt";

        try
        {
            // add OBSOLETE.txt with appropriate text - remove folder in v21
            var obsoleteFilePath = Path.Combine(oldSysFolderRootFullPath, obsoleteFilename);
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

    private void MigrateSystemCustomFolder(string version, string oldSysFolderRootFullPath)
    {
        try
        {
            var oldSystemCustomFolder = Path.Combine(
                oldSysFolderRootFullPath,
                FolderConstants.AppDataProtectedFolder,
                FolderConstants.FolderSystemCustom
            );

            var systemCustomFolder = _globalConfiguration.Value.DataCustomFolder();

            if (Directory.Exists(oldSystemCustomFolder))
            {
                Helpers.DirectoryCopy(oldSystemCustomFolder, systemCustomFolder, true);
                _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolder)} - Old system custom folder migrated to new location: " + systemCustomFolder);
            }
            else
                _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolder)} - Old system custom folder does not exist.");
        }
        catch (Exception e)
        {
            _installLogger.LogStep(version, $"{nameof(MigrateSystemCustomFolder)} - Error during migration - " + e.Message);
        }
    }

    private void MigrateUpgradeFolder(string version, string oldSysFolderRootFullPath)
    {
        try
        {
            const string upgrade = "Upgrade";
            var oldUpgradeFolder = Path.Combine(
                oldSysFolderRootFullPath,
                upgrade
                );

            var upgradeFolder = Path.Combine(
                _globalConfiguration.Value.GlobalFolder(),
                upgrade
                );

            if (Directory.Exists(oldUpgradeFolder))
            {
                Helpers.DirectoryCopy(oldUpgradeFolder, upgradeFolder, true);
                _installLogger.LogStep(version, $"{nameof(MigrateUpgradeFolder)} - Old system upgrade folder migrated to new location: " + upgradeFolder);
            }
            else
                _installLogger.LogStep(version, $"{nameof(MigrateUpgradeFolder)} - Old system upgrade folder does not exist.");
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